using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JT76.Data.Models;

namespace JT76.Data.Factories
{
    public enum ErrorLevels
    {
        Message,
        Warning,
        Critical,
        Default
    };

    public static class ErrorFactory
    {
        public static Error GetErrorFromException(Exception e, ErrorLevels errorLevel, string strAdditionalInformation)
        {
            Debug.WriteLine("ErrorFactory.GetErrorFromException()");

            var error = new Error
            {
                StrMessage = e.Message + ((e.InnerException == null) ? "" : "     |Inner Exception| " + e.InnerException.Message),
                StrSource = e.Source + "     |Module| " + (e.TargetSite != null ? e.TargetSite.Module.Name : "Aggregate Exception") + "     |Class| " + ((e.TargetSite != null && e.TargetSite.ReflectedType != null) ? e.TargetSite.ReflectedType.Name : "No Reflected Name Found"),
                StrErrorLevel = "     |Error Level|" + Enum.GetName(typeof(ErrorLevels), errorLevel),
                StrAdditionalInformation = strAdditionalInformation,
                StrStackTrace = e.StackTrace + (e.InnerException == null ? "             |No inner exception| " : "             |Inner Exception| " + GetErrorAsString(e.InnerException)),
                DtCreated = DateTime.UtcNow
            };

            return error;
        }

        public static string GetErrorAsHtml(Exception e)
        {
            Debug.WriteLine("ErrorFactory.GetErrorAsHtml()");

            var sb = new StringBuilder();
            bool first = true;

            while (e != null)
            {
                sb.Append(first ? "<br/>" : "<br/><br/><br/>");
                first = false;
                sb.Append("|Message| " + e.Message + "<br/>");
                sb.Append("|Module| " + (e.TargetSite != null ? e.TargetSite.Module.Name : "Aggregate Exception") + "<br/>");
                sb.Append("|Source| " + e.Source + "<br/>");
                sb.Append("|Class| " + (e.TargetSite != null && e.TargetSite.ReflectedType != null ? e.TargetSite.ReflectedType.Name + "<br/>" : "No Reflected Name Found<br/>"));
                sb.Append("|Target Site| " + (e.TargetSite != null ? e.TargetSite.ToString() : "Aggregate Exception") + "<br/>");
                sb.AppendLine("[Stack Trace|<br/>");
                foreach (var item in GetStackStraceStrings(e.StackTrace))
                {
                    sb.AppendLine("&nbsp;&nbsp;&nbsp;" + item + "<br/>");
                }
                sb.AppendLine("<br/>");
                sb.Append(e.InnerException == null ? "<br/>|No Inner Exception|<br/>" : "<br/>|Inner Exception|: <br/>");
                e = e.InnerException;
            }

            return sb.ToString();
        }

        public static string GetErrorAsString(Exception e)
        {
            Debug.WriteLine("ErrorFactory.GetErrorAsString()");

            var sb = new StringBuilder();
            bool first = true;

            while (e != null)
            {
                if (!first)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
                first = false;
                sb.AppendLine("|Message| " + e.Message);
                sb.AppendLine("|Time| " + DateTime.Now);
                sb.AppendLine("|Module| " + (e.TargetSite != null ? e.TargetSite.Module.Name : "Aggregate Exception"));
                sb.AppendLine("|Source| " + e.Source);
                sb.AppendLine("|Class| " + (e.TargetSite != null && e.TargetSite.ReflectedType != null ? e.TargetSite.ReflectedType.Name : "No Reflected Name Found"));
                sb.AppendLine("|Target Site| " + e.TargetSite ?? "Aggregate Exception");
                sb.AppendLine("[Stack Trace|");
                foreach (string item in GetStackStraceStrings(e.StackTrace))
                {
                    sb.AppendLine("  " + item);
                }
                sb.AppendLine("");
                sb.AppendLine(e.InnerException == null ? "|No Inner Exception|" : "\n|Inner Exception|: ");
                e = e.InnerException;
            }

            return sb.ToString();
        }

        public static string GetAggregateErrorAsString(AggregateException ae)
        {
            Debug.WriteLine("ErrorFactory.GetAggregateErrorAsString()");

            var sb = new StringBuilder();
            sb.AppendLine("|AGGREGATE ERROR|");
            sb.AppendLine(GetErrorAsString(ae.GetBaseException()));
            sb.AppendLine("");

            foreach (var e in ae.InnerExceptions)
            {
                sb.AppendLine(GetErrorAsString(e));
                sb.AppendLine("");
            }

            return sb.ToString();
        }

        //helper method for throwing an aggregate exception
        private static string[] GetAllFiles(string str)
        {
            Debug.WriteLine("ErrorFactory.GetAllFiles()");

            // Should throw an UnauthorizedAccessException exception. 
            return System.IO.Directory.GetFiles(str, "*.txt", SearchOption.AllDirectories);
        }

        public static bool GetThrownException()
        {
            Debug.WriteLine("ErrorFactory.GetThrownException()");

            int n = 0;
            int divideByZero = 1 / n;

            return false;
        }

        /// <summary>
        /// Throws an aggregate exception.
        /// </summary>
        /// <returns>An awaitable method that will cause an aggregate exception</returns>
        public static Task<string[][]> GetThrownAggregateException()
        {
            Debug.WriteLine("ErrorFactory.GetThrownAggregateException()");

            // Get a folder path whose directories should throw an UnauthorizedAccessException. 
            string path = Directory.GetParent(
                                    Environment.GetFolderPath(
                                    Environment.SpecialFolder.UserProfile)).FullName;

            var task = Task<string[]>.Factory.StartNew(() => GetAllFiles(path));
            var taskTwo = Task<string[]>.Factory.StartNew(() => { throw new IndexOutOfRangeException(); });

            Task.WaitAll(task, taskTwo); //waits on all
            return Task.WhenAll(task, taskTwo);
        }


        private static IEnumerable<string> GetStackStraceStrings(string strStackTrace)
        {
            Debug.WriteLine("ErrorFactory.GetStackStraceStrings()");

            return strStackTrace == null ? new[] { "" } :
                strStackTrace.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }
    }
}
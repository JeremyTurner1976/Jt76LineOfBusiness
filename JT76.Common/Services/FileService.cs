using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using JT76.Common.ObjectExtensions;

namespace JT76.Common.Services
{
    //Production (TimedService): Have a service that will run on the server timed to handle file cleanup operations

    public enum DirectoryFolders
    {
        Jt76Errors,
        Jt76Logs,
        Jt76Email,
        Jt76Data,
        Jt76Test,
    }

    public interface IFileService
    {
        bool SaveTextToDirectoryFile(DirectoryFolders directory, string strMessage);

        string[] LoadTextFromDirectoryFile(DirectoryFolders directory, String strFileName = "",
            DateTime dtIdentifier = new DateTime());

        bool DeleteOldFilesInFolder(DirectoryFolders directory, int nFilesToSave);
        bool DeleteFilesByDays(DirectoryFolders directory, int nDays);

        string GetDirectoryFolderLocation(DirectoryFolders directory);
    }

    public class FileService : IFileService
    {
        private const int NDaysToHoldDirectoryFiles = 7;
        private const int NMaxDirectoryFolderFiles = 10;
        private readonly string _strExecutingDirectory;

        public FileService()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _strExecutingDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //Production: Service will handle this
            Init();
        }

        public bool SaveTextToDirectoryFile(DirectoryFolders directory, string strMessage)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            string strLocationAndFile = GetDirectoryFileLocation(directory);
            File.AppendAllText(strLocationAndFile, strMessage + Environment.NewLine);

            return true;
        }

        public string[] LoadTextFromDirectoryFile(DirectoryFolders directory, String strFileName = "",
            DateTime dtIdentifier = new DateTime())
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            string strFolderLocation = GetDirectoryFolderLocation(directory);

            string strFileAndPathName;

            if (!string.IsNullOrEmpty(strFileName))
                strFileAndPathName = strFolderLocation + "\\" + strFileName;
            else
            {
                string strEnum = directory.ToNameString();
                strFileAndPathName = strFolderLocation + "\\" + strEnum + "_" + dtIdentifier.ToString("M-dd-yyyy") +
                                     ".txt";
            }

            if (strFileAndPathName.Contains(".txt") && File.Exists(strFileAndPathName))
                return File.ReadAllLines(strFileAndPathName);
            throw new FileNotFoundException();
        }

        public bool DeleteOldFilesInFolder(DirectoryFolders directory, int nNewestFilesToSave)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            string strFolderLocation = GetDirectoryFolderLocation(directory);

            foreach (
                FileInfo file in
                    new DirectoryInfo(strFolderLocation).GetFiles()
                        .OrderByDescending(x => x.LastWriteTime)
                        .Skip(nNewestFilesToSave))
                file.Delete();

            return true;
        }

        public bool DeleteFilesByDays(DirectoryFolders directory, int nDays)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            string strFolderLocation = GetDirectoryFolderLocation(directory);

            foreach (
                FileInfo file in
                    new DirectoryInfo(strFolderLocation).GetFiles()
                        .Where(x => x.LastWriteTime <= DateTime.Now.AddDays(nDays*-1)))
                file.Delete();

            return true;
        }

        private bool Init()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            Array enumValues = Enum.GetValues(typeof (DirectoryFolders));
            foreach (object value in enumValues)
            {
                if (!Directory.Exists(GetDirectoryFolderLocation((DirectoryFolders) value)))
                    Directory.CreateDirectory(GetDirectoryFolderLocation((DirectoryFolders) value));

                if ((DirectoryFolders) value != DirectoryFolders.Jt76Data)
                {
                    //DeleteFilesByDays((DirectoryFolders) value, NDaysToHoldDirectoryFiles);
                    DeleteOldFilesInFolder((DirectoryFolders) value, NMaxDirectoryFolderFiles);
                }
            }

            return true;
        }

        public string GetDirectoryFolderLocation(DirectoryFolders directory)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            string strEnum = directory.ToNameString();
            return _strExecutingDirectory + "\\App_Data\\" + strEnum;
        }

        private string GetDirectoryFileLocation(DirectoryFolders directory)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            string strEnum = directory.ToNameString();
            return GetDirectoryFolderLocation(directory) + "\\" + strEnum + "_" + DateTime.Now.ToString("M-dd-yyyy") +
                   ".txt";
        }
    }
}
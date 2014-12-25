using System;
using System.Diagnostics;
using System.Reflection;
using JT76.Common.Services;
using JT76.Data.Factories;

namespace JT76.Ui
{
    public interface IUiService
    {
        bool HandleError(Exception e, ErrorLevels errorLevel = ErrorLevels.Default,
            string strAdditionalInformation = "Default Additional Information");

        bool LogMessage(string stringLogMessage);

        string ParseErrorAsHtml(Exception e);

        void SendMeMail(string strBody);
    }

    public class UiService : IUiService
    {
        private readonly ILoggingService _dbLoggingService;
        private readonly ILoggingService _emailLoggingService;
        private readonly ILoggingService _fileLoggingService;

        public UiService(DbLoggingService dbLoggingService, EmailLoggingService emailLoggingService,
            FileLoggingService fileLoggingService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _dbLoggingService = dbLoggingService;
            _emailLoggingService = emailLoggingService;
            _fileLoggingService = fileLoggingService;
        }

        public string ParseErrorAsHtml(Exception e)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return ErrorFactory.GetErrorAsHtml(e);
        }

        public bool LogMessage(string strLogMessage = "")
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            try
            {
                _dbLoggingService.LogMessage(strLogMessage);
            }
            catch (Exception e)
            {
                try
                {
                    _emailLoggingService.LogMessage(strLogMessage);
                }
                catch
                {
                    try
                    {
                        _fileLoggingService.LogMessage(strLogMessage);
                    }
                    catch
                    {
                        //all three message loggers have failed
                        return HandleError(e, ErrorLevels.Default,
                            "Unable to Log the message, all three message loggers have failed" + strLogMessage);
                    }
                }
            }

            return true;
        }

        public bool HandleError(Exception e, ErrorLevels errorLevel = ErrorLevels.Default,
            string strAdditionalInformation = "Default Error Additional Information")
        {
            string strMethodName = GetType().FullName + "." + MethodBase.GetCurrentMethod().Name;
            Debug.WriteLine(strMethodName);

            string strException = "Default Exception String in " + strMethodName;

            try
            {
                strException = ErrorFactory.GetErrorAsString(e);
            }
            catch
            {
                strException += ". ErrorFactory.GetErrorAsString(e) also threw an exception.";
            }

            //attempt to log this with redundancy
            try
            {
                _dbLoggingService.LogError(e, errorLevel, strAdditionalInformation);
            }
            catch
            {
                try
                {
                    _emailLoggingService.LogError(e, errorLevel, strAdditionalInformation);
                }
                catch
                {
                    try
                    {
                        _fileLoggingService.LogError(e, errorLevel, strAdditionalInformation);
                    }
                    catch
                    {
                        //all three Error Loggers have failed
                        strException = strAdditionalInformation + "     \n|EXCEPTION|       " + strException;

                        try
                        {
                            _dbLoggingService.LogMessage(strException);
                        }
                        catch
                        {
                            try
                            {
                                _emailLoggingService.LogMessage(strException);
                            }
                            catch
                            {
                                try
                                {
                                    _fileLoggingService.LogMessage(strException);
                                }
                                catch
                                {
                                    //all three Message Loggers have failed
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void SendMeMail(string strBody)
        {
            _emailLoggingService.LogMessage(strBody);
        }
    }
}
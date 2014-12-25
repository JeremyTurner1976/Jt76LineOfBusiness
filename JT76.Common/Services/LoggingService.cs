using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using JT76.Common.ObjectExtensions;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Factories;
using JT76.Data.Models;

namespace JT76.Common.Services
{
    //interface
    public interface ILoggingService
    {
        bool LogError(Exception e, ErrorLevels errorLevel = ErrorLevels.Default,
            string strAdditionalInformation = "Additional Information Default");

        bool LogMessage(string strLogMessage);
    }

    //EmailLoggingService
    public class EmailLoggingService : ILoggingService
    {
        private readonly IEmailService _emailService;

        public EmailLoggingService(IEmailService emailService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _emailService = emailService;
        }

        public bool LogError(Exception e, ErrorLevels errorLevel = ErrorLevels.Default,
            string strAdditionalInformation = "Additional Information Default")
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            var sb = new StringBuilder();
            sb.AppendLine(strAdditionalInformation);
            sb.AppendLine(errorLevel.ToNameString());
            sb.AppendLine(ErrorFactory.GetErrorAsString(e));

            _emailService.SendMeMail(sb.ToString());
            //async can wait for !Result.Contains(false) if a valid return is wanted
            return true;
        }

        public bool LogMessage(string strLogMessage)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _emailService.SendMeMail(strLogMessage);
            //async can wait for !Result.Contains(false) if a valid return is wanted
            return true;
        }
    }

    //DbLoggingService
    public class DbLoggingService : ILoggingService
    {
        private readonly IErrorRepository _errorRepository;
        private readonly ILogMessageRepository _logMessageRepository;

        public DbLoggingService(IErrorRepository errorRepository, ILogMessageRepository logMessageRepository)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _errorRepository = errorRepository;
            _logMessageRepository = logMessageRepository;
        }

        public bool LogError(Exception e, ErrorLevels errorLevel = ErrorLevels.Default,
            string strAdditionalInformation = "Additional Information Default")
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            Error newError = ErrorFactory.GetErrorFromException(e, errorLevel, strAdditionalInformation);

            return _errorRepository.AddError(newError, true);
        }

        public bool LogMessage(string strLogMessage)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            var logMessage = new LogMessage
            {
                StrLogMessage = strLogMessage,
                DtCreated = DateTime.UtcNow
            };

            return _logMessageRepository.AddLogMessage(logMessage, true);
        }
    }

    //FileLoggingService
    public class FileLoggingService : ILoggingService
    {
        private readonly IFileService _fileService;

        public FileLoggingService(IFileService fileService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _fileService = fileService;
        }

        public bool LogError(Exception e, ErrorLevels errorLevel = ErrorLevels.Default,
            string strAdditionalInformation = "Additional Information Default")
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            var sb = new StringBuilder();
            sb.AppendLine("______________________________ERROR_________________________________");
            sb.AppendLine(strAdditionalInformation);
            sb.AppendLine(errorLevel.ToNameString());
            sb.AppendLine(ErrorFactory.GetErrorAsString(e));
            sb.AppendLine();
            sb.AppendLine();

            _fileService.SaveTextToDirectoryFile(DirectoryFolders.Jt76Errors, sb.ToString());
            return true;
        }

        public bool LogMessage(string strLogMessage)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            var sb = new StringBuilder();
            sb.AppendLine(strLogMessage);

            _fileService.SaveTextToDirectoryFile(DirectoryFolders.Jt76Logs, sb.ToString());
            return true;
        }
    }
}
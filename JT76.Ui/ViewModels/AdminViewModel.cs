using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Models;

namespace JT76.Ui.ViewModels
{
    public class AdminViewModel
    {
        private readonly IErrorRepository _errorRepository;
        private readonly ILogMessageRepository _logMessageRepository;
        private readonly IUiService _uiService;

        public AdminViewModel(IErrorRepository errorRepository, ILogMessageRepository logMessageRepository, IUiService uiService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _errorRepository = errorRepository;
            _logMessageRepository = logMessageRepository;
            _uiService = uiService;
        }

        public bool SaveErrors()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _errorRepository.Save();
        }

        public bool SaveLogMessages()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _logMessageRepository.Save();
        }


        public IQueryable<Error> GetErrors()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _errorRepository.GetErrors();
        }

        public bool AddError(Error newError)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _errorRepository.AddError(newError, true);
            return true;
        }

        public IQueryable<LogMessage> GetLogMessages()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _logMessageRepository.GetLogMessages();
        }

        public bool AddLogMessage(string stringLogMessage = "", LogMessage logMessage = null, bool bSave = true)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _uiService.LogMessage(stringLogMessage);
        }
    }
}
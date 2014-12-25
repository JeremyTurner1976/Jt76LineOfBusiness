using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using JT76.Data.Models;
using JT76.Ui.ViewModels;

namespace JT76.Ui.Controllers
{
    public class LogMessagesApiController : ApiController
    {
        private readonly IUiService _uiService;
        private readonly AdminViewModel _viewModel;

        public LogMessagesApiController(AdminViewModel viewModel, IUiService uiService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _viewModel = viewModel;
            _uiService = uiService;
        }

        //map verbs
        public IEnumerable<LogMessage> Get()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //var requestUri = Request.RequestUri;

            IQueryable<LogMessage> logMessages = _viewModel.GetLogMessages();

            logMessages = logMessages
                .OrderByDescending(x => x.DtCreated);

            //_uiService.LogMessage(logMessages.Count() + " different LogMessages loaded");

            return logMessages;
        }

        public HttpResponseMessage Post([FromBody] LogMessage newLogMessage)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //var requestUri = Request.RequestUri;

            if (newLogMessage.DtCreated == default(DateTime))
                newLogMessage.DtCreated = DateTime.UtcNow;

            if (_viewModel.AddLogMessage(newLogMessage.StrLogMessage)) //force valid datatype
            {
                //200 success
                //_uiService.LogMessage(newLogMessage.StrLogMessage + " - Successfully saved");
                return Request.CreateResponse(HttpStatusCode.Created, newLogMessage);
            }

            //400 LogMessage
            //_uiService.LogMessage(newLogMessage.StrLogMessage + " - Was unable to save");
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
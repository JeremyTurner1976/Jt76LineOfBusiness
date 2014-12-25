using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using JT76.Data.Models;
using JT76.Ui.ViewModels;

namespace JT76.Ui.Controllers
{
    public class RepliesApiController : ApiController
    {
        private readonly IUiService _uiService;
        private readonly MessageBoardViewModel _viewModel;

        public RepliesApiController(MessageBoardViewModel viewModel, IUiService uiService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _viewModel = viewModel;
            _uiService = uiService;
        }

        public IEnumerable<Reply> Get(int topicId)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //var requestUri = Request.RequestUri;

            //_uiService.LogMessage("Replies are being loaded for Topic : " + topicId);
            return _viewModel.GetRepliesByTopic(topicId);
        }

        public HttpResponseMessage Post(int topicId, [FromBody] Reply newReply)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //var requestUri = Request.RequestUri;

            //ensure this will be the right Id
            newReply.TopicId = topicId;

            if (newReply.DtCreated == default(DateTime))
                newReply.DtCreated = DateTime.UtcNow;

            if (ModelState.IsValid && _viewModel.AddReply(newReply))
            {
                //200 success
                //_uiService.LogMessage(topicId + " saved reply: [ " + newReply.StrBody + " ] - Successfully saved");
                return Request.CreateResponse(HttpStatusCode.Created, newReply);
            }

            //400 error
            //_uiService.LogMessage("Topic Id: " + topicId + " tried to save reply: [ " + newReply.StrBody + " ]- Was unable to save");
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
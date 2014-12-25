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
    //special web api class that exposes an API
    public class TopicsApiController : ApiController
    {
        //use a web api to link into Angular

        private readonly IUiService _uiService;
        private readonly MessageBoardViewModel _viewModel;

        public TopicsApiController(MessageBoardViewModel viewModel, IUiService uiService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _viewModel = viewModel;
            _uiService = uiService;
        }

        //map verbs
        public IEnumerable<Topic> Get(bool includeReplies = false)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //var requestUri = Request.RequestUri;

            IQueryable<Topic> topics = includeReplies ? _viewModel.GetTopicsIncludingReplies() : _viewModel.GetTopics();

            topics = topics
                .OrderByDescending(x => x.DtCreated);

            //_uiService.LogMessage(topics.Count() + " different topics loaded");

            return topics;
        }

        public HttpResponseMessage Post([FromBody] Topic newTopic)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //var requestUri = Request.RequestUri;

            if (newTopic.DtCreated == default(DateTime))
                newTopic.DtCreated = DateTime.UtcNow;

            if (ModelState.IsValid && _viewModel.AddTopic(newTopic))
            {
                //200 success
                //_uiService.LogMessage(newTopic.StrTitle + " - Successfully saved");
                return Request.CreateResponse(HttpStatusCode.Created, newTopic);
            }

            //400 error
            //_uiService.LogMessage(newTopic.StrTitle + " - Was unable to save");
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
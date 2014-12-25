using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json.Serialization;

namespace JT76.Ui
{

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            Debug.WriteLine("WebApiConfig.Register()");

            //register validation error json returns
            config.Filters.Add(new ValidateModelAttribute());

            // Web API configuration and services

            //make this return camelcased Json
            JsonMediaTypeFormatter jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //add the Supported Media Outputs
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));

            config.Routes.MapHttpRoute("RepliesRoute", "api/v1/topics/{topicid}/replies/{id}",
                new {controller = "RepliesApi", id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute("TopicsRoute", "api/v1/topics/{id}",
                new {controller = "TopicsApi", id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute("ErrorsRoute", "api/v1/errors/{id}",
                new {controller = "ErrorsApi", id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute("LogMessagesRoute", "api/v1/logMessages/{id}",
                new {controller = "LogMessagesApi", id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute("DefaultApi", "api/v1/{controller}/{id}", new {id = RouteParameter.Optional}
                );
        }
    }
}
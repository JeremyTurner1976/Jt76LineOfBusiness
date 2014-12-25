using System;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using JT76.Ui.Controllers;
using Ninject;

namespace JT76.Ui
{
    public class MvcApplication : HttpApplication
    {
        [Inject]
        public IUiService UiService { get; set; }

        protected void Application_Start()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //In Global.asax to catch all errors
        //At this point the only C# try catch logic wanted is when logging, or wanting to skip a minor exception
        protected void Application_Error(object sender, EventArgs e)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            Exception ex = Server.GetLastError();
            UiService.HandleError(ex);
            Server.ClearError();

            RedirectToErrorPage(ex);
        }

        private void RedirectToErrorPage(Exception ex)
        {
            var httpException = ex as HttpException;

            var routeData = new RouteData();
            routeData.Values["controller"] = "Errors";
            routeData.Values["action"] = "General";
            routeData.Values["exception"] = httpException;

            //Production: Handle any specific Http errors with custom views

            //Response.StatusCode = 500;

            //if (httpException != null)
            //{
            //    Response.StatusCode = httpException.GetHttpCode();
            //    switch (Response.StatusCode)
            //    {
            //        case 403:
            //            routeData.Values["action"] = "Http403";
            //            break;
            //        case 404:
            //            routeData.Values["action"] = "Http404";
            //            break;
            //    }
            //}

            IController errorsController = new ErrorsController(UiService);
            var requestContext = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorsController.Execute(requestContext);
        }
    }
}
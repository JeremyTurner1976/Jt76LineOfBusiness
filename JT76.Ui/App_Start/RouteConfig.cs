using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;

namespace JT76.Ui
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            Debug.WriteLine("RouteConfig.RegisterRoutes()");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}
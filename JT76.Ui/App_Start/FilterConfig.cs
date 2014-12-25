using System.Diagnostics;
using System.Web.Mvc;

namespace JT76.Ui
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            Debug.WriteLine("FilterConfig.RegisterGlobalFilters");

            filters.Add(new HandleErrorAttribute());
        }
    }
}
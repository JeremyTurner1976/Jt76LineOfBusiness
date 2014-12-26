using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace JT76.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUiService _uiService;

        public HomeController(IUiService uiService)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _uiService = uiService;
        }

        public ActionResult Index()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return View();
        }

        public ActionResult Components()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return View();
        }

        public ActionResult MessageBoard()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return View();
        }

        public ActionResult Admin()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return View();
        }

        public ActionResult LineOfBusiness()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return View();
        }
    }
}
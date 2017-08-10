using System.Web.Mvc;

namespace FileBrowser.Controllers
{
    public class DefaultController : Controller
    {
        /***************************************************************
         * Index
         * author:  john greaux
         * date:    05/17/2017
         * desc:    Default controller action for loading the File Browser
         *          view. 
         **************************************************************/
        public ActionResult Index()
        {
            return View();
        }
    }
}
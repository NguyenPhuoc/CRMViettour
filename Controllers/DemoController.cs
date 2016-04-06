using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    public class DemoController : Controller
    {
        //
        // GET: /Demo/
        /// <summary>
        /// //////
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadFileOnline()
        {
            return View();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    public class MissionManageController : Controller
    {
        //
        // GET: /MissionManage/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TypeTask()
        {
            return View();
        }

        public ActionResult StatusTask()
        {
            return View();
        }

        public ActionResult PriorityTask()
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    public class AppointmentListController : Controller
    {
        //
        // GET: /AppointmentList/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AppointmentType()
        {
            return View();
        }

    }
}

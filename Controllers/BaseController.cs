using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    public class BaseController : Controller
    {
        private IBaseRepository baseRepository;
        public BaseController(IBaseRepository baseRepository)
        {
            this.baseRepository = baseRepository;
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            return base.BeginExecuteCore(callback, state);
        }
    }
}
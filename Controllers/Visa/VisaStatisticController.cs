using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Visa
{
    public class VisaStatisticController : BaseController
    {
        // GET: VisaStatistic

        #region Init

        private IGenericRepository<tbl_VisaInfomation> _visaInfomationRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;

        private DataContext _db;

        public VisaStatisticController(IGenericRepository<tbl_VisaInfomation> visaInfomationRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._visaInfomationRepository = visaInfomationRepository;
            this._tagsRepository = tagsRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

    }
}
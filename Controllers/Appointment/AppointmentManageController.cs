using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Appointment
{
    public class AppointmentManageController : BaseController
    {
        // GET: AppointmentManage
        #region Init

        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;

        private DataContext _db;

        public AppointmentManageController(IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._staffRepository = staffRepository;
            this._customerRepository = customerRepository;
            this._programRepository = programRepository;
            this._taskRepository = taskRepository;
            this._partnerRepository = partnerRepository;
            this._dictionaryRepository = dictionaryRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_AppointmentList()
        {
            var model = _appointmentHistoryRepository.GetAllAsQueryable()
                .Select(p => new tbl_AppointmentHistory
                {
                    Time = p.Time,
                    
                }).ToList();
            return PartialView("_Partial_AppointmentList", model);
        }

        #region Create
        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion
    }
}
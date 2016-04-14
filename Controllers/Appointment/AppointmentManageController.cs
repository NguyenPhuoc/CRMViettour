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
        private IGenericRepository<tbl_Tour> _tourRepository;

        private DataContext _db;

        public AppointmentManageController(IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tour> tourRepository,
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
            this._tourRepository = tourRepository;
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
                    tbl_Customer = _customerRepository.FindId(p.CustomerId),
                    tbl_Program = _programRepository.FindId(p.ProgramId),
                    tbl_Task = _taskRepository.FindId(p.TaskId),
                    tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                    tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                    tbl_Tour = _tourRepository.FindId(p.TourId),
                    Note = p.Note,
                    OtherStaff = p.OtherStaff,
                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                    tbl_Staff = _staffRepository.FindId(p.StatusId),
                    CreatedDate = p.CreatedDate
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
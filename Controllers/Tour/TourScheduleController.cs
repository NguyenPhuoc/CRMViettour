using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Tour
{
    public class TourScheduleController : BaseController
    {
        // GET: TourSchedule
        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_ReviewTourDetail> _reviewTourDetailRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_TourGuide> _tourGuideRepository;
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_TourOption> _tourOptionRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public TourScheduleController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_ReviewTourDetail> reviewTourDetailRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_TourGuide> tourGuideRepository,
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_TourOption> tourOptionRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._dictionaryRepository = dictionaryRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tourRepository = tourRepository;
            this._reviewTourRepository = reviewTourRepository;
            this._reviewTourDetailRepository = reviewTourDetailRepository;
            this._customerRepository = customerRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._tagsRepository = tagsRepository;
            this._taskRepository = taskRepository;
            this._documentFileRepository = documentFileRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._contractRepository = contractRepository;
            this._partnerRepository = partnerRepository;
            this._tourGuideRepository = tourGuideRepository;
            this._tourScheduleRepository = tourScheduleRepository;
            this._tourCustomerRepository = tourCustomerRepository;
            this._tourCustomerVisaRepository = tourCustomerVisaRepository;
            this._tourOptionRepository = tourOptionRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }
        #endregion
        public ActionResult Index()
        {
            return View();
        }

        #region JsonCalendar
        [HttpPost]
        public JsonResult JsonCalendar(int id)
        {
            if (id == -1)
            {
                var _model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
               .Select(p => new tbl_TourSchedule
               {
                   Id = p.Id,
                   Date = p.Date,
                   Place = p.Place != null ? p.Place : "",
                   StartTime = p.StartTime,
                   EndTime = p.EndTime

               }).ToList();

                var _eventList = from e in _model
                                 select new
                                 {
                                     id = e.Id,
                                     title = e.Place,
                                     start = e.Date.ToString("yyyy-MM-dd") + "T" + e.StartTime,
                                     end = e.Date.ToString("yyyy-MM-dd") + "T" + e.EndTime,
                                     constraint = e.Id,
                                 };
                var _rows = _eventList.ToArray();
                return Json(_rows, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["idTour"] = id;
                var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == id).Where(p => p.IsDelete == false)
                   .Select(p => new tbl_TourSchedule
                   {
                       Id = p.Id,
                       Date = p.Date,
                       Place = p.Place != null ? p.Place : "",
                       StartTime = p.StartTime,
                       EndTime = p.EndTime

                   }).ToList();

                var eventList = from e in model
                                select new
                                {
                                    id = e.Id,
                                    title = e.Place,
                                    start = e.Date.ToString("yyyy-MM-dd") + "T" + e.StartTime,
                                    end = e.Date.ToString("yyyy-MM-dd") + "T" + e.EndTime,
                                    constraint = e.Id,

                                };
                var rows = eventList.ToArray();
                return Json(rows, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult JsonCalendarDefaul()
        {
            var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
               .Select(p => new tbl_TourSchedule
               {
                   Id = p.Id,
                   Date = p.Date,
                   Place = p.Place != null ? p.Place : "",
                   StartTime = p.StartTime,
                   EndTime = p.EndTime

               }).ToList();

            var eventList = from e in model
                            select new
                            {
                                id = e.Id,
                                title = e.Place,
                                start = e.Date.ToString("yyyy-MM-dd") + "T" + e.StartTime,
                                end = e.Date.ToString("yyyy-MM-dd") + "T" + e.EndTime,
                                constraint = e.Id,

                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Tạo lịch đi tour

        [ValidateInput(false)]
        public async Task<ActionResult> CreateScheduleTour(tbl_TourSchedule model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.TourId = Convert.ToInt32(Session["idTour"].ToString());
                model.StaffId = clsPermission.GetUser().StaffID;
                await _tourScheduleRepository.Create(model);
                //Response.Write("<script>alert('Đã lưu');</script>");
            }
            catch { }
            ////------------------------------
            var models = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == model.TourId).Where(p => p.IsDelete == false)
               .Select(p => new tbl_TourSchedule
               {
                   Id = p.Id,
                   Date = p.Date,
                   Place = p.Place != null ? p.Place : "",
                   StartTime = p.StartTime,
                   EndTime = p.EndTime

               }).ToList();

            var eventList = from e in models
                            select new
                            {
                                id = e.Id,
                                title = e.Place,
                                start = e.Date.ToString("yyyy-MM-dd") + "T" + e.StartTime,
                                end = e.Date.ToString("yyyy-MM-dd") + "T" + e.EndTime,
                                constraint = e.Id,
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region UpdateScheduleTour
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditScheduleTour(int id)
        {
            var model = await _tourScheduleRepository.GetById(id);
            return PartialView("_Partial_EditScheduleTour", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateScheduleTour(tbl_TourSchedule model, FormCollection form)
        {
            try
            {
                await _tourScheduleRepository.Update(model);
                var models = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == model.TourId).Where(p => p.IsDelete == false)
                   .Select(p => new tbl_TourSchedule
                   {
                       Id = p.Id,
                       Date = p.Date,
                       Place = p.Place != null ? p.Place : "",
                       StartTime = p.StartTime,
                       EndTime = p.EndTime

                   }).ToList();

                var eventList = from e in models
                                select new
                                {
                                    id = e.Id,
                                    title = e.Place,
                                    start = e.Date.ToString("yyyy-MM-dd") + "T" + e.StartTime,
                                    end = e.Date.ToString("yyyy-MM-dd") + "T" + e.EndTime,
                                    constraint = e.Id,
                                };
                var rows = eventList.ToArray();

                return Json(rows, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return PartialView("_Partial_AppointmentList");
            }
        }
        #endregion

        #region _Partial_TourScheduleList
        [ChildActionOnly]
        public ActionResult _Partial_TourScheduleList()
        {
            var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false).ToList();

            return PartialView("_Partial_TourScheduleList", model);
        }
        #endregion


        public ActionResult TourScheduleFilter(int id)
        {
            if (id == -1)
            {
                var _model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false).ToList();

                return PartialView("_Partial_TourScheduleList", _model);
            }
            else
            {
                var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == id).Where(p => p.IsDelete == false).ToList();

            return PartialView("_Partial_TourScheduleList", model);
            }
        }
    }
}
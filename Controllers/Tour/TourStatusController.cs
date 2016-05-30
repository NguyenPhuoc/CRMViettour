using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Tour
{
    [Authorize]
    public class TourStatusController : BaseController
    {
        // GET: TourStatus

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

        public TourStatusController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
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

        public ActionResult _Partial_TourStatus()
        {
            var model = _tourRepository.GetAllAsQueryable().Where(p => p.IsDelete == false)
                .Select(p => new TourListViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    CustomerName = p.tbl_Customer.FullName,
                    NumberCustomer = p.NumberCustomer ?? 0,
                    DestinationPlace = p.tbl_TagsDestinationPlace.Tag,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    NumberDay = p.NumberDay ?? 0,
                    TourGuide = p.tbl_TourGuide.FirstOrDefault() == null ? "" : p.tbl_TourGuide.FirstOrDefault().tbl_Staff.FullName,
                    TourType = p.tbl_DictionaryTypeTour.Name
                }).ToList();
            return PartialView("_Partial_TourStatus", model);
        }

        public ActionResult FilterTour(string key)
        {
            var model = _tourRepository.GetAllAsQueryable().AsEnumerable();
            switch (key)
            {
                case "dangthuchien":
                    model = model.AsEnumerable().Where(p => p.StartDate <= DateTime.Now && DateTime.Now <= p.EndDate);
                    break;
                case "hoantat":
                    model = model.AsEnumerable().Where(p => p.EndDate < DateTime.Now);
                    break;
                case "sapkhoihanh":
                    model = model.AsEnumerable().Where(p => p.StartDate > DateTime.Now);
                    break;
                case "tatca":
                    model = model.AsEnumerable();
                    break;
            }
            var items = model.AsEnumerable().Where(p => p.IsDelete == false).Select(p => new TourListViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    CustomerName = p.CustomerId == null ? "" : p.tbl_Customer.FullName,
                    NumberCustomer = p.NumberCustomer ?? 0,
                    DestinationPlace = p.DestinationPlace == null ? "" : p.tbl_TagsDestinationPlace.Tag,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    NumberDay = p.NumberDay ?? 0,
                    TourGuide = p.tbl_TourGuide.FirstOrDefault() == null ? "" : p.tbl_TourGuide.FirstOrDefault().tbl_Staff.FullName,
                    TourType = p.tbl_DictionaryTypeTour.Name
                }).ToList();

            return PartialView("_Partial_TourStatus", items);
        }
    }
}
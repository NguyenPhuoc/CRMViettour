using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRMViettour.Models;
using System.Threading.Tasks;

namespace CRMViettour.Controllers
{
    public class TourManageController : BaseController
    {
        //
        // GET: /TourManage/

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
        private DataContext _db;

        public TourManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
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
            _db = new DataContext();
        }

        #endregion

        #region List

        [HttpPost]
        public ActionResult GetIdTour(int id)
        {
            Session["idTour"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Partial_ListTours()
        {
            var model = _tourRepository.GetAllAsQueryable()
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
            return PartialView("_Partial_ListTours", model);
        }

        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(TourViewModel model, FormCollection form)
        {
            try
            {
                model.SingleTour.CreatedDate = DateTime.Now;
                model.SingleTour.ModifiedDate = DateTime.Now;
                model.SingleTour.Permission = form["SingleTour.Permission"] != null ? form["SingleTour.Permission"].ToString() : null;
                if (model.StartDateTour != null && model.StartDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.StartDate = model.StartDateTour;
                }
                if (model.EndDateTour != null && model.EndDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.EndDate = model.EndDateTour;
                }
                model.SingleTour.CreateStaffId = 9;
                model.SingleTour.StatusId = 1145;
                if (await _tourRepository.Create(model.SingleTour))
                {
                    model.SingleTourGuide.CreateDate = DateTime.Now;
                    model.SingleTourGuide.TourId = model.SingleTour.Id;
                    if (model.StartDateTourGuide != null && model.StartDateTour.Value.Year >= 1980)
                    {
                        model.SingleTourGuide.StartDate = model.StartDateTourGuide;
                    }
                    if (model.EndDateTourGuide != null && model.EndDateTourGuide.Value.Year >= 1980)
                    {
                        model.SingleTourGuide.EndDate = model.EndDateTourGuide;
                    }
                    await _tourGuideRepository.Create(model.SingleTourGuide);
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update

        public async Task<ActionResult> TourInfomation(int id)
        {
            var singleTour = await _tourRepository.GetById(id);
            var singleTourGuide = _tourGuideRepository.GetAllAsQueryable().FirstOrDefault(p => p.TourId == id);
            var model = new TourViewModel
            {
                EndDateTour = singleTour.EndDate,
                SingleTour = singleTour,
                StartDateTour = singleTour.StartDate,
                SingleTourGuide = singleTourGuide,
                StartDateTourGuide = singleTourGuide != null ? singleTourGuide.StartDate : null,
                EndDateTourGuide = singleTourGuide != null ? singleTourGuide.EndDate : null
            };

            return PartialView("_Partial_EditTour", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(TourViewModel model, FormCollection form)
        {
            try
            {
                model.SingleTour.ModifiedDate = DateTime.Now;
                model.SingleTour.Permission = form["SingleTour.Permission"] != null ? form["SingleTour.Permission"].ToString() : null;
                if (model.StartDateTour != null && model.StartDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.StartDate = model.StartDateTour;
                }
                if (model.EndDateTour != null && model.EndDateTour.Value.Year >= 1980)
                {
                    model.SingleTour.EndDate = model.EndDateTour;
                }
                model.SingleTour.CreateStaffId = 9;

                if (await _tourRepository.Update(model.SingleTour))
                {
                    if (model.StartDateTourGuide != null && model.StartDateTourGuide.Value.Year >= 1980)
                    {
                        model.SingleTourGuide.StartDate = model.StartDateTourGuide;
                    }
                    if (model.EndDateTourGuide != null && model.EndDateTourGuide.Value.Year >= 1980)
                    {
                        model.SingleTourGuide.EndDate = model.EndDateTourGuide;
                    }

                    if (model.SingleTourGuide.Id == 0)
                    {
                        model.SingleTourGuide.CreateDate = DateTime.Now;
                        model.SingleTourGuide.TourId = model.SingleTour.Id;
                        await _tourGuideRepository.Create(model.SingleTourGuide);
                    }
                    else
                    {
                        await _tourGuideRepository.Update(model.SingleTourGuide);
                    }
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        if (await _customerRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TourManage") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Filter Type
        public ActionResult FilterTour(int id)
        {
            if (id == 9999)
            {
                var model = _tourRepository.GetAllAsQueryable().Where(p => p.CustomerId == null)
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

                return PartialView("_Partial_ListTours", model);
            }
            else
            {
                var model = _tourRepository.GetAllAsQueryable().Where(p => p.TypeTourId == id)
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

                return PartialView("_Partial_ListTours", model);
            }
        }
        #endregion
    }
}

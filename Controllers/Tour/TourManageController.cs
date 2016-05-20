using CRM.Core;
using CRMViettour.Utilities;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRMViettour.Models;
using System.Threading.Tasks;
using System.Globalization;
using CRM.Enum;
using OfficeOpenXml;

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
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_TourOption> _tourOptionRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_Company> _companyRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
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
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_TourOption> tourOptionRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_Company> companyRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
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
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._companyRepository = companyRepository;
            this._customerContactRepository = customerContactRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._updateHistoryRepository = updateHistoryRepository;
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

        int SDBID = 6;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsDelete = list.Contains(2);

            var ltAccess = _db.tbl_AccessData.Where(p => p.ShowDataById == PermissionsId & p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;
        }

        public ActionResult _Partial_ListTours()
        {
            // phân quyền
            Permission(1, 24);

            //Luu khi dang nhap

            if (SDBID == 6)
                return PartialView("_Partial_ListTours", new List<TourListViewModel>());

            try
            {
                int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
                switch (SDBID)
                {
                    case 2: maPB = clsPermission.GetUser().DepartmentID;
                        maCN = clsPermission.GetUser().BranchID;
                        break;
                    case 3: maNKD = clsPermission.GetUser().GroupID;
                        maCN = clsPermission.GetUser().BranchID; break;
                    case 4: maNV = clsPermission.GetUser().StaffID; break;
                    case 5: maCN = clsPermission.GetUser().BranchID; break;
                }

                var model = _tourRepository.GetAllAsQueryable().Where(p => (p.CreateStaffId == maNV | maNV == 0)
                    & (p.tbl_StaffCreate.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_StaffCreate.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_StaffCreate.HeadquarterId == maCN | maCN == 0))
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

                foreach (var item in model)
                {
                    item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id).Sum(c => c.ServicePrice) ?? 0;
                    item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id).Sum(c => c.TotalContract) ?? 0;
                }
                return PartialView("_Partial_ListTours", model);
            }
            catch
            { }

            return PartialView("_Partial_ListTours");
            /*var model = _tourRepository.GetAllAsQueryable()
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
                    TourType = p.tbl_DictionaryTypeTour.Name,
                    //CongNoKhachHang =  _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == p.Id).Sum(c => c.TotalContract) ?? 0,
                    //CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == p.Id).Sum(c => c.ServicePrice) ?? 0
                }).ToList();
            foreach (var item in model)
            {
                item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id).Sum(c => c.ServicePrice) ?? 0;
                item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id).Sum(c => c.TotalContract) ?? 0;
            }
            return PartialView("_Partial_ListTours", model);*/
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
                model.SingleTour.IsBidding = false;
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
                    var lichhen = new tbl_AppointmentHistory()
                    {
                        TourId = model.SingleTour.Id,
                        Time = model.SingleTourGuide.StartDate ?? DateTime.Now,
                        StaffId = 9,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        DictionaryId = 1213,
                        StatusId = 1145,
                        OtherStaff = model.SingleTourGuide.StaffId.ToString(),
                        Title = model.SingleTour.Name
                    };
                    await _appointmentHistoryRepository.Create(lichhen);
                    var lichditour = new tbl_TourSchedule()
                    {
                        TourId = model.SingleTour.Id,
                        TourGuideId = model.SingleTourGuide.StaffId,
                        StaffId = 9,
                        CreatedDate = DateTime.Now,
                        Date = model.SingleTour.StartDate ?? DateTime.Now,
                    };
                    int idtag = model.SingleTour.DestinationPlace ?? 0;
                    lichditour.Place = LoadData.DropdownlistLocation().Where(c => c.Id == idtag).Select(c => c.Tags).SingleOrDefault();
                    lichditour.Address = lichditour.Place;
                    await _tourScheduleRepository.Create(lichditour);
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
                        if (await _tourRepository.DeleteMany(listIds, false))
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
                var model = _tourRepository.GetAllAsQueryable().Where(p => p.IsBidding == true)
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

        #region Tạo lịch đi tour

        [ValidateInput(false)]
        public async Task<ActionResult> CreateScheduleTour(tbl_TourSchedule model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.TourId = Convert.ToInt32(Session["idTour"].ToString());
                model.StaffId = 9;
                await _tourScheduleRepository.Create(model);
                //Response.Write("<script>alert('Đã lưu');</script>");
            }
            catch { }

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Nhiệm vụ

        public JsonResult LoadPermission(int id)
        {
            var model = new SelectList(_staffRepository.GetAllAsQueryable().Where(p => p.DepartmentId == id).ToList(), "Id", "FullName");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public async Task<ActionResult> CreateTaskTour(tbl_Task model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.TaskStatusId = 1193;
                model.TourId = Convert.ToInt32(Session["idTour"].ToString());
                model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                model.IsNotify = false;
                model.StaffId = 9;
                await _taskRepository.Create(model);
                Response.Write("<script>alert('Đã lưu');</script>");
            }
            catch { }

            var list = _taskRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId)
                            .Select(p => new tbl_Task
                            {
                                Id = p.Id,
                                tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                                Name = p.Name,
                                Permission = p.Permission,
                                StartDate = p.StartDate,
                                EndDate = p.EndDate,
                                Time = p.Time,
                                TimeType = p.TimeType,
                                FinishDate = p.FinishDate,
                                PercentFinish = p.PercentFinish,
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml", list);
        }
        #endregion

        #region cập nhật loại tour

        public ActionResult UpdateTypeTour()
        {
            try
            {
                int tourId = Convert.ToInt32(Session["idTour"].ToString());
                var item = _tourRepository.FindId(tourId);
                item.IsBidding = false;
                _db.SaveChanges();
            }
            catch { }

            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Import Customer
        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {
                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<tbl_Customer> list = new List<tbl_Customer>();
                    List<tbl_CustomerVisa> listVisa = new List<tbl_CustomerVisa>();
                    int i = 0;
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    if (worksheet.Cells["B2"].Text == "1")//Cá nhân
                        for (int row = 2; row <= lastRow; row++)
                        {
                            if (worksheet.Cells["c" + row].Value == null || worksheet.Cells["c" + row].Text == "")
                                continue;
                            var cus = new tbl_Customer
                            {
                                CustomerType = CustomerType.Personal,
                                FullName = worksheet.Cells["d" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                                PersonalEmail = worksheet.Cells["f" + row].Value != null ? worksheet.Cells["f" + row].Text : null,
                                Phone = worksheet.Cells["g" + row].Value != null ? worksheet.Cells["g" + row].Text : null,
                                Address = worksheet.Cells["h" + row].Value != null ? worksheet.Cells["h" + row].Text : null,
                                AccountNumber = worksheet.Cells["o" + row].Value != null ? worksheet.Cells["o" + row].Text : null,
                                Bank = worksheet.Cells["p" + row].Value != null ? worksheet.Cells["p" + row].Text : null,
                                IdentityCard = worksheet.Cells["q" + row].Value != null ? worksheet.Cells["q" + row].Text : null,
                                PassportCard = worksheet.Cells["t" + row].Value != null ? worksheet.Cells["t" + row].Text : null,
                                Position = worksheet.Cells["l" + row].Value != null ? worksheet.Cells["l" + row].Text : null,
                                Department = worksheet.Cells["m" + row].Value != null ? worksheet.Cells["m" + row].Text : null,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                IsTemp = true,
                            };
                            string cel = "";
                            try//ngay sinh
                            {
                                cel = "e";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.Birthday = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay cấp cmnd
                            {
                                cel = "r";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.CreatedDateIdentity = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay hiệu lực passport
                            {
                                cel = "u";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.CreatedDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay hết hạn passport
                            {
                                cel = "v";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.ExpiredDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//danh xưng
                            {
                                cel = "c";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                    {
                                        string danhsung = worksheet.Cells[cel + row].Text;
                                        cus.NameTypeId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == danhsung && c.DictionaryCategoryId == 7).Select(c => c.Id).SingleOrDefault();
                                    }
                                }
                            }
                            catch { }
                            try//tagid dia chi
                            {
                                cel = "i";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string tinhtp = worksheet.Cells[cel + row].Text;
                                    cus.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tinhtp && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault().ToString();
                                }
                                cel = "j";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string quanhuyen = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == quanhuyen && c.TypeTag == 6).SingleOrDefault();
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                                cel = "k";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string phuongxa = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == phuongxa && c.TypeTag == 7).SingleOrDefault();
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                            }
                            catch { }
                            try//ngành nghề
                            {
                                cel = "n";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string nganhnghe = worksheet.Cells[cel + row].Text;
                                    cus.CareerId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == nganhnghe && c.DictionaryCategoryId == 2).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try//noi cap cmnd
                            {
                                cel = "s";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string noicap = worksheet.Cells[cel + row].Text;
                                    cus.IdentityTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noicap && c.TypeTag == 3).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try//noi cap passport
                            {
                                cel = "w";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string noicap = worksheet.Cells[cel + row].Text;
                                    cus.PassportTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noicap && c.TypeTag == 3).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try
                            {
                                cel = "x";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    var visa = new tbl_CustomerVisa
                                    {
                                        CustomerId = i,
                                        VisaNumber = worksheet.Cells[cel + row].Value != null ? worksheet.Cells[cel + row].Text : "",
                                        Deadline = Int16.Parse(worksheet.Cells["ab" + row].Value != null ? worksheet.Cells["ab" + row].Text : "0"),
                                        CreatedDate = DateTime.Now,
                                        ModifiedDate = DateTime.Now,
                                        CreatedDateVisa = worksheet.Cells["z" + row].Value != null && worksheet.Cells["z" + row].Text != "" ? DateTime.ParseExact(worksheet.Cells["z" + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture) : new DateTime(),
                                        ExpiredDateVisa = worksheet.Cells["aa" + row].Value != null && worksheet.Cells["aa" + row].Text != "" ? DateTime.ParseExact(worksheet.Cells["aa" + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture) : new DateTime(),
                                    };
                                    try//trang thái visa
                                    {
                                        cel = "ac";
                                        if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                        {
                                            string trangthai = worksheet.Cells[cel + row].Text;
                                            visa.DictionaryId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == trangthai && c.DictionaryCategoryId == 14).Select(c => c.Id).SingleOrDefault();
                                        }
                                    }
                                    catch { }
                                    try//loại visa
                                    {
                                        cel = "ad";
                                        if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                        {
                                            string loai = worksheet.Cells[cel + row].Text;
                                            visa.VisaType = (CRM.Enum.VisaType)Enum.Parse(typeof(CRM.Enum.VisaType), loai);
                                        }
                                    }
                                    catch { }
                                    try//tag id
                                    {
                                        cel = "y";
                                        if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                        {
                                            string tag = worksheet.Cells[cel + row].Text;
                                            visa.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tag && c.TypeTag == 3).Select(c => c.Id).SingleOrDefault();
                                        }
                                    }
                                    catch { }
                                    listVisa.Add(visa);
                                }
                                else
                                {
                                    listVisa.Add(new tbl_CustomerVisa());
                                }
                            }
                            catch { }
                            list.Add(cus);
                            i++;
                        }
                    Session["listCustomerImportTour"] = list;
                    Session["listCustomerVisaImportTour"] = listVisa;
                    return PartialView("_Partial_ImportDataList", list);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpPost]
        public async Task<ActionResult> SaveImport()
        {
            try
            {
                int idtour = Int32.Parse(Session["idTour"].ToString());
                List<tbl_Customer> list = Session["listCustomerImportTour"] as List<tbl_Customer>;
                List<tbl_CustomerVisa> listVisa = Session["listCustomerVisaImportTour"] as List<tbl_CustomerVisa>;
                int i = 0, ii = 0;

                foreach (var item in list)
                {
                    if (item.CustomerType == CustomerType.Personal)
                    {
                    randomCode: item.Code = new Random().Next(100, 999).ToString(); ;
                        if (!await _customerRepository.Create(item))
                            goto randomCode;
                        else
                        {
                            var tcus = new tbl_TourCustomer()
                            {
                                TourId = idtour,
                                CustomerId = item.Id
                            };
                            await _tourCustomerRepository.Create(tcus);
                            i++;
                            if (listVisa[ii].VisaNumber != null)
                            {
                                var visa = listVisa[ii];
                                visa.CustomerId = item.Id;
                                await _customerVisaRepository.Create(visa);
                                var tourvisa = new tbl_TourCustomerVisa
                                {
                                    CustomerId = visa.Id,
                                    TourId = idtour
                                };
                                await _tourCustomerVisaRepository.Create(tourvisa);
                            }
                        }
                    }
                    ii++;
                }
                Session["listCustomerImportTour"] = null;
                Session["listCustomerVisaImportTour"] = null;
                if (i != 0)
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Đã import thành công " + i + " dòng dữ liệu !", IsPartialView = false, RedirectTo = Url.Action("Index", "TourManage") }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Chưa có dữ liệu nào được import !" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                Session["listCustomerImportTour"] = null;
                Session["listCustomerVisaImportTour"] = null;
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Import dữ liệu lỗi !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteImport(String listItemId)
        {
            try
            {
                List<tbl_Customer> list = Session["listCustomerImportTour"] as List<tbl_Customer>;
                List<tbl_CustomerVisa> listVisa = Session["listCustomerVisaImportTour"] as List<tbl_CustomerVisa>;
                if (listItemId != null && listItemId != "")
                {
                    var listIds = listItemId.Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        int[] listIdsint = new int[listIds.Length];
                        for (int i = 0; i < listIds.Length; i++)
                        {
                            listIdsint[i] = Int32.Parse(listIds[i]);
                        }
                        for (int i = 0; i < listIdsint.Length; i++)
                        {
                            for (int j = i; j < listIdsint.Length; j++)
                            {
                                if (listIdsint[i] < listIdsint[j])
                                {
                                    int temp = listIdsint[i];
                                    listIdsint[i] = listIdsint[j];
                                    listIdsint[j] = temp;
                                }
                            }
                        }
                        foreach (var item in listIdsint)
                        {
                            listVisa.RemoveAt(item);
                            list.RemoveAt(item);
                        }
                    }
                }
                Session["listCustomerImportTour"] = list;
                return PartialView("_Partial_ImportDataList", list);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Customer in tour
        [HttpPost]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            try
            {
                int idtour = Int16.Parse(Session["idTour"].ToString());
                var ctu = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == idtour && c.CustomerId == id).Single();
                var history = _db.tbl_UpdateHistory.AsEnumerable().Where(c => c.CustomerId == id).ToList();
                foreach (var item in history)
                {
                    await _updateHistoryRepository.Delete(item.Id, false);
                }
                await _tourCustomerRepository.Delete(ctu.Id, true);

                if (await _customerRepository.Delete(id, true))
                {
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TourManage") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #region Update
        /// <summary>
        /// partial view edit thông tin khách hàng
        /// </summary>
        /// <returns></returns>
        /// 
        //[ChildActionOnly]
        //public ActionResult _Partial_EditCustomer()
        //{
        //    return PartialView("_Partial_EditCustomer", new CustomerViewModel());
        //}

        /// <summary>
        /// load thông tin khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustomerInfomation(int id)
        {
            var model = new CustomerViewModel();
            var customer = _customerRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            var customerVisa = _customerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.CustomerId == id).ToList();
            if (customer.CustomerType == 0) // doanh nghiep
            {
                model.SingleCompany = customer;
                model.CreatedDateIdentity = customer.CreatedDateIdentity ?? DateTime.Now;
                model.CreatedDatePassport = customer.CreatedDatePassport ?? DateTime.Now;
                model.ExpiredDatePassport = customer.ExpiredDatePassport ?? DateTime.Now;
                model.IdentityCard = customer.IdentityCard;
                model.IdentityTagId = customer.IdentityTagId ?? 0;
                model.PassportCard = customer.PassportCard;
                model.PassportTagId = customer.PassportTagId ?? 0;
            }
            else // ca nhan
            {
                model.SinglePersonal = customer;
                model.CreatedDateIdentity = customer.CreatedDateIdentity ?? DateTime.Now;
                model.CreatedDatePassport = customer.CreatedDatePassport ?? DateTime.Now;
                model.ExpiredDatePassport = customer.ExpiredDatePassport ?? DateTime.Now;
                model.IdentityCard = customer.IdentityCard;
                model.IdentityTagId = customer.IdentityTagId ?? 0;
                model.PassportCard = customer.PassportCard;
                model.PassportTagId = customer.PassportTagId ?? 0;
                model.IsTemp = customer.IsTemp;
            }
            var contact = _customerContactRepository.GetAllAsQueryable().FirstOrDefault(p => p.CustomerId == id);
            if (contact != null)
            {
                model.SingleContact = contact;
            }
            if (customerVisa.Count() > 0)
            {
                model.ListCustomerVisa = customerVisa;
            }
            return PartialView("_Partial_EditCustomer", model);
        }

        /// <summary>
        /// cập nhật khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateCustomer(CustomerViewModel model, FormCollection form)
        {
            try
            {
                if (model.SingleCompany.Id != 0)
                {
                    model.SingleCompany.CustomerType = CustomerType.Personal;
                    model.SingleCompany.TagsId = form["SingleCompany.TagsId"];
                    model.SingleCompany.ModifiedDate = DateTime.Now;
                    model.SingleCompany.IdentityCard = model.IdentityCard;
                    model.SingleCompany.IdentityTagId = model.IdentityTagId;
                    model.SingleCompany.ParentId = 0;
                    model.SingleCompany.PassportCard = model.PassportCard;
                    model.SingleCompany.PassportTagId = model.PassportTagId;
                    model.SingleCompany.NameTypeId = 47;
                    model.SingleCompany.FullName = _db.tbl_Company.Find(model.SingleCompany.CompanyId).Name;
                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SingleCompany.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SingleCompany.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SingleCompany.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerRepository.Update(model.SingleCompany))
                    {
                        UpdateHistory.SaveCustomer(model.SingleCompany.Id, 9, "Cập nhật khách hàng doanh nghiệp, code: " + model.SingleCompany.Code);

                        // xóa tất cả visa của customer
                        var visaList = _customerVisaRepository.GetAllAsQueryable().Where(p => p.CustomerId == model.SingleCompany.Id).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                await _customerVisaRepository.Delete(v.Id, false);
                            }
                        }

                        // add các visa mới
                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerId = model.SingleCompany.Id,
                                    DictionaryId = 1069
                                };
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }
                                await _customerVisaRepository.Create(visa);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                if (model.SinglePersonal.Id != 0)
                {
                    model.SinglePersonal.CustomerType = CustomerType.Personal;
                    model.SinglePersonal.TagsId = form["SinglePersonal.TagsId"];
                    model.SinglePersonal.ModifiedDate = DateTime.Now;
                    model.SinglePersonal.IdentityCard = model.IdentityCard;
                    model.SinglePersonal.IdentityTagId = model.IdentityTagId;
                    model.SinglePersonal.ParentId = 0;
                    model.SinglePersonal.PassportCard = model.PassportCard;
                    model.SinglePersonal.PassportTagId = model.PassportTagId;
                    model.SinglePersonal.IsTemp = model.IsTemp;
                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SinglePersonal.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SinglePersonal.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SinglePersonal.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerRepository.Update(model.SinglePersonal))
                    {
                        UpdateHistory.SaveCustomer(model.SinglePersonal.Id, 9, "Cập nhật khách hàng cá nhân, code: " + model.SinglePersonal.Code);

                        // xóa tất cả visa của customer
                        var visaList = _customerVisaRepository.GetAllAsQueryable().Where(p => p.CustomerId == model.SinglePersonal.Id).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                await _customerVisaRepository.Delete(v.Id, false);
                            }
                        }

                        // add các visa mới
                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerId = model.SinglePersonal.Id,
                                    DictionaryId = 1069
                                };
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }
                                await _customerVisaRepository.Create(visa);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                if (model.SingleContact.Id != 0)
                {
                    model.SingleContact.TagsId = form["SinglePersonal.TagsId"];
                    model.SingleContact.ModifiedDate = DateTime.Now;
                    model.SingleContact.CreatedDateIdentity = model.CreatedDateIdentity;
                    model.SingleContact.CreatedDatePassport = model.CreatedDatePassport;
                    model.SingleContact.ExpiredDatePassport = model.ExpiredDatePassport;
                    model.SingleContact.IdentityCard = model.IdentityCard;
                    model.SingleContact.IdentityTagId = model.IdentityTagId;
                    model.SingleContact.PassportCard = model.PassportCard;
                    model.SingleContact.PassportTagId = model.PassportTagId;
                    if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                    {
                        model.SingleContact.CreatedDateIdentity = model.CreatedDateIdentity;
                    }
                    if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                    {
                        model.SingleContact.CreatedDatePassport = model.CreatedDatePassport;
                    }
                    if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                    {
                        model.SingleContact.ExpiredDatePassport = model.ExpiredDatePassport;
                    }

                    if (await _customerContactRepository.Update(model.SingleContact))
                    {
                        UpdateHistory.SaveCustomer(model.SingleContact.Id, 9, "Cập nhật người liên hệ, code: " + model.SingleContact.Code);
                        // xóa tất cả visa của customer
                        var visaList = _customerContactVisaRepository.GetAllAsQueryable().Where(p => p.CustomerContactId == model.SingleContact.Id).ToList();
                        if (visaList.Count() > 0)
                        {
                            foreach (var v in visaList)
                            {
                                await _customerContactVisaRepository.Delete(v.Id, false);
                            }
                        }

                        // add các visa mới
                        for (int i = 1; i < 6; i++)
                        {
                            if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                            {
                                var visa = new tbl_CustomerContactVisa
                                {
                                    VisaNumber = form["VisaNumber" + i].ToString(),
                                    TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                    CustomerContactId = model.SingleContact.Id,
                                    DictionaryId = 1069
                                };
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    visa.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980)
                                {
                                    visa.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa" + i]);
                                }
                                if (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980 && Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980)
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }

                                await _customerContactVisaRepository.Create(visa);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }
        #endregion
        [HttpPost]
        public async Task<ActionResult> CapNhatKH(int id)
        {
            try
            {
                var tour = _tourRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Id == id).Single();
                var custour = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == id).Select(c => c.tbl_Customer).ToList();
                foreach (var item in custour)
                {
                    _db = new DataContext();
                    bool temp = false;
                    tbl_Customer cus = new tbl_Customer();

                    var namebirthday = _db.tbl_Customer.AsEnumerable().Where(c => c.FullName == item.FullName && c.Birthday == item.Birthday && c.IsTemp == false).FirstOrDefault();
                    if (namebirthday != null)
                    {
                        cus = namebirthday;
                        temp = true;
                    }

                    var cmnd = _db.tbl_Customer.AsEnumerable().Where(c => c.IdentityCard == item.IdentityCard && c.IsTemp == false).FirstOrDefault();
                    if (cmnd != null)
                    {
                        cus = cmnd;
                        temp = true;
                    }

                    var passport = _db.tbl_Customer.AsEnumerable().Where(c => c.PassportCard == item.PassportCard && c.IsTemp == false).FirstOrDefault();
                    if (passport != null)
                    {
                        cus = passport;
                        temp = true;
                    }

                    if (!temp)
                    {
                        try
                        {
                            var abs = _db.tbl_Customer.Where(c => c.Id == item.Id).Single();
                            abs.Code = LoadData.NewCodeCustomerPersonal();
                            abs.IsTemp = false;
                            _db.SaveChanges();
                        }
                        catch { }
                    }
                    else
                    {
                        cus = _db.tbl_Customer.AsEnumerable().Where(c => c.Id == cus.Id).Single();
                        cus.FullName = item.FullName;
                        cus.Birthday = item.Birthday;
                        cus.PersonalEmail = item.PersonalEmail;
                        cus.Phone = item.Phone;
                        cus.Address = item.Address;
                        cus.TagsId = item.TagsId;
                        cus.Position = item.Position;
                        cus.Department = item.Department;
                        cus.CareerId = item.CareerId;
                        cus.AccountNumber = item.AccountNumber;
                        cus.Bank = item.Bank;
                        cus.IdentityCard = item.IdentityCard;
                        cus.CreatedDateIdentity = item.CreatedDateIdentity;
                        cus.IdentityTagId = item.IdentityTagId;
                        cus.PassportCard = item.PassportCard;
                        cus.CreatedDatePassport = item.CreatedDatePassport;
                        cus.ExpiredDatePassport = item.ExpiredDatePassport;
                        cus.PassportTagId = item.PassportTagId;
                        cus.Note = item.Note;
                        var ctu = _tourCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == id && c.CustomerId == item.Id).SingleOrDefault();
                        ctu.CustomerId = cus.Id;
                        await _tourCustomerRepository.Update(ctu);
                        _db.SaveChanges();
                        var visas = _customerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.CustomerId == item.Id).ToList();
                        foreach (var vs in visas)
                        {
                            vs.CustomerId = cus.Id;
                            await _customerVisaRepository.Update(vs);
                        }
                        var history = _db.tbl_UpdateHistory.AsEnumerable().Where(c => c.CustomerId == item.Id).ToList();
                        foreach (var it in history)
                        {
                            await _updateHistoryRepository.Delete(it.Id, false);
                        }
                        await _customerRepository.Delete(item.Id, false);
                    }
                }
                var st = tour.StartDate ?? DateTime.Now;
                var en = tour.EndDate ?? DateTime.Now;
                TimeSpan totalDay = en - st;
                tour.NumberDay = Int32.Parse(totalDay.TotalDays.ToString());
                tour.NumberCustomer = _db.tbl_TourCustomer.AsEnumerable().Where(c => c.TourId == tour.Id).Count();
                tour.IsUpdate = true;
                await _tourRepository.Update(tour);
                return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Cập nhật dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TourManage") }, JsonRequestBehavior.AllowGet);


                //return RedirectToAction("Index");
            }
            catch
            {
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Cập nhật dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
        }
        #endregion
    }
}

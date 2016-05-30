using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRMViettour.Models;
using CRM.Core;
using CRM.Infrastructure;
using System.Threading.Tasks;
using System.Data.Entity;
using CRMViettour.Utilities;

namespace CRMViettour.Controllers.Customer
{
    [Authorize]
    public class CustomerTabInfoController : BaseController
    {
        // GET: CustomerTabInfo
        #region Init

        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_TourCustomer> _tourCustomerRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private DataContext _db;

        public CustomerTabInfoController(
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TourCustomer> tourCustomerRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._reviewTourRepository = reviewTourRepository;
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._tourCustomerRepository = tourCustomerRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            _db = new DataContext();
        }
        #endregion
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
        }
        #region Phản hồi khách hàng
        [ChildActionOnly]
        public ActionResult _PhanHoiKhachHang()
        {
            return PartialView("_PhanHoiKhachHang");
        }

        [HttpPost]
        public async Task<ActionResult> InfoPhanHoiKhachHang(int id)
        {
            var model = await _reviewTourRepository.GetAllAsQueryable().Where(p => p.CustomerId == id && p.IsDelete == false).ToListAsync();
            return PartialView("_PhanHoiKhachHang", model);
        }
        #endregion

        #region Người liên hệ
        [ChildActionOnly]
        public ActionResult _NguoiLienHe()
        {
            return PartialView("_NguoiLienHe");
        }

        [HttpPost]
        public async Task<ActionResult> InfoNguoiLienHe(int id)
        {
            var model = await _customerContactRepository.GetAllAsQueryable().Where(p => p.CustomerId == id && p.IsDelete == false).ToListAsync();
            return PartialView("_NguoiLienHe", model);
        }
        #endregion

        #region Cập nhật thay đổi
        [ChildActionOnly]
        public ActionResult _CapNhatThayDoi()
        {
            return PartialView("_CapNhatThayDoi");
        }

        [HttpPost]
        public async Task<ActionResult> InfoCapNhatThayDoi(int id)
        {
            var model = await _updateHistoryRepository.GetAllAsQueryable().Where(p => p.CustomerId == id && p.IsDelete == false).ToListAsync();
            return PartialView("_CapNhatThayDoi", model);
        }
        #endregion

        #region Email
        [ChildActionOnly]
        public ActionResult _Email()
        {
            return PartialView("_Email");
        }

        [HttpPost]
        public ActionResult InfoEmail()
        {
            return PartialView("_Email");
        }
        #endregion

        #region Hồ sơ liên quan
        [ChildActionOnly]
        public ActionResult _HoSoLienQuan()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_HoSoLienQuan");
        }

        [HttpPost]
        public async Task<ActionResult> InfoHoSoLienQuan(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 55);
            var model = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == id && p.IsDelete == false)
                     .Select(p => new tbl_DocumentFile
                     {
                         Id = p.Id,
                         FileName = p.FileName,
                         FileSize = p.FileSize,
                         Note = p.Note,
                         CreatedDate = p.CreatedDate,
                         TagsId = p.TagsId,
                         tbl_Staff = _staffRepository.FindId(p.StaffId)
                     }).ToList();
            return PartialView("_HoSoLienQuan", model);
        }
        #endregion

        #region Lịch hẹn
        [ChildActionOnly]
        public ActionResult _LichHen()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_LichHen");
        }

        [HttpPost]
        public ActionResult InfoLichHen(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 53);
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.CustomerId == id && p.IsDelete == false)
                .Select(p => new tbl_AppointmentHistory
                {
                    Id = p.Id,
                    Title = p.Title,
                    Time = p.Time,
                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                    Note = p.Note
                }).ToList();
            return PartialView("_LichHen", model);
        }
        #endregion

        #region Lịch sử liên hệ
        [ChildActionOnly]
        public ActionResult _LichSuLienHe()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_LichSuLienHe");
        }

        [HttpPost]
        public async Task<ActionResult> InfoLichSuLienHe(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 56);
            var model = _contactHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.CustomerId == id && p.IsDelete == false)
                       .Select(p => new tbl_ContactHistory
                       {
                           Id = p.Id,
                           ContactDate = p.ContactDate,
                           Request = p.Request,
                           Note = p.Note,
                           tbl_Staff = _staffRepository.FindId(p.StaffId),
                           tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId)
                       }).ToList();
            return PartialView("_LichSuLienHe", model);
        }
        #endregion

        #region SMS
        [ChildActionOnly]
        public ActionResult _SMS()
        {
            return PartialView("_SMS");
        }

        [HttpPost]
        public ActionResult InfoSMS()
        {
            return PartialView("_SMS");
        }
        #endregion

        #region Thông tin chi tiết
        [ChildActionOnly]
        public ActionResult _ThongTinChiTiet()
        {
            return PartialView("_ThongTinChiTiet");
        }

        [HttpPost]
        public async Task<ActionResult> InfoThongTinChiTiet(int id)
        {
            var model = await _customerRepository.GetById(id);
            return PartialView("_ThongTinChiTiet", model);
        }
        #endregion

        #region Tour tuyến
        [ChildActionOnly]
        public ActionResult _TourTuyen()
        {
            return PartialView("_TourTuyen");
        }

        [HttpPost]
        public async Task<ActionResult> InfoTourTuyen(int id)
        {
            var model = _tourCustomerRepository.GetAllAsQueryable().Where(c => c.CustomerId == id && c.IsDelete == false)
                .Select(p => new TourListViewModel
                {
                    Id = p.tbl_Tour.Id,
                    Code = p.tbl_Tour.Code,
                    Name = p.tbl_Tour.Name,
                    NumberCustomer = p.tbl_Tour.NumberCustomer ?? 0,
                    StartDate = p.tbl_Tour.StartDate,
                    EndDate = p.tbl_Tour.EndDate,
                    NumberDay = p.tbl_Tour.NumberDay ?? 0,
                    TourType = p.tbl_Tour.tbl_DictionaryTypeTour.Name,
                    Status = p.tbl_Tour.tbl_DictionaryStatus.Name,
                }).ToList();
            foreach (var item in model)
            {
                item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0;
                item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0;
            }

            return PartialView("_TourTuyen", model);
        }
        #endregion

        #region Visa
        [ChildActionOnly]
        public ActionResult _Visa()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_Visa");
        }

        [HttpPost]
        public async Task<ActionResult> InfoVisa(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 54);
            var model = await _customerVisaRepository.GetAllAsQueryable().Where(p => p.CustomerId == id && p.IsDelete == false).ToListAsync();
            return PartialView("_Visa", model);
        }
        #endregion
    }
}
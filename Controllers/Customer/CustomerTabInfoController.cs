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

namespace CRMViettour.Controllers.Customer
{
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
        private IGenericRepository<tbl_Company> _companyRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private DataContext _db;

        public CustomerTabInfoController(
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_Company> companyRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
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
            this._companyRepository = companyRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            _db = new DataContext();
        }
        #endregion

        #region Phản hồi khách hàng
        [ChildActionOnly]
        public ActionResult _PhanHoiKhachHang()
        {
            return PartialView("_PhanHoiKhachHang");
        }

        [HttpPost]
        public async Task<ActionResult> InfoPhanHoiKhachHang(int id)
        {
            var model = await _reviewTourRepository.GetAllAsQueryable().Where(p => p.CustomerId == id).ToListAsync();
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
            var model = await _customerContactRepository.GetAllAsQueryable().Where(p => p.CustomerId == id).ToListAsync();
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
            var model = await _updateHistoryRepository.GetAllAsQueryable().Where(p => p.CustomerId == id).ToListAsync();
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
            return PartialView("_HoSoLienQuan");
        }

        [HttpPost]
        public async Task<ActionResult> InfoHoSoLienQuan(int id)
        {
            //var model = await _documentFileRepository.GetAllAsQueryable().Where(p => p.CustomerId == id).ToListAsync();
            var model = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == id)
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
            return PartialView("_LichHen");
        }

        [HttpPost]
        public ActionResult InfoLichHen(int id)
        {
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.CustomerId == id)
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
            return PartialView("_LichSuLienHe");
        }

        [HttpPost]
        public async Task<ActionResult> InfoLichSuLienHe(int id)
        {
            // var model = await _contactHistoryRepository.GetAllAsQueryable().Where(p => p.CustomerId == id).ToListAsync();
            var model = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.CustomerId == id)
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
        public ActionResult InfoTourTuyen()
        {
            return PartialView("_TourTuyen");
        }
        #endregion

        #region Visa
        [ChildActionOnly]
        public ActionResult _Visa()
        {
            return PartialView("_Visa");
        }

        [HttpPost]
        public async Task<ActionResult> InfoVisa(int id)
        {
            var model = await _customerVisaRepository.GetAllAsQueryable().Where(p => p.CustomerId == id).ToListAsync();
            return PartialView("_Visa", model);
        }
        #endregion
    }
}
using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CRMViettour.Controllers.Partner
{
    public class PartnerTabInfoController : BaseController
    {
        // GET: PartnerTabInfo

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_PartnerNote> _partnerNoteRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerHistoryRepository;
        private DataContext _db;

        public PartnerTabInfoController(
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerHistoryRepository,
            IGenericRepository<tbl_PartnerNote> partnerNoteRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._partnerRepository = partnerRepository;
            this._tagsRepository = tagsRepository;
            this._partnerNoteRepository = partnerNoteRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._servicesPartnerHistoryRepository = servicesPartnerHistoryRepository;
            _db = new DataContext();
        }
        #endregion

        #region ThongTinChiTiet
        [ChildActionOnly]
        public ActionResult _ThongTinChiTiet()
        {
            return PartialView("_ThongTinChiTiet");
        }

        [HttpPost]
        public async Task<ActionResult> InfoThongTinChiTiet(int id)
        {
            var model = await _partnerRepository.GetById(id);
            return PartialView("_ThongTinChiTiet", model);
        }
        #endregion

        #region Lịch hẹn
        [ChildActionOnly]
        public ActionResult _LichHen()
        {
            return PartialView("_LichHen");
        }

        [HttpPost]
        public async Task<ActionResult> InfoLichHen(int id)
        {
            //var model = await _appointmentHistoryRepository.GetAllAsQueryable().Where(p => p.PartnerId == id).ToListAsync();
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.PartnerId == id)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Time = p.Time,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                OtherStaff = p.OtherStaff
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
            //var model = await _contactHistoryRepository.GetAllAsQueryable().Where(p => p.PartnerId == id).ToListAsync();
            var model = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.PartnerId == id)
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

        #region Thầu/tour
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

        #region Đánh giá
        [ChildActionOnly]
        public ActionResult _DanhGia()
        {
            return PartialView("_DanhGia");
        }

        [HttpPost]
        public ActionResult InfoDanhGia()
        {
            return PartialView("_DanhGia");
        }
        #endregion

        #region Dịch vụ cung cấp
        [ChildActionOnly]
        public ActionResult _DichVuCungCap()
        {
            return PartialView("_DichVuCungCap");
        }

        [HttpPost]
        public async Task<ActionResult> InfoDichVuCungCap(int id)
        {
            var model = await _servicesPartnerHistoryRepository.GetAllAsQueryable().Where(p => p.PartnerId == id).ToListAsync();
            return PartialView("_DichVuCungCap", model);
        }
        #endregion

        #region Ghi chú
        [ChildActionOnly]
        public ActionResult _GhiChu()
        {
            return PartialView("_GhiChu");
        }

        [HttpPost]
        public async Task<ActionResult> InfoGhiChu(int id)
        {
            var model = await _partnerNoteRepository.GetAllAsQueryable().Where(p => p.PartnerId == id).Select(p => new tbl_PartnerNote
            {
                Id = p.Id,
                Note = p.Note,
                tbl_Staff = _staffRepository.FindId(p.StaffId),
                CreatedDate = p.CreatedDate
            }).ToListAsync();
            return PartialView("_GhiChu", model);
        }
        #endregion

        #region Invoice
        [ChildActionOnly]
        public ActionResult _Invoice()
        {
            return PartialView("_Invoice");
        }

        [HttpPost]
        public async Task<ActionResult> InfoInvoice()
        {
            return PartialView("_Invoice");
        }
        #endregion

        #region Tài liệu mẫu
        [ChildActionOnly]
        public ActionResult _TaiLieuMau()
        {
            return PartialView("_TaiLieuMau");
        }

        [HttpPost]
        public async Task<ActionResult> InfoTaiLieuMau(int id)
        {
            var model = await _documentFileRepository.GetAllAsQueryable().Where(p => p.PartnerId == id).ToListAsync();
            return PartialView("_TaiLieuMau", model);
        }
        #endregion
    }
}
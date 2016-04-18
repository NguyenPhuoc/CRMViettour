using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CRMViettour.Controllers.Tour
{
    public class TourTabInfoController : BaseController
    {
        // GET: TourTabInfo

        #region init
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
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Quotation> _quotationRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private DataContext _db;

        public TourTabInfoController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
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
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Quotation> quotationRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
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
            this._programRepository = programRepository;
            this._quotationRepository = quotationRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            _db = new DataContext();
        }
        #endregion

        #region Chi tiết tour
        [ChildActionOnly]
        public ActionResult _ChiTietTour()
        {
            return PartialView("_ChiTietTour");
        }

        [HttpPost]
        public async Task<ActionResult> InfoChiTietTour(int id)
        {
            var model = await _tourRepository.GetById(id);
            return PartialView("_ChiTietTour", model);
        }
        #endregion

        #region Ds dịch vụ
        [ChildActionOnly]
        public ActionResult _DichVu()
        {
            return PartialView("_DichVu");
        }

        [HttpPost]
        public async Task<ActionResult> InfoDichVu(int id)
        {
            return PartialView("_DichVu");
        }
        #endregion

        #region Ds nhiệm vụ
        [ChildActionOnly]
        public ActionResult _NhiemVu()
        {
            return PartialView("_NhiemVu");
        }

        [HttpPost]
        public async Task<ActionResult> InfoNhiemVu(int id)
        {
            var model = await _taskRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_NhiemVu", model);
        }
        #endregion

        #region Khách hàng
        [ChildActionOnly]
        public ActionResult _KhachHang()
        {
            return PartialView("_KhachHang");
        }

        [HttpPost]
        public async Task<ActionResult> InfoKhachHang(int id)
        {

            return PartialView("_KhachHang");
        }
        #endregion

        #region Chương trình
        [ChildActionOnly]
        public ActionResult _ChuongTrinh()
        {
            return PartialView("_ChuongTrinh");
        }

        [HttpPost]
        public async Task<ActionResult> InfoChuongTrinh(int id)
        {
            var model = await _programRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_ChuongTrinh", model);
        }
        #endregion

        #region Hợp đồng
        [ChildActionOnly]
        public ActionResult _HopDong()
        {
            return PartialView("_HopDong");
        }

        [HttpPost]
        public async Task<ActionResult> InfoHopDong(int id)
        {
            var model = await _contractRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_HopDong", model);
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
            return PartialView("_Visa");
        }
        #endregion

        #region Viettour báo giá
        [ChildActionOnly]
        public ActionResult _ViettourBaoGia()
        {
            return PartialView("_ViettourBaoGia");
        }

        [HttpPost]
        public async Task<ActionResult> InfoViettourBaoGia(int id)
        {
            var model = await _quotationRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_ViettourBaoGia", model);
        }
        #endregion

        #region Công nợ khách hàng
        [ChildActionOnly]
        public ActionResult _CongNoKH()
        {
            return PartialView("_CongNoKH");
        }

        [HttpPost]
        public async Task<ActionResult> InfoCongNoKH(int id)
        {
            var model = await _liabilityCustomerRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_CongNoKH", model);
        }
        #endregion

        #region Công nợ đối tác
        [ChildActionOnly]
        public ActionResult _CongNoDoiTac()
        {
            return PartialView("_CongNoDoiTac");
        }

        [HttpPost]
        public async Task<ActionResult> InfoCongNoDoiTac(int id)
        {
            var model = await _liabilityPartnerRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_CongNoDoiTac", model);
        }
        #endregion

        #region Đánh giá
        [ChildActionOnly]
        public ActionResult _DanhGia()
        {
            return PartialView("_DanhGia");
        }

        [HttpPost]
        public async Task<ActionResult> InfoDanhGia(int id)
        {
            var model = await _reviewTourRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_DanhGia", model);
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
            var model = await _appointmentHistoryRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_LichHen", model);
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
            var model = await _documentFileRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_TaiLieuMau", model);
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
            var model = await _contactHistoryRepository.GetAllAsQueryable().Where(p => p.TourId == id).ToListAsync();
            return PartialView("_LichSuLienHe", model);
        }
        #endregion


    }
}
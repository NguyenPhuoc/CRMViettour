using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CRMViettour.Models;
using CRMViettour.Utilities;

namespace CRMViettour.Controllers.Contract
{
    [Authorize]
    public class ContractTabInfoController : BaseController
    {
        // GET: ContractTabInfo
        #region Init

        private IGenericRepository<tbl_Contract> _contractRepository;
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
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_DeadlineOption> _deadlineOptionRepository;
        private IGenericRepository<tbl_TourOption> _tourOptionRepository;
        private DataContext _db;

        public ContractTabInfoController(
            IGenericRepository<tbl_Contract> contractRepository,
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
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_DeadlineOption> deadlineOptionRepository,
            IGenericRepository<tbl_TourOption> tourOptionRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._contractRepository = contractRepository;
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
            this._tourRepository = tourRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._deadlineOptionRepository = deadlineOptionRepository;
            this._tourOptionRepository = tourOptionRepository;
            _db = new DataContext();
        }
        #endregion
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
        }
        #region ThongTinChiTiet
        [ChildActionOnly]
        public ActionResult _ThongTinChiTiet()
        {
            return PartialView("_ThongTinChiTiet");
        }

        [HttpPost]
        public async Task<ActionResult> InfoThongTinChiTiet(int id)
        {
            var model = await _contractRepository.GetById(id);
            return PartialView("_ThongTinChiTiet", model);
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
        public async Task<ActionResult> InfoLichHen(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 71);
            var model = _db.tbl_AppointmentHistory.AsEnumerable().Where(p => p.ContractId == id && p.IsDelete == false).Select(p => new tbl_AppointmentHistory
            {
                Id = p.Id,
                Time = p.Time,
                Title = p.Title,
                Note = p.Note,
                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.DictionaryId),
                tbl_Staff = _staffRepository.FindId(p.StaffId),
                OtherStaff = p.OtherStaff
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
            Permission(clsPermission.GetUser().PermissionID, 70);
            //var model = await _contactHistoryRepository.GetAllAsQueryable().Where(p => p.ContractId == id).ToListAsync();
            var model = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.ContractId == id && p.IsDelete == false)
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

        #region Chi tiết tour
        [ChildActionOnly]
        public ActionResult _ChiTietTour()
        {
            return PartialView("_ChiTietTour");
        }

        [HttpPost]
        public ActionResult InfoChiTietTour(int id)
        {
            var model = _contractRepository.FindId(id);
            var tour = model.tbl_Tour;
            if (tour != null)
            {
                var tourModel = new TourListViewModel()
                {
                    Id = tour.Id,
                    Code = tour.Code,
                    Name = tour.Name,
                    NumberDay = tour.NumberDay ?? 0,
                    NumberCustomer = tour.NumberCustomer ?? 0,
                    StartDate = tour.StartDate,
                    EndDate = tour.EndDate,
                    TourType = tour.tbl_DictionaryTypeTour.Name,
                    Status = tour.tbl_DictionaryStatus.Name,
                    CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == tour.Id && c.IsDelete == false).Sum(c => c.TotalContract) ?? 0,
                    CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == tour.Id && c.IsDelete == false).Sum(c => c.ServicePrice) ?? 0,


                };
                return PartialView("_ChiTietTour", tourModel);
            }
            return PartialView("_ChiTietTour", new TourListViewModel());
        }
        #endregion

        #region Tài liệu mẫu
        [ChildActionOnly]
        public ActionResult _TaiLieuMau()
        {
            ViewBag.IsAdd = false;
            ViewBag.IsDelete = false;
            ViewBag.IsEdit = false;
            return PartialView("_TaiLieuMau");
        }

        [HttpPost]
        public async Task<ActionResult> InfoTaiLieuMau(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 69);
            // var model = await _documentFileRepository.GetAllAsQueryable().Where(p => p.ContractId == id).ToListAsync();
            var model = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId == id && p.IsDelete == false)
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
            return PartialView("_TaiLieuMau", model);
        }
        #endregion

        #region Công nợ
        [ChildActionOnly]
        public ActionResult _CongNo()
        {
            return PartialView("_CongNo");
        }

        [HttpPost]
        public ActionResult InfoCongNo()
        {
            return PartialView("_CongNo");
        }
        #endregion

        #region Lịch sử invoice đối tác
        [ChildActionOnly]
        public ActionResult _LichSuInvoiceDoiTac()
        {
            return PartialView("_LichSuInvoiceDoiTac");
        }

        [HttpPost]
        public async Task<ActionResult> InfoLichSuInvoiceDoiTac(int id)
        {
            var tour = _contractRepository.FindId(id).tbl_Tour;
            if (tour != null)
            {
                var model = _db.tbl_TourOption.AsEnumerable().Where(c => c.TourId == tour.Id && c.DeadlineId != null).Where(p => p.IsDelete == false)
                                 .Select(c => new InvoiceListViewModel
                                 {
                                     Id = c.DeadlineId ?? 0,
                                     Partner = c.tbl_DeadlineOption.tbl_ServicesPartner.tbl_Partner.Name,
                                     Service = c.tbl_DeadlineOption.tbl_ServicesPartner.Name,
                                     Name = c.tbl_DeadlineOption.Name,
                                     Status = _dictionaryRepository.FindId(c.tbl_DeadlineOption.StatusId).Name,
                                     Note = c.tbl_DeadlineOption.Note,
                                     CodeTour = tour.Code,
                                     NameTour = tour.Name,
                                     NameStaff = c.tbl_DeadlineOption.tbl_Staff.FullName
                                 }).ToList();
                foreach (var item in model)
                {
                    var dt = _deadlineOptionRepository.FindId(item.Id).Time;
                    if (dt != null)
                        item.Date = dt ?? DateTime.Now;
                }
                return PartialView("_LichSuInvoiceDoiTac", model);
            }
            return PartialView("_LichSuInvoiceDoiTac");
        }
        #endregion

        #region Nhật ký xử lý
        [ChildActionOnly]
        public ActionResult _NhatKyXuLy()
        {
            return PartialView("_NhatKyXuLy");
        }

        [HttpPost]
        public ActionResult InfoNhatKyXuLy()
        {
            return PartialView("_NhatKyXuLy");
        }
        #endregion

        #region Phiếu thu
        [ChildActionOnly]
        public ActionResult _PhieuThu()
        {
            return PartialView("_PhieuThu");
        }

        [HttpPost]
        public ActionResult InfoPhieuThu()
        {
            return PartialView("_PhieuThu");
        }
        #endregion
    }
}
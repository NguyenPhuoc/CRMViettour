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

namespace CRMViettour.Controllers.Staff
{
    public class StaffTabInfoController : BaseController
    {
        // GET: StaffTabInfo

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Company> _companyRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private DataContext _db;

        public StaffTabInfoController(
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_StaffVisa> customerVisaRepository,
            IGenericRepository<tbl_Company> companyRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._staffVisaRepository = customerVisaRepository;
            this._taskRepository = taskRepository;
            this._companyRepository = companyRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._tourScheduleRepository = tourScheduleRepository;
            this._tourRepository = tourRepository;
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
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
            var model = await _staffRepository.GetById(id);
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
            //var model = await _appointmentHistoryRepository.GetAllAsQueryable().Where(p => p.StaffId == id).ToListAsync();
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.StaffId == id)
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

        #region Nhiệm vụ
        [ChildActionOnly]
        public ActionResult _NhiemVu()
        {
            return PartialView("_NhiemVu");
        }

        [HttpPost]
        public async Task<ActionResult> InfoNhiemVu(int id)
        {
            var model = _taskRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.StaffId == id)
                              .Select(p => new tbl_Task
                              {
                                  Id = p.Id,
                                  tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                                  tbl_DictionaryTaskStatus = _dictionaryRepository.FindId(p.TaskStatusId),
                                  Name = p.Name,
                                  Permission = p.Permission,
                                  StartDate = p.StartDate,
                                  EndDate = p.EndDate,
                                  Time = p.Time,
                                  TimeType = p.TimeType,
                                  FinishDate = p.FinishDate,
                                  PercentFinish = p.PercentFinish,
                                  Note = p.Note
                              }).ToList();
            return PartialView("_NhiemVu", model);
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
            //var model = await _contactHistoryRepository.GetAllAsQueryable().Where(p => p.StaffId == id).ToListAsync();
            var model = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.StaffId == id)
                       .Select(p => new tbl_ContactHistory
                       {
                           Id = p.Id,
                           ContactDate = p.CreatedDate,
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
        public ActionResult _ThauTour()
        {
            return PartialView("_ThauTour");
        }

        [HttpPost]
        public async Task<ActionResult> InfoThauTour(int id)
        {
            var model = _tourRepository.GetAllAsQueryable().Where(c => c.StaffId == id)
                .Select(p => new TourListViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    NumberCustomer = p.NumberCustomer ?? 0,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    NumberDay = p.NumberDay ?? 0,
                    TourType = p.tbl_DictionaryTypeTour.Name,
                    Status = p.tbl_DictionaryStatus.Name,
                }).ToList();
            foreach (var item in model)
            {
                item.CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id).Sum(c => c.ServicePrice) ?? 0;
                item.CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == item.Id).Sum(c => c.TotalContract) ?? 0;
            }
            return PartialView("_ThauTour", model);
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
            //var model = await _documentFileRepository.GetAllAsQueryable().Where(p => p.StaffId == id).ToListAsync();
            var model = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId == id)
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

        #region Khách hàng
        [ChildActionOnly]
        public ActionResult _KhachHang()
        {
            return PartialView("_KhachHang");
        }

        [HttpPost]
        public async Task<ActionResult> InfoKhachHang(int id)
        {
            var model = _customerRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.StaffManager == id)
                .Select(p => new CustomerListViewModel
                {
                    Id = p.Id,
                    Address = p.Address,
                    Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                    Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                    Code = p.Code == null ? "" : p.Code,
                    Company = p.CompanyId == null ? "" : _db.tbl_Company.Find(p.CompanyId).Name,
                    Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
                    StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
                    EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
                    Fullname = p.FullName == null ? "" : p.FullName,
                    Phone = p.Phone == null ? "" : p.Phone,
                    OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
                    Passport = p.PassportCard == null ? "" : p.PassportCard,
                    Skype = p.Skype == null ? "" : p.Skype,
                    TagsId = p.TagsId,
                    IdentityCard = p.IdentityCard ?? "",
                    Position = p.Position,
                    Department = p.Department,
                    CustomerType = p.CustomerType,
                    TaxCode = p.TaxCode

                }).ToList();
            return PartialView("_KhachHang", model);
        }
        #endregion

        #region Lịch sử đi tour
        [ChildActionOnly]
        public ActionResult _LichSuDiTour()
        {
            return PartialView("_LichSuDiTour");
        }

        [HttpPost]
        public async Task<ActionResult> InfoLichSuDiTour(int id)
        {
            var model = _tourScheduleRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.StaffId == id).ToList();
            return PartialView("_LichSuDiTour", model);
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
            var model = await _staffVisaRepository.GetAllAsQueryable().Where(p => p.StaffId == id).ToListAsync();
            return PartialView("_Visa", model);
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
            var model = await _updateHistoryRepository.GetAllAsQueryable().Where(p => p.StaffId == id).ToListAsync();
            return PartialView("_CapNhatThayDoi", model);
        }
        #endregion
    }
}
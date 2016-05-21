using CRM.Core;
using CRM.Enum;
using CRM.Infrastructure;
using CRMViettour.Models;
using CRMViettour.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    public class HomeController : BaseController
    {

        #region Init

        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_TourGuide> _tourGuideRepository;
        private IGenericRepository<tbl_TourSchedule> _tourScheduleRepository;
        private DataContext _db;

        public HomeController(IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            IGenericRepository<tbl_StaffVisa> staffVisaRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TourGuide> tourGuideRepository,
            IGenericRepository<tbl_TourSchedule> tourScheduleRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagsRepository = tagsRepository;
            this._customerRepository = customerRepository;
            this._staffRepository = staffRepository;
            this._tourRepository = tourRepository;
            this._customerContactRepository = customerContactRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._staffVisaRepository = staffVisaRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._tourGuideRepository = tourGuideRepository;
            this._tourScheduleRepository = tourScheduleRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        #region CreateCustomer
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateCustomer(CustomerViewModel model, FormCollection form)
        {
            try
            {
                if (model.SingleCompany.Code != null && model.SingleCompany.CompanyId != 0)
                {
                    model.SingleCompany.CustomerType = CustomerType.Personal;
                    model.SingleCompany.TagsId = form["SingleCompany.TagsId"];
                    model.SingleCompany.CreatedDate = DateTime.Now;
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

                    if (await _customerRepository.Create(model.SingleCompany))
                    {
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

                                UpdateHistory.SaveCustomer(model.SingleCompany.Id, 9, "Thêm mới khách hàng doanh nghiệp, code: " + model.SingleCompany.Code);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                if (model.SinglePersonal.Code != null && model.SinglePersonal.FullName != null)
                {
                    model.SinglePersonal.CustomerType = CustomerType.Personal;
                    model.SinglePersonal.TagsId = form["SinglePersonal.TagsId"];
                    model.SinglePersonal.CreatedDate = DateTime.Now;
                    model.SinglePersonal.ModifiedDate = DateTime.Now;
                    model.SinglePersonal.IdentityCard = model.IdentityCard;
                    model.SinglePersonal.IdentityTagId = model.IdentityTagId;
                    model.SinglePersonal.ParentId = 0;
                    model.SinglePersonal.PassportCard = model.PassportCard;
                    model.SinglePersonal.PassportTagId = model.PassportTagId;
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

                    if (await _customerRepository.Create(model.SinglePersonal))
                    {
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
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }
                                await _customerVisaRepository.Create(visa);

                                UpdateHistory.SaveCustomer(model.SinglePersonal.Id, 9, "Thêm mới khách hàng cá nhân, code: " + model.SinglePersonal.Code);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                if (model.SingleContact.Code != null && model.SingleContact.FullName != null && model.SingleContact.CustomerId != 0)
                {
                    model.SingleContact.TagsId = form["SinglePersonal.TagsId"];
                    model.SingleContact.CreatedDate = DateTime.Now;
                    model.SingleContact.ModifiedDate = DateTime.Now;
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

                    if (await _customerContactRepository.Create(model.SingleContact))
                    {
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
                                if (Convert.ToDateTime(form["CreatedDateVisa" + i]).Year >= 1980 && (Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year >= 1980))
                                {
                                    int age = Convert.ToDateTime(form["ExpiredDateVisa" + i]).Year - Convert.ToDateTime(form["CreatedDateVisa" + i]).Year;
                                    if (Convert.ToDateTime(form["CreatedDateVisa" + i]) > Convert.ToDateTime(form["ExpiredDateVisa" + i]).AddYears(-age)) age--;
                                    visa.Deadline = age;
                                }

                                await _customerContactVisaRepository.Create(visa);
                                UpdateHistory.SaveCustomer(model.SingleContact.Id, 9, "Thêm mới người liên hệ của khách hàng, code: " + model.SingleContact.Code);
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

        #region CreateStaff
        [HttpPost]
        public async Task<ActionResult> CreateStaff(StaffViewModel model, FormCollection form)
        {
            try
            {
                model.SingleStaff.TagsId = form["SingleStaff.TagsId"].ToString();
                model.SingleStaff.CreatedDate = DateTime.Now;
                model.SingleStaff.ModifiedDate = DateTime.Now;
                model.SingleStaff.IdentityTagId = Convert.ToInt32(form["IdentityTagId"].ToString());
                model.SingleStaff.PassportTagId = Convert.ToInt32(form["PassportTagId"].ToString());

                if (model.CreatedDateIdentity != null && model.CreatedDateIdentity.Year >= 1980)
                {
                    model.SingleStaff.CreatedDateIdentity = model.CreatedDateIdentity;
                }
                if (model.CreatedDatePassport != null && model.CreatedDatePassport.Year >= 1980)
                {
                    model.SingleStaff.CreatedDatePassport = model.CreatedDatePassport;
                }
                if (model.ExpiredDatePassport != null && model.ExpiredDatePassport.Year >= 1980)
                {
                    model.SingleStaff.ExpiredDatePassport = model.ExpiredDatePassport;
                }

                if (await _staffRepository.Create(model.SingleStaff))
                {
                    for (int i = 1; i < 6; i++)
                    {
                        if (form["VisaNumber" + i] != null && form["VisaNumber" + i] != "")
                        {
                            var visa = new tbl_StaffVisa
                            {
                                VisaNumber = form["VisaNumber" + i].ToString(),
                                TagsId = Convert.ToInt32(form["TagsId" + i].ToString()),
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                StaffId = model.SingleStaff.Id,
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
                            await _staffVisaRepository.Create(visa);

                            UpdateHistory.SaveStaff(9, "Thêm mới nhân viên có code " + model.SingleStaff.Code);
                        }
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion

        #region CreateTour
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateTour(TourViewModel model, FormCollection form)
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
    }
}

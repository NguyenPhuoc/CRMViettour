using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using CRMViettour.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Tour
{
    public class TourServiceController : BaseController
    {
        // GET: TourService
        #region Init

        private IGenericRepository<tbl_DeadlineOption> _deadlineOptionRepository;
        private IGenericRepository<tbl_Company> _companyRepository;
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

        public TourServiceController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_DeadlineOption> deadlineOptionRepository,
            IGenericRepository<tbl_Company> companyRepository,
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
            this._deadlineOptionRepository = deadlineOptionRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._companyRepository = companyRepository;
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

        #region Khách sạn

        [HttpPost]
        public async Task<ActionResult> CreateHotel(FormCollection form)
        {
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionHotel"]); i++)
                {
                    // insert dịch vụ khách hàng
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["currency-hotel-tour" + i]),
                        Note = form["note-hotel" + i].ToString(),
                        NumberNight = form["night-hotel" + i] != "" ? Convert.ToInt32(form["night-hotel" + i]) : 0,
                        NumberRoom = form["room-hotel" + i] != "" ? Convert.ToInt32(form["room-hotel" + i]) : 0,
                        Phone = form["phone-hotel" + i].ToString(),
                        Position = form["position-hotel" + i].ToString(),
                        Price = form["price-hotel" + i] != null ? Convert.ToDouble(form["price-hotel" + i].ToString()) : 0,
                        StaffContact = form["nguoi-lien-he-hotel" + i].ToString(),
                        Standard = form["star-hotel"] + i != "" ? Convert.ToInt32(form["star-hotel"] + i) : 0,
                        PartnerId = Convert.ToInt32(form["hotel-tour" + i])
                    };
                    if (form["tungay-hotel" + i] != "")
                    {
                        service.Time = Convert.ToDateTime(form["tungay-hotel" + i].ToString());
                    }
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = form["note-hotel" + i].ToString(),
                            PartnerId = service.PartnerId,
                            Request = service.NumberRoom + " phòng, " + service.NumberNight + " đêm, giá/đơn vị: " + string.Format("{0:0,0}", service.Price) + " " + _dictionaryRepository.FindId(service.tbl_DictionaryCurrency.Name),
                            StaffId = 9,
                            TourId = tourId
                        };
                        if (form["tungay-hotel" + i] != "")
                        {
                            contact.ContactDate = Convert.ToDateTime(form["tungay-hotel" + i].ToString());
                        }
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= 3; j++)
                            {
                                if (form["deadline-name-hotel" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["deadline-total-hotel" + i + j] != null ? Convert.ToDecimal(form["deadline-total-hotel" + i + j].ToString()) : 0,
                                        Name = form["deadline-name-hotel" + i + j].ToString(),
                                        Note = form["deadline-note-hotel" + i + j].ToString(),
                                        ServicesPartnerId = service.Id,
                                        StaffId = 9,
                                        StatusId = Convert.ToInt32(form["deadline-status-hotel" + i + j].ToString()),
                                        Time = Convert.ToDateTime(form["deadline-thoigian-hotel" + i + j].ToString()),
                                    };
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = 9,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1048,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId)
                            .Select(p => new TourServiceViewModel
                            {
                                Id = p.Id,
                                Code = p.tbl_Partner.Code,
                                ServiceId = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Id,
                                ServiceName = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Name,
                                Name = _partnerRepository.FindId(p.PartnerId).Name,
                                Address = _partnerRepository.FindId(p.PartnerId).Address,
                                StaffContact = _partnerRepository.FindId(p.PartnerId).StaffContact,
                                Phone = _partnerRepository.FindId(p.PartnerId).Phone,
                                Note = _partnerRepository.FindId(p.PartnerId).Note,
                                TourOptionId = p.Id,
                                TourId = p.TourId
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateHotel()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Nhà hàng

        /// <summary>
        /// upload file nhà hàng
        /// </summary>
        /// <param name="TransportDocument"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadFileRestaurant(HttpPostedFileBase RestaurantDocument, int id)
        {
            Session["RestaurantFile" + id] = RestaurantDocument;
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// thêm mới dịch vụ nhà hàng
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateRestaurant(FormCollection form)
        {
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionRestaurant"]); i++)
                {
                    // insert dịch vụ khách hàng
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["RestaurantCurrency" + i]),
                        Note = form["RestaurantNote" + i].ToString(),
                        Phone = form["DienThoai" + i].ToString(),
                        Price = form["RestaurantPrice" + i] != null ? Convert.ToDouble(form["RestaurantPrice" + i].ToString()) : 0,
                        StaffContact = form["NguoiLienHe" + i].ToString(),
                        PartnerId = Convert.ToInt32(form["RestaurantName" + i]),
                        Address = form["RestaurantAddress" + i].ToString()
                    };
                    // tài liệu
                    HttpPostedFileBase FileName = Session["RestaurantFile" + i] as HttpPostedFileBase;
                    String newName = "";
                    string FileSize = "";
                    if (FileName != null && FileName.ContentLength > 0)
                    {
                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                    }
                    if (newName != "" && FileSize != null)
                    {
                        service.FileName = newName;
                        Random rd = new Random();
                        // insert tbl_DocumentFile
                        var document = new tbl_DocumentFile
                        {
                            Code = rd.Next(1111, 9999).ToString(),
                            CreatedDate = DateTime.Now,
                            FileName = newName,
                            FileSize = FileSize,
                            IsCustomer = false,
                            IsRead = false,
                            ModifiedDate = DateTime.Now,
                            PartnerId = service.PartnerId,
                            PermissionStaff = "9",
                            StaffId = 9,
                            TourId = tourId
                        };
                        await _documentFileRepository.Create(document);
                    }
                    //end file
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = service.Note,
                            PartnerId = service.PartnerId,
                            Request = "giá: " + string.Format("{0:0,0}", service.Price) + " " + _dictionaryRepository.FindId(service.tbl_DictionaryCurrency.Name),
                            StaffId = 9,
                            TourId = tourId
                        };
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= 3; j++)
                            {
                                if (form["deadline-name-hotel" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["DeadlineDeposit" + i + j] != null ? Convert.ToDecimal(form["DeadlineDeposit" + i + j].ToString()) : 0,
                                        Name = form["DeadlineTen" + i + j].ToString(),
                                        Note = form["DeadlineNote" + i + j].ToString(),
                                        ServicesPartnerId = service.Id,
                                        StaffId = 9,
                                        StatusId = Convert.ToInt32(form["DeadlineStatus" + i + j].ToString()),
                                        Time = Convert.ToDateTime(form["DeadlineThoiGian1" + i + j].ToString()),
                                    };
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = 9,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1047,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId)
                            .Select(p => new TourServiceViewModel
                            {
                                Id = p.Id,
                                Code = p.tbl_Partner.Code,
                                ServiceId = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Id,
                                ServiceName = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Name,
                                Name = _partnerRepository.FindId(p.PartnerId).Name,
                                Address = _partnerRepository.FindId(p.PartnerId).Address,
                                StaffContact = _partnerRepository.FindId(p.PartnerId).StaffContact,
                                Phone = _partnerRepository.FindId(p.PartnerId).Phone,
                                Note = _partnerRepository.FindId(p.PartnerId).Note,
                                TourOptionId = p.Id,
                                TourId = p.TourId
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateRestaurant()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Vận chuyển

        /// <summary>
        /// upload file nhà hàng
        /// </summary>
        /// <param name="RestaurantDocument"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadFileTransport(HttpPostedFileBase TransportDocument, int id)
        {
            Session["TransportFile" + id] = TransportDocument;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTransport(FormCollection form)
        {
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i < Convert.ToInt32(form["NumberOptionTransport"]); i++)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        // insert ServicePartner
                        var service = new tbl_ServicesPartner{
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            StaffContact = form["nguoilienhe-transport" + i] != "" ? form["nguoilienhe-transport" + i].ToString() : "",
                            Phone= form["phone-transport" + i] != "" ? form["phone-transport" + i].ToString() : "",
                            PartnerId = Convert.ToInt32(form["name-transport" + i].ToString()),
                            Name= form["ServiceName" + i + j] != "" ?form["ServiceName" + i + j].ToString() : "",
                            Price = form["ServicePrice" + i + j] != "" ? Convert.ToDouble(form["ServicePrice" + i + j].ToString()) : 0,
                            CurrencyId = Convert.ToInt32(form["ServiceCurrency" + i + j].ToString()),
                            Note= form["ServiceNote" + i + j] != "" ? form["ServiceNote" + i + j].ToString() : "",
                        };

                        // tài liệu
                        HttpPostedFileBase FileName = Session["TransportFile" + i] as HttpPostedFileBase;
                        String newName = "";
                        string FileSize = "";
                        if (FileName != null && FileName.ContentLength > 0)
                        {
                            FileSize = Common.ConvertFileSize(FileName.ContentLength);
                            newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                            String path = Server.MapPath("~/Upload/file/" + newName);
                            FileName.SaveAs(path);
                        }
                        if (newName != "" && FileSize != null)
                        {
                            service.FileName = newName;
                            
                            Random rd = new Random();
                            // insert tbl_DocumentFile
                            var document = new tbl_DocumentFile
                            {
                                Code = rd.Next(1111, 9999).ToString(),
                                CreatedDate = DateTime.Now,
                                FileName = newName,
                                FileSize = FileSize,
                                IsCustomer = false,
                                IsRead = false,
                                ModifiedDate = DateTime.Now,
                                PartnerId = service.PartnerId,
                                PermissionStaff = "9",
                                StaffId = 9,
                                TourId = tourId
                            };
                            await _documentFileRepository.Create(document);
                        }
                        //end file

                        if (await _servicesPartnerRepository.Create(service))
                        {
                            // lưu option --> lịch sử liên hệ
                            var contact = new tbl_ContactHistory
                            {
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                DictionaryId = 1145,
                                Note = service.Note,
                                PartnerId = service.PartnerId,
                                Request =  "dịch vụ: " + service.Name + ", giá: " + string.Format("{0:0,0}", service.Price) + " " + _dictionaryRepository.FindId(service.tbl_DictionaryCurrency.Name),
                                StaffId = 9,
                                TourId = tourId
                            };

                            if (await _contactHistoryRepository.Create(contact))
                            {
                                // insert tbl_TourOption
                                var touroption = new tbl_TourOption
                                {
                                    PartnerId = service.PartnerId,
                                    ServiceId = 1050,
                                    ServicePartnerId = service.Id,
                                    TourId = tourId
                                };
                                await _tourOptionRepository.Create(touroption);
                            }
                        }
                    }
                }
            }
            catch { }

            var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId)
                           .Select(p => new TourServiceViewModel
                           {
                               Id = p.Id,
                               Code = p.tbl_Partner.Code,
                               ServiceId = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Id,
                               ServiceName = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Name,
                               Name = _partnerRepository.FindId(p.PartnerId).Name,
                               Address = _partnerRepository.FindId(p.PartnerId).Address,
                               StaffContact = _partnerRepository.FindId(p.PartnerId).StaffContact,
                               Phone = _partnerRepository.FindId(p.PartnerId).Phone,
                               Note = _partnerRepository.FindId(p.PartnerId).Note,
                               TourOptionId = p.Id,
                               TourId = p.TourId
                           }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTransport()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Vé máy bay

        /// <summary>
        /// upload file vé máy bay
        /// </summary>
        /// <param name="FileNamePlane"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFilePlane(HttpPostedFileBase FileNamePlane, int id)
        {
            Session["PlaneFile" + id] = FileNamePlane;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePlane(FormCollection form)
        {
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionPlane"]); i++)
                {
                    // insert dịch vụ vé máy bay
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["loaitien-plane" + i]),
                        Note = form["note-plane" + i].ToString(),
                        Phone = form["contacter-phone-plane" + i].ToString(),
                        Price = form["price-code" + i] != null ? Convert.ToDouble(form["price-code" + i].ToString()) : 0,
                        StaffContact = form["contacter-plane" + i].ToString(),
                        PartnerId = Convert.ToInt32(form["hang-plane" + i]),
                        Flight = form["flight-plane" + i] != null ? form["flight-plane" + i].ToString() : null,
                        NumberTicket = form["quantity-plane" + i] != null ? Convert.ToInt32(form["quantity-plane" + i].ToString()) : 0
                    };
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = service.Note,
                            PartnerId = service.PartnerId,
                            Request = "Chuyến bay: " + service.Flight + ", số lượng vé: "+ service.NumberTicket + ", giá/vé: " + string.Format("{0:0,0}", service.Price) + " " + _dictionaryRepository.FindId(service.tbl_DictionaryCurrency.Name),
                            StaffId = 9,
                            TourId = tourId
                        };
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= 3; j++)
                            {
                                if (form["name-deadline-plane" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["sotien-deadline-plane" + i + j] != null ? Convert.ToDecimal(form["sotien-deadline-plane" + i + j].ToString()) : 0,
                                        Name = form["name-deadline-plane" + i + j].ToString(),
                                        Note = form["PlaneNoteDeadline" + i + j].ToString(),
                                        ServicesPartnerId = service.Id,
                                        StaffId = 9,
                                        StatusId = Convert.ToInt32(form["tinhtrang-deadline-plane" + i + j].ToString()),
                                        Time = Convert.ToDateTime(form["thoigian-deadline-plane" + i + j].ToString()),
                                    };
                                    // tài liệu
                                    HttpPostedFileBase FileName = Session["PlaneFile" + i + j] as HttpPostedFileBase;
                                    String newName = "";
                                    string FileSize = "";
                                    if (FileName != null && FileName.ContentLength > 0)
                                    {
                                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                                        String path = Server.MapPath("~/Upload/file/" + newName);
                                        FileName.SaveAs(path);
                                    }
                                    if (newName != "" && FileSize != null)
                                    {
                                        service.FileName = newName;
                                        Random rd = new Random();
                                        // insert tbl_DocumentFile
                                        var document = new tbl_DocumentFile
                                        {
                                            Code = rd.Next(1111, 9999).ToString(),
                                            CreatedDate = DateTime.Now,
                                            FileName = newName,
                                            FileSize = FileSize,
                                            IsCustomer = false,
                                            IsRead = false,
                                            ModifiedDate = DateTime.Now,
                                            PartnerId = service.PartnerId,
                                            PermissionStaff = "9",
                                            StaffId = 9,
                                            TourId = tourId
                                        };
                                        await _documentFileRepository.Create(document);
                                    }
                                    //end file
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = 9,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1049,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId)
                            .Select(p => new TourServiceViewModel
                            {
                                Id = p.Id,
                                Code = p.tbl_Partner.Code,
                                ServiceId = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Id,
                                ServiceName = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Name,
                                Name = _partnerRepository.FindId(p.PartnerId).Name,
                                Address = _partnerRepository.FindId(p.PartnerId).Address,
                                StaffContact = _partnerRepository.FindId(p.PartnerId).StaffContact,
                                Phone = _partnerRepository.FindId(p.PartnerId).Phone,
                                Note = _partnerRepository.FindId(p.PartnerId).Note,
                                TourOptionId = p.Id,
                                TourId = p.TourId
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePlane()
        {
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
        }

        #endregion

        #region Sự kiện

        /// <summary>
        /// upload file sự kiện
        /// </summary>
        /// <param name="FileNameEvent"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFileEvent(HttpPostedFileBase FileNameEvent, int id)
        {
            Session["EventFile" + id] = FileNameEvent;
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// thêm mới sự kiện
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateEvent(FormCollection form)
        {
            int tourId = Convert.ToInt32(Session["idTour"].ToString());
            try
            {
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionEvent"]); i++)
                {
                    // insert dịch vụ khách hàng
                    var service = new tbl_ServicesPartner()
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CurrencyId = Convert.ToInt32(form["insert-currency-event" + i]),
                        Note = form["insert-note-event" + i].ToString(),
                        Phone = form["insert-phone-event" + i].ToString(),
                        Price = form["insert-price-event" + i] != null ? Convert.ToDouble(form["insert-price-event" + i].ToString()) : 0,
                        StaffContact = form["insert-contact-event" + i].ToString(),
                        PartnerId = Convert.ToInt32(form["insert-company-event" + i])
                    };
                    // tài liệu
                    HttpPostedFileBase FileName = Session["EventFile" + i] as HttpPostedFileBase;
                    String newName = "";
                    string FileSize = "";
                    if (FileName != null && FileName.ContentLength > 0)
                    {
                        FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                    }
                    if (newName != "" && FileSize != null)
                    {
                        service.FileName = newName;
                        Random rd = new Random();
                        // insert tbl_DocumentFile
                        var document = new tbl_DocumentFile
                        {
                            Code = rd.Next(1111, 9999).ToString(),
                            CreatedDate = DateTime.Now,
                            FileName = newName,
                            FileSize = FileSize,
                            IsCustomer = false,
                            IsRead = false,
                            ModifiedDate = DateTime.Now,
                            PartnerId = service.PartnerId,
                            PermissionStaff = "9",
                            StaffId = 9,
                            TourId = tourId
                        };
                        await _documentFileRepository.Create(document);
                    }
                    //end file
                    if (await _servicesPartnerRepository.Create(service))
                    {
                        // lưu option --> lịch sử liên hệ
                        var contact = new tbl_ContactHistory
                        {
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            DictionaryId = 1145,
                            Note = service.Note,
                            PartnerId = service.PartnerId,
                            Request = "Tổng giá trị: " + string.Format("{0:0,0}", service.Price) + " " + _dictionaryRepository.FindId(service.tbl_DictionaryCurrency.Name),
                            StaffId = 9,
                            TourId = tourId
                        };
                        if (await _contactHistoryRepository.Create(contact))
                        {
                            // insert deadline dịch vụ
                            for (int j = 1; j <= 3; j++)
                            {
                                if (form["deadline-name-event" + i + j] != "")
                                {
                                    // insert tbl_DeadlineOption
                                    var deadline = new tbl_DeadlineOption
                                    {
                                        CreatedDate = DateTime.Now,
                                        Deposit = form["deadline-total-event" + i + j] != null ? Convert.ToDecimal(form["deadline-total-event" + i + j].ToString()) : 0,
                                        Name = form["deadline-name-event" + i + j].ToString(),
                                        Note = form["deadline-note-event" + i + j].ToString(),
                                        ServicesPartnerId = service.Id,
                                        StaffId = 9,
                                        StatusId = Convert.ToInt32(form["deadline-status-event" + i + j].ToString()),
                                        Time = Convert.ToDateTime(form["deadline-thoigian-event" + i + j].ToString()),
                                    };
                                    if (await _deadlineOptionRepository.Create(deadline))
                                    {
                                        // insert tbl_AppointmentHistory
                                        var appointment = new tbl_AppointmentHistory
                                        {
                                            CreatedDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            DictionaryId = 1214,
                                            IsNotify = true,
                                            IsRepeat = true,
                                            Note = deadline.Note,
                                            Notify = 5,
                                            PartnerId = service.PartnerId,
                                            Repeat = 5,
                                            StaffId = 9,
                                            StatusId = deadline.StatusId,
                                            Time = deadline.Time ?? DateTime.Now,
                                            Title = deadline.Name,
                                            TourId = tourId
                                        };
                                        await _appointmentHistoryRepository.Create(appointment);
                                    }
                                    // insert tbl_TourOption
                                    var touroption = new tbl_TourOption
                                    {
                                        DeadlineId = deadline.Id,
                                        PartnerId = service.PartnerId,
                                        ServiceId = 1051,
                                        ServicePartnerId = service.Id,
                                        TourId = tourId
                                    };
                                    await _tourOptionRepository.Create(touroption);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId)
                            .Select(p => new TourServiceViewModel
                            {
                                Id = p.Id,
                                Code = p.tbl_Partner.Code,
                                ServiceId = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Id,
                                ServiceName = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Name,
                                Name = _partnerRepository.FindId(p.PartnerId).Name,
                                Address = _partnerRepository.FindId(p.PartnerId).Address,
                                StaffContact = _partnerRepository.FindId(p.PartnerId).StaffContact,
                                Phone = _partnerRepository.FindId(p.PartnerId).Phone,
                                Note = _partnerRepository.FindId(p.PartnerId).Note,
                                TourOptionId = p.Id,
                                TourId = p.TourId
                            }).ToList();
            return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
        }
        #endregion

        #region Khác
        #endregion

        #region Xóa dịch vụ

        [HttpPost]
        public async Task<ActionResult> DeleteService(int tourid, int optionid)
        {
            try
            {
                await _tourOptionRepository.Delete(optionid, true);
                var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourid).
                            Select(p => new TourServiceViewModel
                            {
                                Id = p.Id,
                                Code = p.tbl_Partner.Code,
                                ServiceId = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Id,
                                ServiceName = _dictionaryRepository.FindId(p.tbl_Partner.DictionaryId).Name,
                                Name = p.tbl_Partner.Name,
                                Address = p.tbl_Partner.Address,
                                StaffContact = p.tbl_Partner.StaffContact,
                                Phone = p.tbl_Partner.Phone,
                                Note = p.tbl_Partner.Note,
                                TourOptionId = p.Id,
                                TourId = p.TourId
                            }).ToList();
                return PartialView("~/Views/TourTabInfo/_DichVu.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_DichVu.cshtml");
            }
        }
        #endregion
    }
}
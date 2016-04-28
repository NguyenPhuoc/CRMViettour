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

        #endregion

        #region Nhà hàng
        #endregion

        #region Vận chuyển
        #endregion

        #region Vé máy bay

        #endregion

        #region Sự kiện

        /// <summary>
        /// upload file sự kiện
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFileEvent(HttpPostedFileBase FileName, int id)
        {
            Session["EventFile" + id] = FileName;
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
            try
            {
                int tourId = Convert.ToInt32(Session["idTour"].ToString());
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionEventService"].ToString()); i++)
                {
                    //file
                    HttpPostedFileBase FileName = Session["EventFile" + i] as HttpPostedFileBase;
                    String newName = "";
                    if (FileName != null && FileName.ContentLength > 0)
                    {
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                    }
                    //end file

                    var model = new tbl_Partner()
                    {
                        CompanyId = Convert.ToInt32(form["CompanyId" + i].ToString()),
                        Name = _companyRepository.FindId(Convert.ToInt32(form["CompanyId" + i].ToString())).Name,
                        Code = form["Code" + i].ToString(),
                        FileName = newName,
                        Price = Convert.ToDecimal(form["Price" + i].ToString()),
                        StaffContact = form["StaffContact" + i].ToString(),
                        Phone = form["Phone" + i].ToString(),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        Note = form["Note" + i].ToString(),
                        IsUse = true,
                        DictionaryId = 1051
                    };

                    if (await _partnerRepository.Create(model))
                    {
                        var item = new tbl_TourOption
                        {
                            PartnerId = model.Id,
                            ServiceId = 1051,
                            TourId = tourId
                        };
                        await _tourOptionRepository.Create(item);
                    }
                }
                var list = _tourOptionRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).
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
                                 Price = p.tbl_Partner.Price,
                                 Note = p.tbl_Partner.Note,
                                 TourOptionId = p.Id,
                                 TourId = p.TourId
                             }).ToList();
                return PartialView("_Partial_InsertEvent", list);
            }
            catch { }

            return PartialView("_Partial_InsertEvent");
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
                                Price = p.tbl_Partner.Price,
                                Note = p.tbl_Partner.Note,
                                TourOptionId = p.Id,
                                TourId = p.TourId
                            }).ToList();
                return PartialView("_Partial_InsertEvent", list);
            }
            catch
            {
                return PartialView("_Partial_InsertEvent");
            }
        }
        #endregion
    }
}
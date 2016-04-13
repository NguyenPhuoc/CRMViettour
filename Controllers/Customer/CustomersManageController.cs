using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRMViettour.Models;
using CRM.Core;
using CRM.Infrastructure;
using System.Threading.Tasks;
using CRM.Enum;
using CRMViettour.Utilities;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CRMViettour.Controllers
{
    public class CustomersManageController : BaseController
    {
        //
        // GET: /CustomersManage/

        #region Init

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
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public CustomersManageController(
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
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
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
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }

        #endregion

        #region Index

        [HttpPost]
        public ActionResult GetIdCustomer(int id)
        {
            Session["idCustomer"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _customerRepository.GetAllAsQueryable().AsEnumerable()
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
                    Position = p.Position,
                    Department = p.Department
                }).ToList();

            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(CustomerViewModel model, FormCollection form)
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
                        if (await _customerRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "CustomersManage") }, JsonRequestBehavior.AllowGet);
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

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["CustomerFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                string id = Session["idCustomer"].ToString();
                if (ModelState.IsValid)
                {
                    model.CustomerId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    //file
                    HttpPostedFileBase FileName = Session["CustomerFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file
                    if (newName != null && FileSize != null)
                    {
                        model.FileName = newName;
                        model.FileSize = FileSize;
                    }

                    if (await _documentFileRepository.Create(model))
                    {
                        Session["CustomerFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId.ToString() == id)
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
                        return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditDocument()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditDocument", new tbl_DocumentFile());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            var model = await _documentFileRepository.GetById(id);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            foreach (var t in _db.tbl_Tags.ToList())
            {
                lstTag.Add(new SelectListItem()
                {
                    Text = t.Tag,
                    Value = t.Id.ToString(),
                    Selected = model.TagsId.Split(',').Contains(t.Id.ToString()) ? true : false
                });
            }
            ViewBag.TagsId = lstTag;
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditDocument", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    if (Session["CustomerFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["CustomerFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                        //end file

                        if (FileName != null && FileSize != null)
                        {
                            String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                            if (System.IO.File.Exists(pathOld))
                                System.IO.File.Delete(pathOld);
                            model.FileName = newName;
                            model.FileSize = FileSize;
                        }
                    }
                    if (await _documentFileRepository.Update(model))
                    {
                        Session["CustomerFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == model.CustomerId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == model.CustomerId)
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
                        return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                int cusId = _documentFileRepository.FindId(id).CustomerId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                if (await _documentFileRepository.Delete(id, true))
                {
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == cusId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == id)
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
                    return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
            }
        }

        #endregion

        #region Visa
        /********** Quản lý visa ************/
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateVisa(tbl_CustomerVisa model, FormCollection form)
        {
            try
            {
                string id = Session["idCustomer"].ToString();
                if (ModelState.IsValid)
                {
                    model.CustomerId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;

                    if (model.CreatedDate != null)
                    {
                        model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null)
                    {
                        model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
                    {
                        int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                        if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                        model.Deadline = age;
                    }

                    if (await _customerVisaRepository.Create(model))
                    {
                        var list = _db.tbl_CustomerVisa.AsEnumerable().Where(p => p.CustomerId.ToString() == id).ToList();
                        return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditVisa()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditVisa", new tbl_CustomerVisa());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoVisa(int id)
        {
            var model = await _customerVisaRepository.GetById(id);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            foreach (var t in LoadData.DropdownlistCountry().ToList())
            {
                lstTag.Add(new SelectListItem()
                {
                    Text = t.Tags,
                    Value = t.Id.ToString(),
                    Selected = model.TagsId == t.Id ? true : false
                });
            }
            ViewBag.TagsId = lstTag;
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 14), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditVisa", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateVisa(tbl_CustomerVisa model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CustomerId = Convert.ToInt32(model.CustomerId);
                    model.ModifiedDate = DateTime.Now;
                    if (model.CreatedDate != null)
                    {
                        model.CreatedDateVisa = Convert.ToDateTime(form["CreatedDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null)
                    {
                        model.ExpiredDateVisa = Convert.ToDateTime(form["ExpiredDateVisa"]);
                    }
                    if (model.ExpiredDateVisa != null && model.CreatedDateVisa != null)
                    {
                        int age = model.ExpiredDateVisa.Value.Year - model.CreatedDateVisa.Value.Year;
                        if (model.CreatedDateVisa > model.ExpiredDateVisa.Value.AddYears(-age)) age--;
                        model.Deadline = age;
                    }

                    if (await _customerVisaRepository.Update(model))
                    {
                        var list = _db.tbl_CustomerVisa.AsEnumerable().Where(p => p.CustomerId == model.CustomerId).ToList();
                        return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteVisa(int id)
        {
            try
            {
                int visaId = _customerVisaRepository.FindId(id).CustomerId;

                if (await _customerVisaRepository.Delete(id, true))
                {
                    var list = _db.tbl_CustomerVisa.AsEnumerable().Where(p => p.CustomerId == visaId).ToList();
                    return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/CustomerTabInfo/_Visa.cshtml");
            }
        }

        #endregion

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
        public async Task<ActionResult> Update(CustomerViewModel model, FormCollection form)
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
                                await _customerVisaRepository.Delete(v.Id, true);
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
                                await _customerVisaRepository.Delete(v.Id, true);
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
                                await _customerContactVisaRepository.Delete(v.Id, true);
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

        #region Import
        #endregion

        #region Export
        [HttpPost]
        public ActionResult ExportFile()
        {
            //var grid = new GridView();
            //grid.AutoGenerateColumns = false;
            //grid.HeaderStyle.Font.Bold = true;
            //grid.HeaderStyle.Height = Unit.Pixel(40);

            ////grid.BorderColor = System.Drawing.Color.DarkGray;
            //grid.Font.Name = "Arial";
            //grid.CellPadding = 50;
            //grid.AllowPaging = false;


            //#region BoundField
            //BoundField bfield = new BoundField();
            //bfield.HeaderText = "HỌ TÊN";
            //bfield.DataField = "FullName";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(150);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "D.O.B";
            //bfield.DataField = "Birthday";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(100);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "PP NO.";
            //bfield.DataField = "Passport";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(75);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "D.O.I";
            //bfield.DataField = "StartDate";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(100);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "D.O.E";
            //bfield.DataField = "EndDate";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(100);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "ĐIỆN THOẠI";
            //bfield.DataField = "Phone";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(110);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "SỐ ĐT KHÁC";
            //bfield.DataField = "OtherPhone";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(110);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "EMAIL";
            //bfield.DataField = "Email";
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.Width = Unit.Pixel(175);
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "NGHỀ NGHIỆP";
            //bfield.DataField = "Career";
            //bfield.ItemStyle.Width = Unit.Pixel(175);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "CÔNG TY";
            //bfield.DataField = "Company";
            //bfield.ItemStyle.Width = Unit.Pixel(175);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "ĐỊA CHỈ";
            //bfield.DataField = "Address";
            //bfield.ItemStyle.Width = Unit.Pixel(250);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "KHU VỰC";
            //bfield.DataField = "TagsId";
            //bfield.ItemStyle.Width = Unit.Pixel(200);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "CMND";
            //bfield.DataField = "IdentityCard";
            //bfield.ItemStyle.Width = Unit.Pixel(110);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "TÊN NGƯỜI LIÊN LẠC";
            //bfield.DataField = "NameCustomerContract";
            //bfield.ItemStyle.Width = Unit.Pixel(150);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "SỐ ĐT LIÊN LẠC";
            //bfield.DataField = "PhoneCustomerContract";
            //bfield.ItemStyle.Width = Unit.Pixel(110);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "MỨC ĐỘ VIP";
            //bfield.DataField = "";
            //bfield.ItemStyle.Width = Unit.Pixel(125);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);

            //bfield = new BoundField();
            //bfield.HeaderText = "GHI CHÚ";
            //bfield.DataField = "Note";
            //bfield.ItemStyle.Width = Unit.Pixel(150);
            //bfield.ItemStyle.BorderColor = Color.DarkGray;
            //bfield.ItemStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.HeaderStyle.BackColor = System.Drawing.Color.Silver;
            //bfield.ItemStyle.VerticalAlign = VerticalAlign.Middle;
            //grid.Columns.Add(bfield);
            //#endregion

            //grid.EnableTheming = true;

            //grid.DataSource = _customerRepository.GetAllAsQueryable().AsEnumerable()
            //    .Select(p => new CustomerListViewModel
            //    {
            //        Fullname = p.FullName == null ? "" : p.FullName,
            //        Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
            //        Passport = p.PassportCard == null ? "" : p.PassportCard,
            //        StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
            //        EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
            //        Phone = p.Phone == null ? "" : p.Phone,
            //        OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
            //        Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
            //        Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
            //        Company = p.CompanyId == null ? "" : _db.tbl_Company.Find(p.CompanyId).Name,
            //        Address = p.Address == null ? "" : p.Address,
            //        TagsId = p.TagsId == null ? "" : LoadData.LocationTags(p.TagsId),
            //        IdentityCard = p.IdentityCard == null ? "" : p.IdentityCard,
            //        NameCustomerContract = _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault() == null ? "" : _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault().FullName,
            //        PhoneCustomerContract = _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault() == null ? "" : _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault().PhoneNR,

            //        Note = p.Note == null ? "" : p.Note,
            //    }).ToList();

            //grid.DataBind();




            //Response.ClearContent();
            //Response.AddHeader("content-disposition", "attachment; filename=Customers.xls");
            //Response.ContentType = "application/excel";
            //Response.ContentEncoding = System.Text.Encoding.Unicode;
            //Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);

            //htw.WriteLine("DANH SÁCH KHÁCH HÀNG");
            //grid.RenderControl(htw);

            //Response.Write(sw.ToString());

            //Response.End();


            var customers = _customerRepository.GetAllAsQueryable().AsEnumerable()
                 .Select(p => new CustomerListViewModel
                 {
                     Fullname = p.FullName == null ? "" : p.FullName,
                     Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                     Passport = p.PassportCard == null ? "" : p.PassportCard,
                     StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("dd-MM-yyyy"),
                     EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy"),
                     Phone = p.Phone == null ? "" : p.Phone,
                     OtherPhone = p.MobilePhone == null ? "" : p.MobilePhone,
                     Email = p.CompanyEmail == null ? p.PersonalEmail : p.CompanyEmail,
                     Career = p.CareerId != null ? p.tbl_DictionaryCareer.Name : "",
                     Company = p.CompanyId == null ? "" : _db.tbl_Company.Find(p.CompanyId).Name,
                     Address = p.Address == null ? "" : p.Address,
                     TagsId = p.TagsId == null ? "" : LoadData.LocationTags(p.TagsId),
                     IdentityCard = p.IdentityCard == null ? "" : p.IdentityCard,
                     NameCustomerContract = _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault() == null ? "" : _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault().FullName,
                     PhoneCustomerContract = _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault() == null ? "" : _db.tbl_CustomerContact.Where(s => s.CustomerId == p.Id).SingleOrDefault().PhoneNR,

                     Note = p.Note == null ? "" : p.Note,
                 }).ToList();

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, customers);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "customers.xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }
        #endregion


        public virtual void ExportCustomersToXlsx(Stream stream, IList<CustomerListViewModel> customers)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Customers");

                //Create Headers and format them
                var properties = new[]
                    {
                        "No.",
                        "HỌ TÊN",
                        "D.O.B",
                        "PP NO.",
                        "D.O.I",
                        "D.O.E",
                        "ĐIỆN THOẠI",
                        "SỐ ĐT KHÁC",
                        "EMAIL",
                        "NGHỀ NGHIỆP",
                        "CÔNG TY",
                        "KHU VỰC",
                        "ĐỊA CHỈ",
                        "CMND",
                        "TÊN NGƯỜI LIÊN LẠC",
                        "SỐ ĐT LIÊN LẠC",
                        "MỨC ĐỘ VIP",
                        "GHI CHÚ"
                        
                    };

                worksheet.Cells[4, 1].Value = "Tour Code:";
                worksheet.Cells[5, 1].Value = "Flight details:";
                worksheet.Cells[6, 1].Value = "NAME LIST OF OUR GROUP TO HONG KONG/  DAYS";
                worksheet.Cells["a6:r6,a8:r8"].Style.Font.Bold = true;
                worksheet.Cells["a6:r6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a6:r6"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[8, i + 1].Value = properties[i];
                }


                int row = 9;
                foreach (var customer in customers)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = row - 8;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Fullname;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Birthday;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Passport;
                    col++;

                    worksheet.Cells[row, col].Value = customer.StartDate;
                    col++;

                    worksheet.Cells[row, col].Value = customer.EndDate;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Phone;
                    col++;

                    worksheet.Cells[row, col].Value = customer.OtherPhone;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Email;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Career;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Company;
                    col++;

                    worksheet.Cells[row, col].Value = customer.TagsId;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Address;
                    col++;

                    worksheet.Cells[row, col].Value = customer.IdentityCard;
                    col++;

                    worksheet.Cells[row, col].Value = customer.NameCustomerContract;
                    col++;

                    worksheet.Cells[row, col].Value = customer.PhoneCustomerContract;
                    col++;

                    worksheet.Cells[row, col].Value = string.Empty;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Note.ToUpperInvariant();
                    col++;

                    row++;
                }
                row--;
                worksheet.Cells["a1:r" + row].Style.Font.Size = 12;
                worksheet.Cells["a9:a" + row].Style.Font.Bold = true;
                worksheet.Cells["j8:r" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["j8:r" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));



                worksheet.Cells["a8:r" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].AutoFitColumns();

                worksheet.Column(1).Width = 3.57;
                worksheet.Column(2).Width = 18;

                // we had better add some document properties to the spreadsheet 

                // set some core property values
                //var storeName = _storeInformationSettings.StoreName;
                //var storeUrl = _storeInformationSettings.StoreUrl;
                //xlPackage.Workbook.Properties.Title = string.Format("{0} customers", storeName);
                //xlPackage.Workbook.Properties.Author = storeName;
                //xlPackage.Workbook.Properties.Subject = string.Format("{0} customers", storeName);
                //xlPackage.Workbook.Properties.Keywords = string.Format("{0} customers", storeName);
                //xlPackage.Workbook.Properties.Category = "Customers";
                //xlPackage.Workbook.Properties.Comments = string.Format("{0} customers", storeName);

                // set some extended property values
                //xlPackage.Workbook.Properties.Company = storeName;
                //xlPackage.Workbook.Properties.HyperlinkBase = new Uri(storeUrl);

                // save the new spreadsheet
                xlPackage.Save();
            }
        }
    }

}

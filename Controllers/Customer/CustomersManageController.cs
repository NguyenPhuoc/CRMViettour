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
using System.Globalization;

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
            //try
            //{

            string id = Session["idCustomer"].ToString();
            if (ModelState.IsValid)
            {
                model.CustomerId = Convert.ToInt32(id);
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();
                model.StaffId = 9;
                //file
                HttpPostedFileBase FileName = Session["CustomerFile"] as HttpPostedFileBase;
                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
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
            //}
            //catch { }
            return PartialView("~/Views/CustomerTabInfo/_HoSoLienQuan.cshtml");
        }

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
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == cusId)
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
                model.IsTemp = customer.IsTemp;
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
        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {

                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<tbl_Customer> list = new List<tbl_Customer>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    if (worksheet.Cells["B2"].Text == "0")//Công ty
                        for (int row = 2; row <= lastRow; row++)
                        {
                            if (worksheet.Cells["c" + row].Value == null || worksheet.Cells["c" + row].Text == "")
                                continue;
                            var cus = new tbl_Customer
                            {
                                CustomerType = CustomerType.Organization,
                                FullName = worksheet.Cells["c" + row].Value != null ? worksheet.Cells["c" + row].Text : null,
                                Director = worksheet.Cells["d" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                                Address = worksheet.Cells["e" + row].Value != null ? worksheet.Cells["e" + row].Text : null,
                                CompanyEmail = worksheet.Cells["i" + row].Value != null ? worksheet.Cells["i" + row].Text : null,
                                Phone = worksheet.Cells["j" + row].Value != null ? worksheet.Cells["j" + row].Text : null,
                                Fax = worksheet.Cells["k" + row].Value != null ? worksheet.Cells["k" + row].Text : null,
                                TaxCode = worksheet.Cells["l" + row].Value != null ? worksheet.Cells["l" + row].Text : null,
                                CreatedPlaceTaxCode = worksheet.Cells["n" + row].Value != null ? worksheet.Cells["n" + row].Text : null,
                                AccountNumber = worksheet.Cells["o" + row].Value != null ? worksheet.Cells["o" + row].Text : null,
                                Bank = worksheet.Cells["p" + row].Value != null ? worksheet.Cells["p" + row].Text : null,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                            };
                            string cel = "";
                            try//ngay cap ma so thue
                            {
                                cel = "m";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.CreatedDateTaxCode = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//công ty
                            {
                                cel = "c";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string congty = worksheet.Cells[cel + row].Text;
                                    cus.CompanyId = _companyRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == congty).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try//công ty
                            {
                                cel = "c";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string congty = worksheet.Cells[cel + row].Text;
                                    cus.CompanyId = _companyRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == congty).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try//tagid dia chi
                            {
                                cel = "f";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string tinhtp = worksheet.Cells[cel + row].Text;
                                    cus.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tinhtp && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault().ToString();
                                }
                                cel = "g";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string quanhuyen = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == quanhuyen && c.TypeTag == 6).SingleOrDefault();
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                                cel = "h";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string phuongxa = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == phuongxa && c.TypeTag == 7).SingleOrDefault();
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                            }
                            catch { }
                            try//nhóm khách hàng
                            {
                                cel = "q";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string nhomkh = worksheet.Cells[cel + row].Text;
                                    cus.CustomerGroupId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == nhomkh && c.DictionaryCategoryId == 3).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try//ngành nghề
                            {
                                cel = "r";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string nganhnghe = worksheet.Cells[cel + row].Text;
                                    cus.CareerId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == nganhnghe && c.DictionaryCategoryId == 2).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }

                            list.Add(cus);
                        }
                    else//cá nhân=====================================================
                        for (int row = 2; row <= lastRow; row++)
                        {
                            if (worksheet.Cells["c" + row].Value == null || worksheet.Cells["c" + row].Text == "")
                                continue;
                            var cus = new tbl_Customer
                            {
                                CustomerType = CustomerType.Personal,
                                FullName = worksheet.Cells["d" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                                PersonalEmail = worksheet.Cells["f" + row].Value != null ? worksheet.Cells["f" + row].Text : null,
                                Phone = worksheet.Cells["g" + row].Value != null ? worksheet.Cells["g" + row].Text : null,
                                Address = worksheet.Cells["h" + row].Value != null ? worksheet.Cells["h" + row].Text : null,
                                AccountNumber = worksheet.Cells["o" + row].Value != null ? worksheet.Cells["o" + row].Text : null,
                                Bank = worksheet.Cells["p" + row].Value != null ? worksheet.Cells["p" + row].Text : null,
                                IdentityCard = worksheet.Cells["q" + row].Value != null ? worksheet.Cells["q" + row].Text : null,
                                PassportCard = worksheet.Cells["t" + row].Value != null ? worksheet.Cells["t" + row].Text : null,
                                Position = worksheet.Cells["l" + row].Value != null ? worksheet.Cells["l" + row].Text : null,
                                Department = worksheet.Cells["m" + row].Value != null ? worksheet.Cells["m" + row].Text : null,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                            };
                            string cel = "";
                            try//ngay sinh
                            {
                                cel = "e";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.Birthday = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay cấp cmnd
                            {
                                cel = "r";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.CreatedDateIdentity = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay hiệu lực passport
                            {
                                cel = "u";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.CreatedDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//ngay hết hạn passport
                            {
                                cel = "v";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    cus.ExpiredDatePassport = DateTime.ParseExact(worksheet.Cells[cel + row].Text, "d/M/yyyy", CultureInfo.InvariantCulture);
                                }
                            }
                            catch { }
                            try//danh xưng
                            {
                                cel = "c";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                    {
                                        string danhsung = worksheet.Cells[cel + row].Text;
                                        cus.NameTypeId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == danhsung && c.DictionaryCategoryId == 7).Select(c => c.Id).SingleOrDefault();
                                    }
                                }
                            }
                            catch { }
                            try//tagid dia chi
                            {
                                cel = "i";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string tinhtp = worksheet.Cells[cel + row].Text;
                                    cus.TagsId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == tinhtp && c.TypeTag == 5).Select(c => c.Id).SingleOrDefault().ToString();
                                }
                                cel = "j";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string quanhuyen = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == quanhuyen && c.TypeTag == 6).SingleOrDefault();
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                                cel = "k";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string phuongxa = worksheet.Cells[cel + row].Text;
                                    var tagid = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == phuongxa && c.TypeTag == 7).SingleOrDefault();
                                    if (tagid != null)
                                        if (cus.TagsId != null)
                                            cus.TagsId += "," + tagid.Id;
                                        else
                                            cus.TagsId = tagid.Id.ToString();
                                }
                            }
                            catch { }
                            try//ngành nghề
                            {
                                cel = "n";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string nganhnghe = worksheet.Cells[cel + row].Text;
                                    cus.CareerId = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Name == nganhnghe && c.DictionaryCategoryId == 2).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try//noi cap cmnd
                            {
                                cel = "s";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string noicap = worksheet.Cells[cel + row].Text;
                                    cus.IdentityTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noicap && c.TypeTag == 3).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }
                            try//noi cap passport
                            {
                                cel = "w";
                                if (worksheet.Cells[cel + row].Value != null && worksheet.Cells[cel + row].Text != "")
                                {
                                    string noicap = worksheet.Cells[cel + row].Text;
                                    cus.PassportTagId = _tagsRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.Tag == noicap && c.TypeTag == 3).Select(c => c.Id).SingleOrDefault();
                                }
                            }
                            catch { }

                            list.Add(cus);
                        }
                    Session["listCustomerImport"] = list;
                    return PartialView("_Partial_ImportDataList", list);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpPost]
        public async Task<ActionResult> SaveImport()
        {
            try
            {
                List<tbl_Customer> list = Session["listCustomerImport"] as List<tbl_Customer>;
                int i = 0;
                foreach (var item in list)
                {
                    if (item.CustomerType == CustomerType.Personal)
                    {
                        item.Code = LoadData.NewCodeCustomerPersonal();
                        await _customerRepository.Create(item);
                        i++;
                    }
                    else
                    {
                        item.Code = "DEMO" + new Random().Next(10000, 99999);
                        await _customerRepository.Create(item);
                        i++;
                    }
                } Session["listCustomerImport"] = null;
                if (i != 0)
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Đã import thành công " + i + " dòng dữ liệu !", IsPartialView = false, RedirectTo = Url.Action("Index", "CustomersManage") }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Chưa có dữ liệu nào được import !" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                Session["listCustomerImport"] = null;
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Import dữ liệu lỗi !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteImport(String listItemId)
        {
            try
            {
                List<tbl_Customer> list = Session["listCustomerImport"] as List<tbl_Customer>;
                if (listItemId != null && listItemId != "")
                {
                    var listIds = listItemId.Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        int[] listIdsint = new int[listIds.Length];
                        for (int i = 0; i < listIds.Length; i++)
                        {
                            listIdsint[i] = Int32.Parse(listIds[i]);
                        }
                        for (int i = 0; i < listIdsint.Length; i++)
                        {
                            for (int j = i; j < listIdsint.Length; j++)
                            {
                                if (listIdsint[i] < listIdsint[j])
                                {
                                    int temp = listIdsint[i];
                                    listIdsint[i] = listIdsint[j];
                                    listIdsint[j] = temp;
                                }
                            }
                        }
                        foreach (var item in listIdsint)
                        {
                            list.RemoveAt(item);
                        }
                    }
                }
                Session["listCustomerImport"] = list;
                return PartialView("_Partial_ImportDataList", list);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Export
        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile()
        {
            var customers = _customerRepository.GetAllAsQueryable().AsEnumerable()
                 .Select(p => new CustomerListViewModel
                 {
                     Fullname = p.FullName == null ? "" : p.FullName,
                     Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("d/M/yyyy"),
                     Passport = p.PassportCard == null ? "" : p.PassportCard,
                     StartDate = p.CreatedDatePassport == null ? "" : p.CreatedDatePassport.Value.ToString("d/M/yyyy"),
                     EndDate = p.ExpiredDatePassport == null ? "" : p.ExpiredDatePassport.Value.ToString("d/M/yyyy"),
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
                return File(bytes, "text/xls", "Customers.xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }


        public virtual void ExportCustomersToXlsx(Stream stream, IList<CustomerListViewModel> customers)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Customers");

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
                worksheet.Cells["a1:r" + row].Style.Font.SetFromFont(new Font("Arial", 12));
                worksheet.Cells["a6:r6,a8:r8"].Style.Font.Bold = true;
                worksheet.Cells["a8:r8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a6:r6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a6:r6"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));

                worksheet.Cells["a9:a" + row].Style.Font.Bold = true;
                worksheet.Cells["a9:a" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["j8:r" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["j8:r" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));



                worksheet.Cells["a8:r" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a8:r" + row].AutoFitColumns();

                worksheet.Row(6).Height = 18.75;
                worksheet.Row(8).Height = 18;
                worksheet.Column(1).Width = 4.5;
                worksheet.Column(2).Width = 18;

                xlPackage.Save();
            }
        }
        #endregion
    }

}

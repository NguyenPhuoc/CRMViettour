using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using CRMViettour.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    public class StaffManageController : BaseController
    {
        //
        // GET: /StaffManage/

        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private DataContext _db;

        public StaffManageController(IGenericRepository<tbl_Staff> staffRepository, IGenericRepository<tbl_StaffVisa> staffVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository, IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffVisaRepository = staffVisaRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._staffRepository = staffRepository;
            this._documentFileRepository = documentFileRepository;
            _db = new DataContext();
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            var model = _staffRepository.GetAllAsQueryable().AsEnumerable().Select(p => new StaffListViewModel
            {
                Birthday = p.Birthday != null ? p.Birthday.Value.ToString("dd-MM-yyyy") : "",
                Code = p.Code,
                CreateDatePassport = p.CreatedDatePassport != null ? p.CreatedDatePassport.Value.ToString("dd-MM-yyyy") : "",
                Department = p.tbl_DictionaryDepartment.Name,
                Email = p.Email,
                ExpiredDatePassport = p.ExpiredDatePassport != null ? p.ExpiredDatePassport.Value.ToString("dd-MM-yyyy") : "",
                Fullname = p.FullName,
                Id = p.Id,
                InternalNumber = p.InternalNumber ?? 0,
                IsLock = p.IsLock,
                Passport = p.PassportCard,
                Phone = p.Phone,
                Position = p.tbl_DictionaryPosition.Name,
                Skype = p.Skype
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdStaff(int id)
        {
            Session["idStaff"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult> Create(StaffViewModel model, FormCollection form)
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

        #region Update
        /// <summary>
        /// partial view edit thông tin khách hàng
        /// </summary>
        /// <returns></returns>
        /// 
        //[ChildActionOnly]
        //public ActionResult _Partial_EditStaff()
        //{
        //    return PartialView("_Partial_EditStaff", new StaffViewModel());
        //}

        /// <summary>
        /// load thông tin khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StaffInformation(int id)
        {
            var model = new StaffViewModel();
            var staff = _staffRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            var staffVisa = _staffVisaRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.StaffId == id).ToList();
            model.SingleStaff = staff;
            model.CreatedDateIdentity = staff.CreatedDateIdentity ?? DateTime.Now;
            model.CreatedDatePassport = staff.CreatedDatePassport ?? DateTime.Now;
            model.ExpiredDatePassport = staff.ExpiredDatePassport ?? DateTime.Now;
            model.IdentityCard = staff.IdentityCard;
            model.IdentityTagId = staff.IdentityTagId ?? 0;
            model.PassportCard = staff.PassportCard;
            model.PassportTagId = staff.PassportTagId ?? 0;
            if (staffVisa.Count() > 0)
            {
                model.ListStaffVisa = staffVisa;
            }
            return PartialView("_Partial_EditStaff", model);
        }

        /// <summary>
        /// cập nhật nhân viên
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(StaffViewModel model, FormCollection form)
        {
            try
            {
                model.SingleStaff.TagsId = form["SingleStaff.TagsId"].ToString();
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

                if (await _staffRepository.Update(model.SingleStaff))
                {
                    // xóa tất cả visa của staff
                    var visaList = _staffVisaRepository.GetAllAsQueryable().Where(p => p.StaffId == model.SingleStaff.Id).ToList();
                    if (visaList.Count() > 0)
                    {
                        foreach (var v in visaList)
                        {
                            await _staffVisaRepository.Delete(v.Id, true);
                        }
                    }

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

                            UpdateHistory.SaveStaff(9, "Cập nhật nhân viên có code " + model.SingleStaff.Code);
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
                        if (await _staffRepository.DeleteMany(listIds, true))
                        {
                            UpdateHistory.SaveStaff(9, "Xóa danh sách nhân viên");
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "StaffManage") }, JsonRequestBehavior.AllowGet);
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

        #region Lock & Unlock
        [HttpPost]
        public async Task<ActionResult> LockStaff(int id)
        {
            var item = await _staffRepository.GetById(id);
            item.IsLock = true;
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UnlockStaff(int id)
        {
            var item = await _staffRepository.GetById(id);
            item.IsLock = false;
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Visa
        /********** Quản lý visa ************/
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateVisa(tbl_StaffVisa model, FormCollection form)
        {
            try
            {
                string id = Session["idStaff"].ToString();
                if (ModelState.IsValid)
                {
                    model.StaffId = Convert.ToInt32(id);
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

                    if (await _staffVisaRepository.Create(model))
                    {
                        UpdateHistory.SaveStaff(9, "Thêm mới visa cho nhân viên");
                        var list = _db.tbl_StaffVisa.AsEnumerable().Where(p => p.StaffId.ToString() == id).ToList();
                        return PartialView("~/Views/StaffTabInfo/_Visa.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditVisa()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditVisa", new tbl_StaffVisa());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoVisa(int id)
        {
            var model = await _staffVisaRepository.GetById(id);
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
        public async Task<ActionResult> UpdateVisa(tbl_StaffVisa model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.StaffId = Convert.ToInt32(model.StaffId);
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

                    if (await _staffVisaRepository.Update(model))
                    {
                        UpdateHistory.SaveStaff(9, "Cập nhật visa cho nhân viên");
                        var list = _db.tbl_StaffVisa.AsEnumerable().Where(p => p.StaffId == model.StaffId).ToList();
                        return PartialView("~/Views/StaffTabInfo/_Visa.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteVisa(int id)
        {
            try
            {
                var sId = _staffVisaRepository.FindId(id).StaffId;
                if (await _staffVisaRepository.Delete(id, true))
                {
                    UpdateHistory.SaveStaff(9, "Xóa danh sách visa của nhân viên");
                    var list = _db.tbl_StaffVisa.AsEnumerable().Where(p => p.StaffId == sId).ToList();
                    return PartialView("~/Views/StaffTabInfo/_Visa.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
                }
            }
            catch { }
            return PartialView("~/Views/StaffTabInfo/_Visa.cshtml");
        }
        #endregion

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["StaffFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                string id = Session["idStaff"].ToString();
                if (ModelState.IsValid)
                {
                    model.StaffId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    //file
                    HttpPostedFileBase FileName = Session["StaffFile"] as HttpPostedFileBase;
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
                        Session["StaffFile"] = null;
                        UpdateHistory.SaveStaff(9, "Thêm mới tài liêu, code: " + model.Code);
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId.ToString() == id)
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
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
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

                    //file
                    if (Session["StaffFile"] != null)
                    {
                        HttpPostedFileBase FileName = Session["StaffFile"] as HttpPostedFileBase;
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
                        Session["StaffFile"] = null;
                        UpdateHistory.SaveStaff(9, "Cập nhật tài liệu, code: " + model.Code);
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId == model.StaffId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId == model.StaffId)
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
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                var sId = _documentFileRepository.FindId(id).StaffId;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                if (await _documentFileRepository.Delete(id, true))
                {
                    UpdateHistory.SaveStaff(9, "Xóa danh sách tài liệu của nhân viên");
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId == sId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.StaffId == id)
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
                    return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/StaffTabInfo/_HoSoLienQuan.cshtml");
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
            var staffs = _staffRepository.GetAllAsQueryable().AsEnumerable()
                 .Select(p => new StaffListViewModel
                 {
                     Code = p.Code == null ? "" : p.Code,
                     Fullname = p.FullName == null ? "" : p.FullName,
                     Birthday = p.Birthday == null ? "" : p.Birthday.Value.ToString("dd-MM-yyyy"),
                     Address = p.Address == null ? "" : p.Address,
                     Phone = p.Phone == null ? "" : p.Phone,
                     InternalNumber = p.InternalNumber ?? 0,
                     Email = p.Email == null ? "" : p.Email,
                     Department = p.tbl_DictionaryDepartment.Name,
                     Position = p.tbl_DictionaryPosition.Name,
                     IsLock = p.IsLock
                 }).ToList();

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, staffs);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Staffs.xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }


        public virtual void ExportCustomersToXlsx(Stream stream, IList<StaffListViewModel> staffs)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Staffs");

                var properties = new[]
                    {
                        "Mã số",
                        "Họ và tên",
                        "Ngày sinh",
                        "Địa chỉ",
                        "Điện thoại",
                        "Số nội bộ",
                        "Email",
                        "Phòng bạn",
                        "Chức vụ",
                        "Nhóm kinh doanh",
                        "Chi nhánh",
                        "Khóa"
                        
                    };



                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                }


                int row = 1;
                foreach (var staff in staffs)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = staff.Code;
                    col++;

                    worksheet.Cells[row, col].Value = staff.Fullname;
                    col++;

                    worksheet.Cells[row, col].Value = staff.Birthday;
                    col++;

                    worksheet.Cells[row, col].Value = staff.Address;
                    col++;

                    worksheet.Cells[row, col].Value = staff.Phone;
                    col++;

                    worksheet.Cells[row, col].Value = staff.InternalNumber;
                    col++;

                    worksheet.Cells[row, col].Value = staff.Email;
                    col++;

                    worksheet.Cells[row, col].Value = staff.Department;
                    col++;

                    worksheet.Cells[row, col].Value = staff.Position;
                    col++;

                    worksheet.Cells[row, col].Value = string.Empty;
                    col++;

                    worksheet.Cells[row, col].Value = string.Empty;
                    col++;

                    if (staff.IsLock)
                        worksheet.Cells[row, col].Value = "Khóa";
                    else
                        worksheet.Cells[row, col].Value = string.Empty;

                    col++;

                }
                worksheet.Cells["a1:l" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));

                worksheet.Cells["a1:l1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a1:l1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a1:l1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));


                worksheet.Cells["a1:l" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:l" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:l" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:l" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:l" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:l" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:l" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:l" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));

                row++;

                worksheet.Cells["a" + row + ":l" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a" + row + ":l" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["c" + row + ":l" + row].Merge = true;
                worksheet.Cells["b" + row].Value = row - 2;

                worksheet.Cells["a1:l" + row].AutoFitColumns();

                worksheet.Column(1).Width = 12.86;
                worksheet.Column(2).Width = 21.71;
                worksheet.Column(3).Width = 9.86;
                worksheet.Column(4).Width = 27.14;
                worksheet.Column(5).Width = 11.86;
                worksheet.Column(6).Width = 11.86;
                worksheet.Column(7).Width = 20;
                worksheet.Column(8).Width = 20;
                worksheet.Column(9).Width = 20;
                worksheet.Column(10).Width = 20;
                worksheet.Column(11).Width = 20;
                worksheet.Column(12).Width = 8;

                xlPackage.Save();
            }
        }
        #endregion
    }
}

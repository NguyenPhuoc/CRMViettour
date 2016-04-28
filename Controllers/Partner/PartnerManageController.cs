using CRM.Core;
using CRM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRMViettour.Models;
using System.Threading.Tasks;
using CRMViettour.Utilities;

namespace CRMViettour.Controllers
{
    public class PartnerManageController : BaseController
    {
        //
        // GET: /PartnerManage/

        #region Init

        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_PartnerNote> _partnerNoteRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public PartnerManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_PartnerNote> partnerNoteRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._partnerNoteRepository = partnerNoteRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._partnerRepository = partnerRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tagRepository = tagRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }

        #endregion

        #region List
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListPartner()
        {
            var model = _partnerRepository.GetAllAsQueryable().AsEnumerable()
                .Select(p => new PartnerViewModel
                {
                    Code = p.Code,
                    Contact = p.StaffContact,
                    Country = p.tbl_TagsCountry == null ? "" : p.tbl_TagsCountry.Tag,
                    Email = p.Email,
                    Id = p.Id,
                    Name = p.Name,
                    Tags = p.TagsLocationId == null ? "" : LoadData.LocationTags(p.TagsLocationId)
                });
            return PartialView("_Partial_ListPartner", model);
        }

        #endregion

        #region Filter
        [HttpPost]
        public ActionResult FilterService(int id)
        {
            var model = _partnerRepository.GetAllAsQueryable().AsEnumerable()
                 .Where(p => p.DictionaryId == id).Select(p => new PartnerViewModel
                 {
                     Code = p.Code,
                     Contact = p.StaffContact,
                     Country = p.tbl_TagsCountry.Tag,
                     Email = p.Email,
                     Id = p.Id,
                     Name = p.Name,
                     Tags = LoadData.LocationTags(p.TagsLocationId)
                 });
            return PartialView("_Partial_ListPartner", model);
        }

        [HttpPost]
        public ActionResult FilterTags(string tags)
        {
            var model = new List<PartnerViewModel>();
            foreach (var item in tags.ToString().Split(','))
            {
                model.AddRange(_partnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TagsLocationId.Split(',')
                    .Contains(item)).Select(p => new PartnerViewModel
                    {
                        Code = p.Code,
                        Contact = p.StaffContact,
                        Country = p.tbl_TagsCountry.Tag,
                        Email = p.Email,
                        Id = p.Id,
                        Name = p.Name,
                        Tags = LoadData.LocationTags(p.TagsLocationId)
                    }).Distinct().ToList());
            }
            return PartialView("_Partial_ListPartner", model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Partner model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.TagsLocationId = form["TagsLocationId"].ToString();
                if (await _partnerRepository.Create(model))
                {
                    for (int i = 1; i <= Convert.ToInt32(form["countService"]); i++)
                    {
                        if (form["PartnerServiceName" + i] != null)
                        {
                            var sv = new tbl_ServicesPartner
                            {
                                Name = form["PartnerServiceName" + i].ToString(),
                                PartnerId = model.Id,
                                Price = form["PartnerServicePrice" + i] != null ? Convert.ToDouble(form["PartnerServicePrice" + i]) : 0,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                Note = form["PartnerServiceNote" + i].ToString()
                            };
                            await _servicesPartnerRepository.Create(sv);

                            UpdateHistory.SavePartner(model.Id, 9, "Thêm mới đối tác, code: " + model.Code);
                        }
                    }
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        //[ChildActionOnly]
        //public ActionResult _Partial_EditPartner()
        //{
        //    return PartialView("_Partial_EditPartner", new tbl_Partner());
        //}

        [HttpPost]
        public ActionResult PartnerInfomation(int id)
        {
            var model = _db.tbl_Partner.Find(id);
            ViewBag.Services = _servicesPartnerRepository.GetAllAsQueryable().Where(p => p.PartnerId == id).ToList();
            return PartialView("_Partial_EditPartner", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Partner model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.TagsLocationId = form["TagsLocationId"].ToString();
                if (await _partnerRepository.Update(model))
                {
                    // delete all service
                    var service = _servicesPartnerRepository.GetAllAsQueryable().Where(p => p.PartnerId == model.Id).ToList();
                    if (service.Count() > 0)
                    {
                        foreach (var item in service)
                        {
                            await _servicesPartnerRepository.Delete(item.Id, true);
                        }
                    }

                    for (int i = 1; i <= Convert.ToInt32(form["countServiceE"].Split(',')[0].ToString()); i++)
                    {
                        if (form["PartnerServiceName" + i] != null)
                        {
                            var sv = new tbl_ServicesPartner
                            {
                                Name = form["PartnerServiceName" + i].ToString(),
                                PartnerId = model.Id,
                                Price = form["PartnerServicePrice" + i] != null ? Convert.ToDouble(form["PartnerServicePrice" + i]) : 0,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                Note = form["PartnerServiceNote" + i].ToString()
                            };
                            await _servicesPartnerRepository.Create(sv);

                            UpdateHistory.SavePartner(model.Id, 9, "Cập nhật đối tác, code: " + model.Code);
                        }
                    }
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
                        if (await _partnerRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "PartnerManage") }, JsonRequestBehavior.AllowGet);
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
        /********** Quản lý tài liệu **********/

        [HttpPost]
        public ActionResult GetIdPartner(int id)
        {
            Session["idPartner"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["PartnerFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                string id = Session["idPartner"].ToString();
                if (ModelState.IsValid)
                {
                    model.PartnerId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();

                    //file
                    HttpPostedFileBase FileName = Session["PartnerFile"] as HttpPostedFileBase;
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
                        UpdateHistory.SavePartner(model.PartnerId ?? 0, 9, "Thêm mới tài liệu đối tác, code: " + model.Code);
                        Session["PartnerFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId.ToString() == id)
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
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult _Partial_EditDocument()
        {
            List<SelectListItem> lstTag = new List<SelectListItem>();
            List<SelectListItem> lstDictionary = new List<SelectListItem>();
            ViewData["TagsId"] = lstTag;
            ViewBag.DictionaryId = lstDictionary;
            return PartialView("_Partial_EditDocument", new tbl_DocumentFile());
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
                    if (Session["PartnerFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["PartnerFile"] as HttpPostedFileBase;
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
                        UpdateHistory.SavePartner(model.PartnerId ?? 0, 9, "Cập nhật tài liệu đối tác, code: " + model.Code);
                        Session["PartnerFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId == model.PartnerId)
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
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                int partnerId = _documentFileRepository.FindId(id).PartnerId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                if (await _documentFileRepository.Delete(id, true))
                {
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == partnerId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.PartnerId == partnerId)
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
                    return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/PartnerTabInfo/_TaiLieuMau.cshtml");
            }
        }

        #endregion

        #region Note
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateNote(tbl_PartnerNote model, FormCollection form)
        {
            try
            {
                string id = Session["idPartner"].ToString();
                if (ModelState.IsValid)
                {
                    model.PartnerId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.StaffId = 9;

                    if (await _partnerNoteRepository.Create(model))
                    {
                        var list = _db.tbl_PartnerNote.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).ToList();
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
        }

        [ChildActionOnly]
        public ActionResult _Partial_EditNote()
        {
            return PartialView("_Partial_EditNote", new tbl_PartnerNote());
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoNote(int id)
        {
            var model = await _partnerNoteRepository.GetById(id);
            return PartialView("_Partial_EditNote", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateNote(tbl_PartnerNote model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedDate = DateTime.Now;

                    if (await _partnerNoteRepository.Update(model))
                    {
                        var list = _db.tbl_PartnerNote.AsEnumerable().Where(p => p.PartnerId == model.PartnerId).ToList();
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteNote(int id)
        {
            try
            {
                int partnerId = _partnerNoteRepository.FindId(id).PartnerId;
                if (await _partnerNoteRepository.Delete(id, true))
                {
                    var list = _db.tbl_PartnerNote.AsEnumerable().Where(p => p.PartnerId == partnerId).ToList();
                    return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
                }
            }

            catch { }
            return PartialView("~/Views/PartnerTabInfo/_GhiChu.cshtml");
        }
        #endregion
    }
}
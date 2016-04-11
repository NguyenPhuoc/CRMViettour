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

namespace CRMViettour.Controllers.Visa
{
    public class DocumentaryVisaManageController : BaseController
    {
        // GET: DocumentaryVisa

        #region Init

        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;

        private DataContext _db;

        public DocumentaryVisaManageController(IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    model.DictionaryId = 27;

                    HttpPostedFileBase file = Request.Files["FileName"];
                    if (file != null && file.ContentLength > 0)
                    {
                        String path = Server.MapPath("~/Upload/file/" + file.FileName);
                        file.SaveAs(path);
                        model.FileName = file.FileName;
                        model.FileSize = Common.ConvertFileSize(file.ContentLength);
                    }

                    if (await _documentFileRepository.Create(model))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                    }
                }
            }
            catch { }
            return RedirectToAction("Index");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditDocumentVisa()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    return PartialView("_Partial_EditDocumentVisa", new tbl_DocumentFile());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            var model = await _documentFileRepository.GetById(id);
            return PartialView("_Partial_EditDocumentVisa", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();

                    HttpPostedFileBase file = Request.Files["FileName"];
                    if (file != null && file.ContentLength > 0)
                    {
                        String path = Server.MapPath("~/Upload/file/" + file.FileName);
                        file.SaveAs(path);
                        model.FileName = file.FileName;
                        model.FileSize = Common.ConvertFileSize(file.ContentLength);
                    }
                    else
                    {
                        var doc = _db.tbl_DocumentFile.Find(model.Id);
                        model.FileName = doc.FileName;
                        model.FileSize = doc.FileSize;
                    }

                    if (await _documentFileRepository.Update(model))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                    }
                }
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

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
                        if (await _documentFileRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "DocumentaryVisaManage") }, JsonRequestBehavior.AllowGet);
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

        [ChildActionOnly]
        public ActionResult _Partial_ListDocument()
        {
            var model = _documentFileRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.DictionaryId == 27)
                 .Select(p => new DocumentFileViewModel
                 {
                     Id = p.Id,
                     Code = p.Code,
                     ModifiedDate = p.ModifiedDate.ToString("dd/MM/yyyy"),
                     URL = "https://view.officeapps.live.com/op/embed.aspx?src=" + Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port) + "/Upload/file/" + p.FileName,
                     CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy"),
                     DictionaryName = p.tbl_Dictionary.Name,
                     FileName = p.FileName,
                     FileSize = p.FileSize,
                     Note = p.Note,
                     TagsId = LoadData.LocationTags(p.TagsId)
                 }).ToList();
            return PartialView("_Partial_ListDocument", model);
        }

        [HttpPost]
        public ActionResult SearchCountryVisa(string id)
        {
            var model = _documentFileRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TagsId.Split(',').Contains(id) && p.DictionaryId == 27)
                .Select(p => new DocumentFileViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    ModifiedDate = p.ModifiedDate.ToString("dd/MM/yyyy"),
                    URL = "https://view.officeapps.live.com/op/embed.aspx?src=" + Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port) + "/Upload/file/" + p.FileName,
                    CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy"),
                    DictionaryName = p.tbl_Dictionary.Name,
                    FileName = p.FileName,
                    FileSize = p.FileSize,
                    Note = p.Note,
                    TagsId = LoadData.LocationTags(p.TagsId)
                }).ToList();
            return PartialView("_Partial_ListDocument", model);

        }

    }
}
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

namespace CRMViettour.Controllers
{
    public class CompanyManageController : BaseController
    {
        //
        // GET: /CompanyManage/

        #region Init

        private IGenericRepository<tbl_Company> _companyRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;

        private DataContext _db;

        public CompanyManageController(IGenericRepository<tbl_Company> companyRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._companyRepository = companyRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            var company = _companyRepository.GetAllAsQueryable().Select(p => new CompanyViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                TagsId = p.TagsId,
                TaxCode = p.TaxCode,
                Note = p.Note
            });

            return View(company.ToList());
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Company model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();

                if (await _companyRepository.Create(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                }
            }

            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult _Partial_EditCompany(tbl_Company model)
        {
            List<SelectListItem> lstTag = new List<SelectListItem>();
            ViewData["TagsId"] = lstTag;
            return PartialView("_Partial_EditCompany", model);
        }

        [HttpPost]
        public ActionResult EditInfomation(int id)
        {
            var item = _db.tbl_Company.Find(id);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            foreach (var t in _db.tbl_Tags.ToList())
            {
                lstTag.Add(new SelectListItem()
                {
                    Text = t.Tag,
                    Value = t.Id.ToString(),
                    Selected = item.TagsId.Split(',').Contains(t.Id.ToString()) ? true : false
                });
            }
            ViewBag.TagsId = lstTag;

            return PartialView("_Partial_EditCompany", item);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Company model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();

                if (await _companyRepository.Update(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Dữ liệu đầu vào không đúng định dạng!");
                }
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
                        if (await _companyRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "CompanyManage") }, JsonRequestBehavior.AllowGet);
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

        /************* Thêm tài liệu *************/

        [ChildActionOnly]
        public ActionResult _Partial_CompanyDocument()
        {
            int idCompany = _companyRepository.GetAllAsQueryable().FirstOrDefault().Id;
            var model = _documentFileRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.CompanyId == idCompany)
                .Select(p => new DocumentFileViewModel
                {
                    Id = p.Id,
                    URL = "https://view.officeapps.live.com/op/embed.aspx?src=" + Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port) + "/Upload/file/" + p.FileName,
                    CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy"),
                    DictionaryName = p.tbl_Dictionary.Name,
                    FileName = p.FileName,
                    FileSize = p.FileSize,
                    Note = p.Note,
                    TagsId = LoadData.LocationTags(p.TagsId)
                }).ToList();

            return PartialView("_Partial_CompanyDocument", model);
        }

        [HttpPost]
        public ActionResult CompanyDocumentList(int id)
        {
            var model = _documentFileRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.CustomerId == id)
                .Select(p => new DocumentFileViewModel
                {
                    Id = p.Id,
                    CreatedDate = p.ModifiedDate.ToString("dd/MM/yyyy"),
                    DictionaryName = p.tbl_Dictionary.Name,
                    FileName = p.FileName,
                    FileSize = p.FileSize,
                    Note = p.Note,
                    TagsId = LoadData.LocationTags(p.TagsId),
                    URL = "https://view.officeapps.live.com/op/embed.aspx?src=" + Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port) + "/Upload/file/" + p.FileName
                }).ToList();

            return PartialView("_Partial_CompanyDocument", model);
        }

        [HttpPost]
        public ActionResult GetIdCompany(int id)
        {
            Session["id"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                string id = Session["id"].ToString();
                if (ModelState.IsValid)
                {
                    model.CompanyId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
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
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                if (await _documentFileRepository.Delete(id, true))
                {
                    return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "CompanyManage") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

    }
}

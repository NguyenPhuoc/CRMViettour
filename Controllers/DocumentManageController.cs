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
    public class DocumentManageController : BaseController
    {
        //
        // GET: /DocumentManage/

        #region Init

        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;

        private DataContext _db;

        public DocumentManageController(IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagsRepository = tagsRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            var model = _documentFileRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_DocumentFile model, FormCollection form, List<HttpPostedFileBase> FileName)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();
                model.PermissionStaff = form["PermissionStaff"].ToString();

                if (FileName != null)
                {
                    foreach (var file in FileName)
                    {
                        String path = Server.MapPath("~/Upload/file/" + file.FileName);
                        file.SaveAs(path);
                        model.FileName = file.FileName;
                        model.FileSize = Common.ConvertFileSize(file.ContentLength);

                        await _documentFileRepository.Create(model);
                    }
                }
                else
                {
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
        //    return PartialView("_Partial_EditDocument", new tbl_DocumentFile());
        //}

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            var model = await _documentFileRepository.GetById(id);
            return PartialView("_Partial_EditDocument", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                model.IsRead = true;
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();
                model.PermissionStaff = form["PermissionStaff"].ToString();

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
                        if (await _documentFileRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "DocumentManage") }, JsonRequestBehavior.AllowGet);
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
        public ActionResult _Partial_TabInfoDocument()
        {
            var model = new List<tbl_Staff>();
            var item = _documentFileRepository.GetAllAsQueryable().FirstOrDefault();
            if (item != null)
            {
                foreach (var i in item.PermissionStaff.Split(','))
                {
                    model.Add(_db.tbl_Staff.Find(Convert.ToInt32(i)));
                }
            }

            return PartialView("_Partial_TabInfoDocument", model);
        }

        public ActionResult TabInfoDocument(int id)
        {
            var model = new List<tbl_Staff>();
            var item = _documentFileRepository.GetAllAsQueryable().FirstOrDefault(p => p.Id == id);
            if (item != null)
            {
                foreach (var i in item.PermissionStaff.Split(','))
                {
                    model.Add(_db.tbl_Staff.Find(Convert.ToInt32(i)));
                }
            }

            return PartialView("_Partial_TabInfoDocument", model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_AddStaffDocument()
        {
            return PartialView ("_Partial_AddStaffDocument", new List<tbl_Staff>());
        }
    }
}

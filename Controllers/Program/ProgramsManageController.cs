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
    public class ProgramsManageController : BaseController
    {
        //
        // GET: /ProgramsManage/

        #region Init

        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private DataContext _db;

        public ProgramsManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._programRepository = programRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            _db = new DataContext();
        }

        #endregion

        #region GetIdProgram
        [HttpPost]
        public ActionResult GetIdProgram(int id)
        {
            Session["idProgram"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region List

        public ActionResult Index()
        {
            var model = _programRepository.GetAllAsQueryable().ToList();
            return View(model);
        }

        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Program model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Permission = "";
                model.DictionaryId = 30;
                model.StaffId = 9;
                model.TagsId = form["TagsId"].ToString();

                if (await _programRepository.Create(model))
                {
                    UpdateHistory.SaveProgram(model.Id, 9, "Thêm mới chương trình, code: " + model.Code);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        //[ChildActionOnly]
        //public ActionResult _Partial_EditProgram()
        //{
        //    return PartialView("_Partial_EditProgram", new tbl_Program());
        //}

        [HttpPost]
        public ActionResult ProgramInfomation(int id)
        {
            var model = _db.tbl_Program.Find(id);
            return PartialView("_Partial_EditProgram", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Program model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.StaffId = 9;
                model.TagsId = form["TagsId"].ToString();
                if (await _programRepository.Update(model))
                {
                    UpdateHistory.SaveProgram(model.Id, 9, "Cập nhật chương trình, code: " + model.Code);
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
                        if (await _programRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ProgramsManage") }, JsonRequestBehavior.AllowGet);
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
                Session["ProgramFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                string id = Session["idProgram"].ToString();
                if (ModelState.IsValid)
                {
                    model.ProgramId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    model.StaffId = 9;
                    //file
                    HttpPostedFileBase FileName = Session["ProgramFile"] as HttpPostedFileBase;
                    string CustomerFileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    if (newName != null && CustomerFileSize != null)
                    {
                        model.FileName = newName;
                        model.FileSize = CustomerFileSize;
                    }

                    if (await _documentFileRepository.Create(model))
                    {
                        Session["ProgramFile"] = null;
                        CustomerFileSize = null;
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId.ToString() == id).ToList();
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
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

                    if (Session["ProgramFile"] != null && Session["CustomerFileSize"] != null)
                    {
                        model.FileName = Session["CustomerFile"].ToString();
                        model.FileSize = Session["CustomerFileSize"].ToString();
                    }

                    if (await _documentFileRepository.Update(model))
                    {
                        Session["ProgramFile"] = null;
                        Session["CustomerFileSize"] = null;
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == model.ProgramId).ToList();
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                int proId = _documentFileRepository.FindId(id).ProgramId ?? 0;

                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                if (await _documentFileRepository.Delete(id, true))
                {
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == proId).ToList();
                    return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
            }
        }

        #endregion
    }
}

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
    public class ContractsManageController : BaseController
    {
        //
        // GET: /ContractsManage/

        #region Init

        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private DataContext _db;

        public ContractsManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._contractRepository = contractRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            _db = new DataContext();
        }

        #endregion

        #region List

        public ActionResult Index()
        {
            var model = _contractRepository.GetAllAsQueryable().ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdContract(int id)
        {
            Session["idContract"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Contract model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Permission = "";
                model.DictionaryId = 28;
                model.StaffId = 9;
                model.TagsId = "";

                if (await _contractRepository.Create(model))
                {
                    UpdateHistory.SavePartner(model.Id, 9, "Thêm mới hợp đồng, code: " + model.Code);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update


        [HttpPost]
        public ActionResult ContractInfomation(int id)
        {
            var model = _db.tbl_Contract.Find(id);
            return PartialView("_Partial_EditContract", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Contract model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.StaffId = 9;
                model.TagsId = "";
                if (await _contractRepository.Update(model))
                {
                    UpdateHistory.SavePartner(model.Id, 9, "Cập nhật hợp đồng, code: " + model.Code);
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
                        if (await _contractRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ContractsManage") }, JsonRequestBehavior.AllowGet);
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

    }
}

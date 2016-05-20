using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Staff
{
    public class HeadquarterManageController : BaseController
    {
        // GET: HeadquarterManage
        #region Init

        private IGenericRepository<tbl_Headquater> _headquaterRepository;

        private DataContext _db;

        public HeadquarterManageController(IGenericRepository<tbl_Headquater> headquaterRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._headquaterRepository = headquaterRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            var headquarter = _headquaterRepository.GetAllAsQueryable();
            return View(headquarter.ToList());
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Headquater model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;

                if (await _headquaterRepository.Create(model))
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

        //[ChildActionOnly]
        //public ActionResult _Partial_EditHeadquarter(tbl_Headquater model)
        //{
        //    return PartialView("_Partial_EditHeadquarter", model);
        //}

        [HttpPost]
        public ActionResult EditInfomation(int id)
        {
            var item = _db.tbl_Headquater.Find(id);
            return PartialView("_Partial_EditHeadquarter", item);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Headquater model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;

                if (await _headquaterRepository.Update(model))
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
                        if (await _headquaterRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "HeadquarterManage") }, JsonRequestBehavior.AllowGet);
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

    }
}
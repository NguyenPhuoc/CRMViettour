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
    public class ReviewTourController : BaseController
    {
        //
        // GET: /ReviewTour/

        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_ReviewTourDetail> _reviewTourDetailRepository;
        private DataContext _db;

        public ReviewTourController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_ReviewTourDetail> reviewTourDetailRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._dictionaryRepository = dictionaryRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tourRepository = tourRepository;
            this._reviewTourRepository = reviewTourRepository;
            this._reviewTourDetailRepository = reviewTourDetailRepository;
            _db = new DataContext();
        }

        #endregion

        #region List
        public ActionResult Index()
        {
            var model = _reviewTourRepository.GetAllAsQueryable();
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListReviewDetail()
        {
            var item = _reviewTourRepository.GetAllAsQueryable().FirstOrDefault();
            if (item != null)
            {
                var model = _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == item.Id).ToList() ?? null;
                return PartialView("_Partial_ListReviewDetail", model);
            }
            else
            {
                return PartialView("_Partial_ListReviewDetail");
            }
        }

        [HttpPost]
        public ActionResult ListReviewDetail(int id)
        {
            var model = _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == id).Where(p => p.IsDelete == false).ToList();
            return PartialView("_Partial_ListReviewDetail", model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_ReviewTour model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                int count = Convert.ToInt32(form["countService"].ToString());
                double mark = 0;
                for (int i = 1; i <= count; i++)
                {
                    mark += Convert.ToInt32(form["Mark" + i].ToString());
                }
                model.AverageMark = mark / count;

                if (await _reviewTourRepository.Create(model))
                {
                    for (int i = 1; i <= count; i++)
                    {
                        var detail = new tbl_ReviewTourDetail
                        {
                            CreatedDate = DateTime.Now,
                            DictionaryId = Convert.ToInt32(form["DictionaryId" + i]),
                            Mark = Convert.ToInt32(form["Mark" + i].ToString()),
                            ModifiedDate = DateTime.Now,
                            ReviewTourId = model.Id
                        };
                        await _reviewTourDetailRepository.Create(detail);
                    }
                }
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        //[ChildActionOnly]
        //public ActionResult _Partial_EditMark()
        //{
        //    ViewBag.Detail = new List<tbl_ReviewTourDetail>();
        //    return PartialView("_Partial_EditMark", new tbl_ReviewTour());
        //}

        [HttpPost]
        public ActionResult ReviewTourInfomation(int id)
        {
            var model = _reviewTourRepository.GetAllAsQueryable().FirstOrDefault();
            ViewBag.Detail = _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == id).Where(p => p.IsDelete == false).ToList();
            return PartialView("_Partial_EditMark", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_ReviewTour model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                int count = Convert.ToInt32(form["countServiceE"].ToString());
                double mark = 0;
                for (int i = 1; i <= count; i++)
                {
                    mark += Convert.ToInt32(form["Mark" + i].ToString());
                }
                model.AverageMark = mark / count;

                if (await _reviewTourRepository.Update(model))
                {
                    // delete
                    foreach (var item in _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == model.Id))
                    {
                        await _reviewTourDetailRepository.Delete(item.Id, false);
                    }

                    // insert
                    for (int i = 1; i <= count; i++)
                    {
                        var detail = new tbl_ReviewTourDetail
                        {
                            CreatedDate = DateTime.Now,
                            DictionaryId = Convert.ToInt32(form["DictionaryId" + i]),
                            Mark = Convert.ToInt32(form["Mark" + i].ToString()),
                            ModifiedDate = DateTime.Now,
                            ReviewTourId = model.Id
                        };
                        await _reviewTourDetailRepository.Create(detail);
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
                        if (await _reviewTourRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ReviewTour") }, JsonRequestBehavior.AllowGet);
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

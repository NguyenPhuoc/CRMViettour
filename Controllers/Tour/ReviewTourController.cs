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
    [Authorize]
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
        int SDBID = 6;
        int maPB = 0, maNKD = 0, maNV = 0, maCN = 0;
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId && p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsImport = list.Contains(4);
            ViewBag.IsExport = list.Contains(5);
            ViewBag.IsLock = list.Contains(6);
            ViewBag.IsUnLock = list.Contains(7);

            var ltAccess = _db.tbl_AccessData.Where(p => p.PermissionId == PermissionsId && p.FormId == formId).Select(p => p.ShowDataById).FirstOrDefault();
            if (ltAccess != 0)
                this.SDBID = ltAccess;

            switch (SDBID)
            {
                case 2: maPB = clsPermission.GetUser().DepartmentID;
                    maCN = clsPermission.GetUser().BranchID;
                    break;
                case 3: maNKD = clsPermission.GetUser().GroupID;
                    maCN = clsPermission.GetUser().BranchID; break;
                case 4: maNV = clsPermission.GetUser().StaffID; break;
                case 5: maCN = clsPermission.GetUser().BranchID; break;
            }
        }
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 25);

            if (SDBID == 6)
                return View(new List<tbl_ReviewTour>());

            var model = _reviewTourRepository.GetAllAsQueryable();
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListReviewDetail()
        {
            var item = _reviewTourRepository.GetAllAsQueryable().FirstOrDefault();
            if (item != null)
            {
                var model = _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == item.Id && p.IsDelete == false).ToList() ?? null;
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
            var model = _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == id && p.IsDelete == false).ToList();
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
            ViewBag.Detail = _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == id && p.IsDelete == false).ToList();
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
                    foreach (var item in _reviewTourDetailRepository.GetAllAsQueryable().Where(p => p.ReviewTourId == model.Id && p.IsDelete == false))
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

using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Other
{
    [Authorize]
    public class LocationTagsManageController : BaseController
    {
        // GET: TagsManage
        #region Init

        private IGenericRepository<tbl_Tags> _locationRepository;
        private DataContext _db;

        public LocationTagsManageController(IGenericRepository<tbl_Tags> locationRepository, IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._locationRepository = locationRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            var location = _locationRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).Where(p => p.TypeTag <= 5).Select(p => new TagsViewModel
            {
                Id = p.Id,
                Tags = p.Tag,
                ParentId = p.ParentId,
                IsoCode = p.ISOCode
            }).ToList();

            var model = new SeededTagsViewModel { Seed = 0, Tags = location };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            var item = new tbl_Tags
            {
                ParentId = Convert.ToInt32(form["ddlTags"].ToString()),
                TypeTag = Convert.ToInt32(form["ddlTagsType"].ToString()),
                Tag = form["txtTag"].ToString(),
                ISOCode = form["txtIsoCode"].ToString()
            };

            _db.tbl_Tags.Add(item);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string isocode)
        {
            var item = _db.tbl_Tags.Find(id);
            item.Tag = name;
            item.ISOCode = isocode;
            _db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string[] checkArray)
        {
            var check = checkArray;
            for (int i = 0; i < check.Count(); i++)
            {
                var item = _db.tbl_Tags.Find(Convert.ToInt32(check[i]));
                var listChild = _db.tbl_Tags.Where(p => p.ParentId == item.Id).Where(p => p.IsDelete == false).ToList();
                if (listChild.Count() == 0)
                {
                    _db.tbl_Tags.Remove(item);
                    _db.SaveChanges();
                }
            }
            return Json("Xóa dữ liệu thành công!", JsonRequestBehavior.AllowGet);
        }

        #region Load TagByType
        public JsonResult LoadTagByType(int id)
        {
            var model = _locationRepository.GetAllAsQueryable().Where(p => p.IsDelete == false && p.TypeTag == id).ToList();
            return Json(new SelectList(model, "Id", "Tag"), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
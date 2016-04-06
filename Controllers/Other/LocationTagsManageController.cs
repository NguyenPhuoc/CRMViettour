﻿using CRM.Core;
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
            var location = _locationRepository.GetAllAsQueryable().Where(p=>p.TypeTag <= 5).Select(p => new TagsViewModel
            {
                Id = p.Id,
                Tags = p.Tag,
                ParentId = p.ParentId
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
            };

            _db.tbl_Tags.Add(item);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Update(int id, string name)
        {
            var item = _db.tbl_Tags.Find(id);
            item.Tag = name;
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
                var listChild = _db.tbl_Tags.Where(p => p.ParentId == item.Id).ToList();
                if (listChild.Count() == 0)
                {
                    _db.tbl_Tags.Remove(item);
                    _db.SaveChanges();
                }
            }
            return Json("Xóa dữ liệu thành công !", JsonRequestBehavior.AllowGet);
        }
    }
}
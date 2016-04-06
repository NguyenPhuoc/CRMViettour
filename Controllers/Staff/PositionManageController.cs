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
    public class PositionManageController : BaseController
    {
        // GET: PositionManage
        #region Init

        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private DataContext _db;

        public PositionManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository, IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._dictionaryRepository = dictionaryRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            var dictionary = _dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 5)
                .Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                });

            return View(dictionary.ToList());
        }

        [HttpPost]
        public ActionResult SaveData(int id, string name)
        {
            try
            {
                if (id == 0) // insert
                {
                    tbl_Dictionary dictionary = new tbl_Dictionary()
                    {
                        Name = name,
                        DictionaryCategoryId = 5
                    };

                    _db.tbl_Dictionary.Add(dictionary);
                }
                else // update
                {
                    var item = _db.tbl_Dictionary.Find(id);
                    item.Name = name;
                }

                _db.SaveChanges();
            }
            catch
            {
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(FormCollection fc)
        {
            if (fc["listItemId"] != null && fc["listItemId"] != "")
            {
                var listIds = fc["listItemId"].Split(',');
                listIds = listIds.Take(listIds.Count() - 1).ToArray();
                if (listIds.Count() > 0)
                {
                    if (await _dictionaryRepository.DeleteMany(listIds, true))
                    {
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "PostionManage") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Xóa dữ liệu thất bại !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn những mục cần xóa !" }, JsonRequestBehavior.AllowGet);
        }
    }
}
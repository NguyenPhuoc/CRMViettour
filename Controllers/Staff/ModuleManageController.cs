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

namespace CRMViettour.Controllers.Staff
{
    [Authorize]
    public class ModuleManageController : BaseController
    {
        // GET: ModuleManage
        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_Module> _moduleRepository;
        private IGenericRepository<tbl_Function> _functionRepository;
        private IGenericRepository<tbl_Form> _formRepository;
        private IGenericRepository<tbl_FormFunction> _formFunctionRepository;
        private DataContext _db;

        public ModuleManageController(IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_StaffVisa> staffVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Module> moduleRepository,
            IGenericRepository<tbl_Function> functionRepository,
            IGenericRepository<tbl_Form> formRepository,
            IGenericRepository<tbl_FormFunction> formFunctionRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffVisaRepository = staffVisaRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._staffRepository = staffRepository;
            this._documentFileRepository = documentFileRepository;
            this._tagsRepository = tagsRepository;
            this._taskRepository = taskRepository;
            this._tourRepository = tourRepository;
            this._moduleRepository = moduleRepository;
            this._functionRepository = functionRepository;
            this._formRepository = formRepository;
            this._formFunctionRepository = formFunctionRepository;
            _db = new DataContext();
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            int perID = clsPermission.GetUser().PermissionID;
            var list = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            var model = _moduleRepository.GetAllAsQueryable().AsEnumerable().ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult GetIdModule(int id)
        {
            Session["idModule"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Form Info
        [ChildActionOnly]
        public ActionResult _Partial_FormList()
        {
            return PartialView("_Partial_FormList");
        }

        [HttpPost]
        public async Task<ActionResult> InfoForm(int id)
        {
            int perID = clsPermission.GetUser().PermissionID;
            var list = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            Session["idModule"] = id;
            var model = _formRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.ModuleId == id && c.IsDelete == false).ToList();
            return PartialView("_Partial_FormList", model);
        }
        #endregion

        #region Function Info
        [ChildActionOnly]
        public ActionResult _Partial_FunctionList()
        {
            return PartialView("_Partial_FunctionList");
        }

        [HttpPost]
        public async Task<ActionResult> InfoFunction(int id)
        {
            int perID = clsPermission.GetUser().PermissionID;
            var list = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
            ViewBag.IsDelete = list.Contains(2);
            Session["idForm"] = id;
            var model = _formFunctionRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.FormId == id && c.IsDelete == false).Select(c => new FunctionViewModel
            {
                Id = c.Id,
                Name = c.tbl_Function.Name
            }).ToList();
            return PartialView("_Partial_FunctionList", model);
        }
        #endregion

        #region Module
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> InsertModule(tbl_Module model)
        {
            try
            {
                await _moduleRepository.Create(model);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditModule(int id)
        {
            var model = _moduleRepository.FindId(id);
            return PartialView("_Partial_EditModule", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateModule(tbl_Module model)
        {
            try
            {
                await _moduleRepository.Update(model);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteModule(FormCollection fc)
        {
            try
            {
                if (fc["listItemId"] != null && fc["listItemId"] != "")
                {
                    var listIds = fc["listItemId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        if (await _moduleRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ModuleManage") }, JsonRequestBehavior.AllowGet);
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

        #region Form
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> InsertForm(tbl_Form model)
        {
            try
            {
                int perID = clsPermission.GetUser().PermissionID;
                var lists = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
                ViewBag.IsDelete = lists.Contains(2);
                ViewBag.IsEdit = lists.Contains(3);
                int idModule = Int16.Parse(Session["idModule"].ToString());
                model.ModuleId = idModule;
                await _formRepository.Create(model);

                var list = _formRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.ModuleId == idModule).Where(p => p.IsDelete == false).ToList();
                return PartialView("_Partial_FormList", list);
            }
            catch
            {
                return PartialView("_Partial_FormList");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditForm(int id)
        {
            var model = _formRepository.FindId(id);
            return PartialView("_Partial_EditForm", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateForm(tbl_Form model)
        {
            try
            {
                int perID = clsPermission.GetUser().PermissionID;
                var lists = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
                ViewBag.IsDelete = lists.Contains(2);
                ViewBag.IsEdit = lists.Contains(3);
                await _formRepository.Update(model);
                var list = _formRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.ModuleId == model.ModuleId && c.IsDelete == false).ToList();
                return PartialView("_Partial_FormList", list);
            }
            catch
            {
                return PartialView("_Partial_FormList");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteForm(int id)
        {
            try
            {
                int perID = clsPermission.GetUser().PermissionID;
                var lists = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
                ViewBag.IsDelete = lists.Contains(2);
                ViewBag.IsEdit = lists.Contains(3);
                var form = _formRepository.FindId(id);
                form.IsDelete = true;
                if (await _formRepository.Update(form))
                {
                    var model = _formRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.ModuleId == form.ModuleId && c.IsDelete == false).ToList();
                    return PartialView("_Partial_FormList", model);
                }
                return PartialView("_Partial_FormList");
            }
            catch
            {
                return PartialView("_Partial_FormList");
            }
        }
        #endregion

        #region Function
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> InsertFunction(tbl_FormFunction model)
        {
            try
            {
                int perID = clsPermission.GetUser().PermissionID;
                var lists = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
                ViewBag.IsDelete = lists.Contains(2);
                int idForm = Int16.Parse(Session["idForm"].ToString());
                model.FormId = idForm;
                await _formFunctionRepository.Create(model);
                _db = new DataContext();
                var list = _db.tbl_FormFunction.AsEnumerable().Where(p => p.IsDelete == false).Where(c => c.FormId == idForm).Select(c => new FunctionViewModel
                {
                    Id = c.Id,
                    Name = c.tbl_Function.Name
                }).ToList();
                return PartialView("_Partial_FunctionList", list);
            }
            catch
            {
                return PartialView("_Partial_FunctionList");
            }
        }

        public JsonResult LoadFunction()
        {
            int idForm = Int16.Parse(Session["idForm"].ToString());
            var lst = _functionRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.IsDelete == false).Select(p => new tbl_Function { Id = p.Id, Name = p.Name }).ToList();
            var formfunc = _formFunctionRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.FormId == idForm).Where(p => p.IsDelete == false).Select(c => c.FunctionId).ToList();
            foreach (var i in formfunc)
            {
                foreach (var item in lst)
                {
                    if (item.Id == i)
                    {
                        lst.Remove(item);
                        break;
                    }
                }
            }
            var funcList = from e in lst
                           select new
                           {
                               id = e.Id,
                               name = e.Name,
                           };
            var obj = funcList.ToArray();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteFunction(int id)
        {
            try
            {
                int perID = clsPermission.GetUser().PermissionID;
                var list = _db.tbl_ActionData.Where(p => p.FormId == 12 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
                ViewBag.IsDelete = list.Contains(2);
                int idForm = _formFunctionRepository.FindId(id).FormId;
                if (await _formFunctionRepository.Delete(id, false))
                {
                    var model = _formFunctionRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.FormId == idForm).Where(p => p.IsDelete == false).Select(c => new FunctionViewModel
                    {
                        Id = c.Id,
                        Name = c.tbl_Function.Name
                    }).ToList();
                    return PartialView("_Partial_FunctionList", model);
                }
                return PartialView("_Partial_FunctionList");
            }
            catch
            {
                return PartialView("_Partial_FunctionList");
            }
        }
        #endregion
    }
}
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

namespace CRMViettour.Controllers.Permission
{
    public class PermissionManageController : BaseController
    {
        // GET: PermissionManage

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
        private IGenericRepository<tbl_Permissions> _permissionsRepository;
        private IGenericRepository<tbl_AccessData> _accessDataRepository;
        private IGenericRepository<tbl_ActionData> _actionDataRepository;
        private DataContext _db;

        public PermissionManageController(IGenericRepository<tbl_Staff> staffRepository,
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
            IGenericRepository<tbl_Permissions> permissionsRepository,
            IGenericRepository<tbl_AccessData> accessDataRepository,
            IGenericRepository<tbl_ActionData> actionDataRepository,
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
            this._permissionsRepository = permissionsRepository;
            this._accessDataRepository = accessDataRepository;
            this._actionDataRepository = actionDataRepository;
            _db = new DataContext();
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            int perID = clsPermission.GetUser().PermissionID;
            var list = _db.tbl_ActionData.Where(p => p.FormId == 11 && p.PermissionsId == perID).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsEdit = list.Contains(3);
            var model = _permissionsRepository.GetAllAsQueryable().AsEnumerable().ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult GetIdPermission(int id)
        {
            Session["idPermission"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Permissions model)
        {
            try
            {
                await _permissionsRepository.Create(model);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Update
        [HttpPost]
        public async Task<ActionResult> Edit(int id)
        {
            var model = _permissionsRepository.FindId(id);
            return PartialView("_Partial_EditRoleGroup", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Permissions model)
        {
            try
            {
                await _permissionsRepository.Update(model);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
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
                        if (await _permissionsRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "PermissionManage") }, JsonRequestBehavior.AllowGet);
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

        #region AddUser
        [HttpPost]
        public async Task<ActionResult> AddUser(int id)
        {
            Session["idPermission"] = id;
            var model = _staffRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.PermissionId == null).Select(c => new StaffListViewModel
            {
                Id = c.Id,
                Code = c.Code,
                Fullname = c.FullName,
                Birthday = c.Birthday != null ? c.Birthday.Value.ToString("dd-MM-yyyy") : "",
                Address = c.Address,
                Department = c.DepartmentId != null ? c.tbl_DictionaryDepartment.Name : "",
                Position = c.PositionId != null ? c.tbl_DictionaryPosition.Name : "",
            }).ToList();
            return PartialView("_Partial_AddUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InsertUser(FormCollection fc)
        {
            try
            {
                int idPermission = Int16.Parse(Session["idPermission"].ToString());
                if (fc["listItemIdAdd"] != null && fc["listItemIdAdd"] != "")
                {
                    var listIds = fc["listItemIdAdd"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var id in listIds)
                        {
                            int _id = Int16.Parse(id);
                            var staff = _staffRepository.FindId(_id);
                            staff.PermissionId = idPermission;
                            await _staffRepository.Update(staff);
                        }
                        return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Lưu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "PermissionManage") }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Vui lòng chọn người dùng !" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ActionModel() { Succeed = false, Code = "200", View = "", Message = "Lưu không thành công !" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region TabInfo

        #region Người Dùng
        [ChildActionOnly]
        public ActionResult _NguoiDung()
        {
            return PartialView("_NguoiDung");
        }

        [HttpPost]
        public async Task<ActionResult> InfoNguoiDung(int id)
        {
            var model = _staffRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.PermissionId == id).Select(c => new StaffListViewModel
            {
                Id = c.Id,
                Code = c.Code,
                Fullname = c.FullName,
                Birthday = c.Birthday != null ? c.Birthday.Value.ToString("dd-MM-yyyy") : "",
                Address = c.Address,
                Department = c.DepartmentId != null ? c.tbl_DictionaryDepartment.Name : "",
                Position = c.PositionId != null ? c.tbl_DictionaryPosition.Name : "",
            }).ToList();
            return PartialView("_NguoiDung", model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var staff = _staffRepository.FindId(id);
            int idP = staff.PermissionId ?? 0;
            staff.PermissionId = null;
            await _staffRepository.Update(staff);
            _db = new DataContext();
            var model = _db.tbl_Staff.AsEnumerable().Where(c => c.PermissionId == idP).Select(c => new StaffListViewModel
            {
                Id = c.Id,
                Code = c.Code,
                Fullname = c.FullName,
                Birthday = c.Birthday != null ? c.Birthday.Value.ToString("dd-MM-yyyy") : "",
                Address = c.Address,
                Department = c.DepartmentId != null ? c.tbl_DictionaryDepartment.Name : "",
                Position = c.PositionId != null ? c.tbl_DictionaryPosition.Name : "",
            }).ToList();
            return PartialView("_NguoiDung", model);
        }
        #endregion

        #endregion

        #region Json Function
        public JsonResult JsonFunction(int id)
        {
            Session["idForm"] = id;
            int idPermission = Int16.Parse(Session["idPermission"].ToString());
            var actions = _actionDataRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.FormId == id && c.PermissionsId == idPermission).Select(c => c.FunctionId).ToList();
            var model = _formFunctionRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.FormId == id)
               .Select(p => new tbl_Function
               {
                   Id = p.tbl_Function.Id,
                   Name = p.tbl_Function.Name
               }).ToList();

            var lst = from e in model
                      select new
                      {
                          id = e.Id,
                          name = e.Name,
                          ckeck = ckeck(e.Id, actions)
                      };

            var funcs = lst.ToArray();
            return Json(funcs, JsonRequestBehavior.AllowGet);
        }
        private bool ckeck(int id, List<int> actions)
        {
            foreach (var item in actions)
            {
                if (id == item)
                    return true;
            }
            return false;
        }
        public JsonResult JsonShowDataBy(int id)
        {
            var acss = _accessDataRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.FormId == id).Select(c => c.ShowDataById).FirstOrDefault();

            var _acss = new
            {
                id = acss
            };
            return Json(_acss, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Lưu thiết lập
        [HttpPost]
        public async Task<ActionResult> SaveSetupRole(int idDataBy, string lst)
        {
            try
            {
                var _listIdFuncs = lst.Split(',');
                _listIdFuncs = _listIdFuncs.Take(_listIdFuncs.Count() - 1).ToArray();
                int idPermission = Int32.Parse(Session["idPermission"].ToString());
                var listIdFuncs = new List<string>();
                foreach (var item in _listIdFuncs)
                {
                    listIdFuncs.Add(item);
                }
                int idForm = Int32.Parse(Session["idForm"].ToString());

                var accs = _accessDataRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.FormId == idForm).FirstOrDefault();
                if (accs != null)
                {
                    accs.ShowDataById = idDataBy;
                    accs.PermissionId = idPermission;
                    await _accessDataRepository.Update(accs);
                }
                else
                {
                    var _accs = new tbl_AccessData()
                    {
                        FormId = idForm,
                        PermissionId = idPermission,
                        ShowDataById = idDataBy
                    };
                    await _accessDataRepository.Create(_accs);
                }
                var actis = _actionDataRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.PermissionsId == idPermission && c.FormId == idForm).ToList();
                if (actis.Count == 0)
                {
                    foreach (var _id in listIdFuncs)
                    {
                        int idFunc = Int16.Parse(_id);
                        var _acti = new tbl_ActionData()
                        {
                            FormId = idForm,
                            PermissionsId = idPermission,
                            FunctionId = idFunc
                        };
                        await _actionDataRepository.Create(_acti);
                    }
                }
                else //else
                {
                    foreach (var item in actis)
                    {
                        await _actionDataRepository.Delete(item.Id, true);
                    }
                    foreach (var _id in listIdFuncs)
                    {
                        int idFunc = Int16.Parse(_id);
                        var _acti = new tbl_ActionData()
                        {
                            FormId = idForm,
                            PermissionsId = idPermission,
                            FunctionId = idFunc
                        };
                        await _actionDataRepository.Create(_acti);
                    }
                }
                //else
                //{
                //    //danh sách đã tồn tại Length luôn bé hơn hoặc bằng Lenght của listIdFuncs
                //    var lstIs = new List<string>();
                //    foreach (var item in actis)
                //    {
                //        bool temp = false;
                //        foreach (var _id in listIdFuncs)
                //        {
                //            int idFunc = Int16.Parse(_id);
                //            if (item.FunctionId == idFunc)
                //            {
                //                temp = true;
                //                lstIs.Add(idFunc.ToString());
                //                break;
                //            }
                //        }
                //        if (!temp)
                //        {
                //            await _actionDataRepository.Delete(item.Id, true);
                //        }
                //    }
                //    if (lstIs.Count != listIdFuncs.Count)
                //    {
                //        int count = listIdFuncs.Count;
                //        for (int i = 0; i < count; i++)
                //        {
                //            foreach (var id in lstIs)
                //            {
                //                if (listIdFuncs[i] == id)
                //                {
                //                    listIdFuncs.RemoveAt(i);
                //                    count--;
                //                    break;
                //                }
                //            }
                //        }
                //        foreach (var _id in listIdFuncs)
                //        {
                //            int idFunc = Int16.Parse(_id);
                //            var _acti = new tbl_ActionData()
                //            {
                //                FormId = idForm,
                //                PermissionsId = idPermission,
                //                FunctionId = idFunc
                //            };
                //            await _actionDataRepository.Create(_acti);
                //        }
                //    }
                //}
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
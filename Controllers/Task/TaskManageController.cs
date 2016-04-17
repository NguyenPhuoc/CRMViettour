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
    public class TaskManageController : BaseController
    {
        //
        // GET: /TaskManage/

        #region Init

        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_TaskStaff> _taskstaffRepository;
        private DataContext _db;

        public TaskManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_TaskStaff> taskstaffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._taskRepository = taskRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            this._staffRepository = staffRepository;
            this._tourRepository = tourRepository;
            this._customerRepository = customerRepository;
            this._taskstaffRepository = taskstaffRepository;
            _db = new DataContext();
        }

        #endregion

        #region GetIdProgram
        [HttpPost]
        public ActionResult GetIdTask(int id)
        {
            Session["idTask"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region List
        public ActionResult Index()
        {
            var model = _taskRepository.GetAllAsQueryable().AsEnumerable().Select(p => new tbl_Task
            {
                Id = p.Id,
                CodeTour = p.CodeTour != null ? p.CodeTour : "",
                Name = p.Name,
                tbl_Staff = _staffRepository.FindId(p.StaffId),
                tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                Email = p.Email != null ? p.Email : "",
                Phone = p.Phone != null ? p.Phone : "",
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Time = p.Time


            }).ToList();
            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Task model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = 9;
                model.Code = new Random().Next(1000, 9999).ToString();
                model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                var cus = _customerRepository.FindId(model.CustomerId);
                model.Email = cus.PersonalEmail != null ? cus.PersonalEmail : cus.CompanyEmail != null ? cus.CompanyEmail : null;
                model.Phone = cus.Phone != null ? cus.Phone : null;

                if (await _taskRepository.Create(model))
                {
                    /// UpdateHistory.....(model.Id, 9, "Thêm mới nhiệm vụ, code: " + model.Code);
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
        public ActionResult TaskInfomation(int id)
        {
            var model = _db.tbl_Task.Find(id);
            return PartialView("_Partial_EditTask", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Task model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.CodeTour = _tourRepository.FindId(model.TourId).Code;
                var cus = _customerRepository.FindId(model.CustomerId);
                model.Email = cus.PersonalEmail != null ? cus.PersonalEmail : cus.CompanyEmail != null ? cus.CompanyEmail : null;
                model.Phone = cus.Phone != null ? cus.Phone : null;
                if (await _taskRepository.Update(model))
                {
                    //UpdateHistory.....(model.Id, 9, "Cập nhật nhiệm vụ, code: " + model.Code);
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
                        if (await _taskRepository.DeleteMany(listIds, true))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "TaskManage") }, JsonRequestBehavior.AllowGet);
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

        #region CreateAssign
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAssign(tbl_TaskStaff model, FormCollection form)
        {
            try
            {
                int id = Int32.Parse(Session["idTask"].ToString());

                for (int i = 1; i <= Convert.ToInt32(form["countAssign"]); i++)
                {
                    if (form["Customer" + i] != null && form["Customer" + i].ToString() != "")
                    {
                        var ts = new tbl_TaskStaff
                        {
                            TaskId = id,
                            IsUse = true,
                            CreateStaffId = 9,
                            CreateDate = DateTime.Now,
                            StaffId = Int32.Parse(form["Customer" + i].ToString()),
                            Role = form["Role" + i].ToString(),
                            Note = form["Note" + i].ToString(),
                        };
                        await _taskstaffRepository.Create(ts);
                    }
                    //UpdateHistory.SavePartner(model.Id, 9, "Thêm mới đối tác, code: " + model.Code);

                }
            }
            catch { }
            return RedirectToAction("Index");
        }
        #endregion
    }
}

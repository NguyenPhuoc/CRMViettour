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

namespace CRMViettour.Controllers.Appointment
{
    public class AppointmentManageController : BaseController
    {
        // GET: AppointmentManage
        #region Init

        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;

        private DataContext _db;

        public AppointmentManageController(IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._staffRepository = staffRepository;
            this._customerRepository = customerRepository;
            this._programRepository = programRepository;
            this._taskRepository = taskRepository;
            this._partnerRepository = partnerRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tourRepository = tourRepository;
            _db = new DataContext();
        }

        #endregion

        #region Index
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
            Permission(clsPermission.GetUser().PermissionID, 26);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_AppointmentList()
        {
            Permission(clsPermission.GetUser().PermissionID, 26);

            if (SDBID == 6)
                return PartialView("_Partial_AppointmentList", new List<tbl_AppointmentHistory>());
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => (p.StaffId == maNV | maNV == 0)
                    & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                    & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                    & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
                .Select(p => new tbl_AppointmentHistory
                {
                    Id = p.Id,
                    Time = p.Time,
                    tbl_Customer = _customerRepository.FindId(p.CustomerId),
                    tbl_Program = _programRepository.FindId(p.ProgramId),
                    tbl_Task = _taskRepository.FindId(p.TaskId),
                    tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                    tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                    tbl_Tour = _tourRepository.FindId(p.TourId),
                    Note = p.Note,
                    OtherStaff = p.OtherStaff,
                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                    tbl_Staff = _staffRepository.FindId(p.StatusId),
                    CreatedDate = p.CreatedDate
                }).ToList();
            return PartialView("_Partial_AppointmentList", model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
                .Select(p => new tbl_AppointmentHistory
                {
                    Id = p.Id,
                    Time = p.Time,
                    tbl_Customer = _customerRepository.FindId(p.CustomerId),
                    tbl_Program = _programRepository.FindId(p.ProgramId),
                    tbl_Task = _taskRepository.FindId(p.TaskId),
                    tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                    tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                    tbl_Tour = _tourRepository.FindId(p.TourId),
                    Note = p.Note,
                    OtherStaff = p.OtherStaff,
                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                    tbl_Staff = _staffRepository.FindId(p.StatusId),
                    CreatedDate = p.CreatedDate
                }).ToList();
                    return PartialView("_Partial_AppointmentList", list);
                }
                else
                {
                    return PartialView("_Partial_AppointmentList");
                }
            }
            catch
            {
                return PartialView("_Partial_AppointmentList");
            }
        }
        #endregion

        #region Update
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditAppointment(int id)
        {
            Permission(clsPermission.GetUser().PermissionID, 26);
            if (ViewBag.IsEdit)
            {
                var model = await _appointmentHistoryRepository.GetById(id);
                return PartialView("_Partial_EditAppointmentHistory", model);
            }
            return null;
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                if (await _appointmentHistoryRepository.Update(model))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.IsDelete == false)
                .Select(p => new tbl_AppointmentHistory
                {
                    Id = p.Id,
                    Time = p.Time,
                    tbl_Customer = _customerRepository.FindId(p.CustomerId),
                    tbl_Program = _programRepository.FindId(p.ProgramId),
                    tbl_Task = _taskRepository.FindId(p.TaskId),
                    tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                    tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                    tbl_Tour = _tourRepository.FindId(p.TourId),
                    Note = p.Note,
                    OtherStaff = p.OtherStaff,
                    tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                    tbl_Staff = _staffRepository.FindId(p.StatusId),
                    CreatedDate = p.CreatedDate
                }).ToList();
                    return PartialView("_Partial_AppointmentList", list);
                }
                else
                {
                    return PartialView("_Partial_AppointmentList");
                }
            }
            catch
            {
                return PartialView("_Partial_AppointmentList");
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
                        if (await _appointmentHistoryRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "AppointmentManage") }, JsonRequestBehavior.AllowGet);
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

        #region Filter

        [HttpPost]
        public async Task<ActionResult> FilterStatusTypeDate(int statusId, int typeId, DateTime? start, DateTime? end)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 26);

                if (SDBID == 6)
                    return PartialView("_Partial_AppointmentList", new List<tbl_AppointmentHistory>());
                var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => (start != null ? p.Time >= start : p.Id != 0) && (end != null ? p.Time <= end : p.Id != 0) && (statusId != -1 ? p.StatusId == statusId : p.Id != 0) && (typeId != -1 ? p.DictionaryId == typeId : p.Id != 0))
                    .Where(p => (p.StaffId == maNV | maNV == 0)
                    & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                    & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                    & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0)
                    & (p.IsDelete == false))
                               .Select(p => new tbl_AppointmentHistory
                               {
                                   Id = p.Id,
                                   Time = p.Time,
                                   tbl_Customer = _customerRepository.FindId(p.CustomerId),
                                   tbl_Program = _programRepository.FindId(p.ProgramId),
                                   tbl_Task = _taskRepository.FindId(p.TaskId),
                                   tbl_DictionaryService = _dictionaryRepository.FindId(p.tbl_DictionaryService),
                                   tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                                   tbl_Tour = _tourRepository.FindId(p.TourId),
                                   Note = p.Note,
                                   OtherStaff = p.OtherStaff,
                                   tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                   tbl_Staff = _staffRepository.FindId(p.StatusId),
                                   CreatedDate = p.CreatedDate
                               }).ToList();
                return PartialView("_Partial_AppointmentList", list);
            }
            catch
            {
                return PartialView("_Partial_AppointmentList");
            }
        }
        #endregion

        #region JsonCalendar
        public JsonResult JsonCalendar()
        {
            Permission(clsPermission.GetUser().PermissionID, 26);

            if (SDBID == 6)
                return Json(JsonRequestBehavior.AllowGet);
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => (p.StaffId == maNV | maNV == 0)
                    & (_staffRepository.FindId(p.StaffId).DepartmentId == maPB | maPB == 0)
                    & (_staffRepository.FindId(p.StaffId).StaffGroupId == maNKD | maNKD == 0)
                    & (_staffRepository.FindId(p.StaffId).HeadquarterId == maCN | maCN == 0) & (p.IsDelete == false))
               .Select(p => new tbl_AppointmentHistory
               {
                   Id = p.Id,
                   Time = p.Time,
                   Title = p.Title,
                   StatusId = p.StatusId

               }).ToList();

            var eventList = from e in model
                            select new
                            {
                                id = e.Id,
                                title = e.Title,
                                start = e.Time.ToString("s"),
                                constraint = e.Id,
                                color = (e.StatusId == 1145 ? "#3fc9d5" : e.StatusId == 1146 ? "#257e4a" : e.StatusId == 1147 ? "#fcb941" : e.StatusId == 1148 ? "#2574a9" : e.StatusId == 1149 ? "#3f3f3f" : "#eb4549")

                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AppointmentDetail
        [ChildActionOnly]
        public ActionResult _ChiTiet()
        {
            return PartialView("_ChiTiet", new tbl_AppointmentHistory());
        }

        public ActionResult AppointmentDetail(int id)
        {
            var item = _appointmentHistoryRepository.FindId(id);
            var model = new tbl_AppointmentHistory()
            {
                tbl_Customer = item.CustomerId != null ? _customerRepository.FindId(item.CustomerId) : null,
                tbl_DictionaryService = item.ServiceId != null ? _dictionaryRepository.FindId(item.ServiceId) : null,
                tbl_Program = item.ProgramId != null ? _programRepository.FindId(item.ProgramId) : null,
                tbl_Task = item.TaskId != null ? _taskRepository.FindId(item.TaskId) : null,
                Note = item.Note
            };
            return PartialView("_ChiTiet", model);
        }
        #endregion
    }
}
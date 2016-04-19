using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
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
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_AppointmentList()
        {
            var model = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
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
                model.StaffId = 9;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
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
            var model = await _appointmentHistoryRepository.GetById(id);
            return PartialView("_Partial_EditAppointmentHistory", model);
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
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable()
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
                        if (await _appointmentHistoryRepository.DeleteMany(listIds, true))
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
                var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => (start != null ? p.Time >= start : p.Id != 0) && (end != null ? p.Time <= end : p.Id != 0) && (statusId != -1 ? p.StatusId == statusId : p.Id != 0) && (typeId != -1 ? p.DictionaryId == typeId : p.Id != 0))
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
    }
}
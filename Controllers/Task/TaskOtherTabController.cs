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

namespace CRMViettour.Controllers.Task
{
    public class TaskOtherTabController : BaseController
    {
        //
        // GET: /TaskOtherTab/
        #region Init

        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_StaffVisa> _staffVisaRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_Company> _companyRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_TaskStaff> _taskStaffRepository;
        private IGenericRepository<tbl_TaskHandling> _taskHandlingRepository;
        private IGenericRepository<tbl_TaskNote> _taskNoteRepository;
        private DataContext _db;

        public TaskOtherTabController(
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_StaffVisa> customerVisaRepository,
            IGenericRepository<tbl_Company> companyRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_TaskStaff> taskStaffRepository,
            IGenericRepository<tbl_TaskHandling> taskHandlingRepository,
            IGenericRepository<tbl_TaskNote> taskNoteRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._staffRepository = staffRepository;
            this._partnerRepository = partnerRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._staffVisaRepository = customerVisaRepository;
            this._taskRepository = taskRepository;
            this._companyRepository = companyRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._taskStaffRepository = taskStaffRepository;
            this._taskHandlingRepository = taskHandlingRepository;
            this._taskNoteRepository = taskNoteRepository;
            _db = new DataContext();
        }
        #endregion

        #region Nhật ký xử lý

        #endregion

        #region DS nhân viên làm nv

        #endregion

        #region Lịch Hẹn

        #endregion

        #region Tài liệu mẫu

        #endregion

        #region Ghi chú
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateNote(tbl_TaskNote model, FormCollection form)
        {
            try
            {
                string id = Session["idTask"].ToString();
                if (ModelState.IsValid)
                {
                    model.TaskId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.StaffId = 9;


                    if (await _taskNoteRepository.Create(model))
                    {
                        var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId.ToString() == id).Select(p => new tbl_TaskNote
                        {
                            Id = p.Id,
                            Note = p.Note,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            CreatedDate = p.CreatedDate
                        }).ToList();
                        return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> DeleteNote(int id)
        {
            int tasId = _taskNoteRepository.FindId(id).TaskId;
            try
            {
                if (await _taskNoteRepository.Delete(id, true))
                {
                    var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == tasId).Select(p => new tbl_TaskNote
                    {
                        Id = p.Id,
                        Note = p.Note,
                        tbl_Staff = _staffRepository.FindId(p.StaffId),
                        CreatedDate = p.CreatedDate
                    }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditNote(int id)
        {
            var model = _taskNoteRepository.FindId(id);
            return PartialView("_Partial_EditNote", model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateTaskNote(tbl_TaskNote model, FormCollection form)
        {
            if (await _taskNoteRepository.Update(model))
            {
                var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == model.TaskId).Select(p => new tbl_TaskNote
                {
                    Id = p.Id,
                    Note = p.Note,
                    tbl_Staff = _staffRepository.FindId(p.StaffId),
                    CreatedDate = p.CreatedDate
                }).ToList();
                return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml", list);
            }
            else
                return PartialView("~/Views/TaskTabInfo/_GhiChu.cshtml");
        }
        #endregion
    }
}
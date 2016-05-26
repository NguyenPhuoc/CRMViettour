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
        [HttpPost]
        public async Task<ActionResult> DeleteHandling(int id)
        {
            var h = _taskHandlingRepository.FindId(id);
            int tasId = h.TaskId;
            try
            {
                if (h.File != null)
                {
                    String path = Server.MapPath("~/Upload/file/" + h.File);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                if (await _taskHandlingRepository.Delete(id, false))
                {
                    var list = _taskHandlingRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false).Select(p => new tbl_TaskHandling
                    {
                        Id = p.Id,
                        CreateDate = p.CreateDate,
                        Note = p.Note,
                        File = p.File,
                        PercentFinish = p.PercentFinish,
                        tbl_Staff = _staffRepository.FindId(p.StaffId),
                        tbl_Dictionary = _dictionaryRepository.FindId(p.StatusId)
                    }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_NhatKyXuLy.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_NhatKyXuLy.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_NhatKyXuLy.cshtml");
            }
        }
        #endregion

        #region DS nhân viên làm nv
        [HttpPost]
        public async Task<ActionResult> DeleteWork(int id)
        {
            int tasId = _taskStaffRepository.FindId(id).TaskId;
            try
            {

                if (await _taskStaffRepository.Delete(id, false))
                {
                    var list = _taskStaffRepository.GetAllAsQueryable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TaskTabInfo/_DSNhanVienDangLamNhiemVu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_DSNhanVienDangLamNhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_DSNhanVienDangLamNhiemVu.cshtml");
            }
        }
        #endregion

        #region Lịch Hẹn
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                model.TaskId = Convert.ToInt32(Session["idTask"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = 9;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == model.TaskId).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Time = p.Time,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                OtherStaff = p.OtherStaff
                            }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
            }
        }

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
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == model.TaskId).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Time = p.Time,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                OtherStaff = p.OtherStaff
                            }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            int tasId = _appointmentHistoryRepository.FindId(id).TaskId ?? 0;
            try
            {
                if (await _appointmentHistoryRepository.Delete(id, false))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_AppointmentHistory
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Time = p.Time,
                                tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusId),
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note,
                                OtherStaff = p.OtherStaff
                            }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_LichHen.cshtml");
            }
        }
        #endregion

        #region Tài liệu mẫu
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["TaskDocFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            //try
            //{

            string id = Session["idTask"].ToString();
            if (ModelState.IsValid)
            {
                model.TaskId = Convert.ToInt32(id);
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();
                model.StaffId = 9;
                //file
                HttpPostedFileBase FileName = Session["TaskDocFile"] as HttpPostedFileBase;
                string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/file/" + newName);
                FileName.SaveAs(path);
                //end file
                if (newName != null && FileSize != null)
                {
                    model.FileName = newName;
                    model.FileSize = FileSize;
                }

                if (await _documentFileRepository.Create(model))
                {
                    Session["TaskDocFile"] = null;
                    var list = _documentFileRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).Where(p => p.TaskId.ToString() == id).ToList();
                    //   var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TaskId.ToString() == id)
                    //.Select(p => new tbl_DocumentFile
                    //{
                    //    Id = p.Id,
                    //    FileName = p.FileName,
                    //    FileSize = p.FileSize,
                    //    Note = p.Note,
                    //    CreatedDate = p.CreatedDate,
                    //    TagsId = p.TagsId,
                    //    tbl_Staff = _staffRepository.FindId(p.StaffId)
                    //}).ToList();
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
                }
            }
            //}
            //catch { }
            return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoDocument(int id)
        {
            var model = await _documentFileRepository.GetById(id);
            List<SelectListItem> lstTag = new List<SelectListItem>();
            foreach (var t in _db.tbl_Tags.ToList())
            {
                lstTag.Add(new SelectListItem()
                {
                    Text = t.Tag,
                    Value = t.Id.ToString(),
                    Selected = model.TagsId.Split(',').Contains(t.Id.ToString()) ? true : false
                });
            }
            ViewBag.TagsId = lstTag;
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1), "Id", "Name", model.DictionaryId);
            return PartialView("~/Views/TaskOtherTab/_Partial_EditDocument.cshtml", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    if (Session["TaskDocFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["TaskDocFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                        String path = Server.MapPath("~/Upload/file/" + newName);
                        FileName.SaveAs(path);
                        //end file

                        if (FileName != null && FileSize != null)
                        {
                            String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                            if (System.IO.File.Exists(pathOld))
                                System.IO.File.Delete(pathOld);
                            model.FileName = newName;
                            model.FileSize = FileSize;
                        }
                    }
                    if (await _documentFileRepository.Update(model))
                    {
                        Session["TaskDocFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == model.CustomerId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TaskId == model.TaskId).Where(p => p.IsDelete == false)
                             .Select(p => new tbl_DocumentFile
                             {
                                 Id = p.Id,
                                 FileName = p.FileName,
                                 FileSize = p.FileSize,
                                 Note = p.Note,
                                 CreatedDate = p.CreatedDate,
                                 TagsId = p.TagsId,
                                 tbl_Staff = _staffRepository.FindId(p.StaffId)
                             }).ToList();
                        return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                int tasId = _documentFileRepository.FindId(id).TaskId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                if (await _documentFileRepository.Delete(id, false))
                {
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false)
                     .Select(p => new tbl_DocumentFile
                     {
                         Id = p.Id,
                         FileName = p.FileName,
                         FileSize = p.FileSize,
                         Note = p.Note,
                         CreatedDate = p.CreatedDate,
                         TagsId = p.TagsId,
                         tbl_Staff = _staffRepository.FindId(p.StaffId)
                     }).ToList();
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TaskTabInfo/_TaiLieuMau.cshtml");
            }
        }
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
                        var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId.ToString() == id).Where(p => p.IsDelete == false).Select(p => new tbl_TaskNote
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
                if (await _taskNoteRepository.Delete(id, false))
                {
                    var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == tasId).Where(p => p.IsDelete == false).Select(p => new tbl_TaskNote
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
                var list = _taskNoteRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TaskId == model.TaskId).Where(p => p.IsDelete == false).Select(p => new tbl_TaskNote
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
﻿using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using CRMViettour.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers.Tour
{
    [Authorize]
    public class TourOtherTabController : BaseController
    {
        // GET: TourOtherTab
        #region Init
        private IGenericRepository<tbl_LiabilityCustomer> _liabilityCustomerRepository;
        private IGenericRepository<tbl_LiabilityPartner> _liabilityPartnerRepository;
        private IGenericRepository<tbl_ReviewTour> _reviewTourRepository;
        private IGenericRepository<tbl_ReviewTourDetail> _reviewTourDetailRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private IGenericRepository<tbl_Customer> _customerRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;
        private IGenericRepository<tbl_Task> _taskRepository;
        private IGenericRepository<tbl_CustomerContact> _customerContactRepository;
        private IGenericRepository<tbl_CustomerVisa> _customerVisaRepository;
        private IGenericRepository<tbl_CustomerContactVisa> _customerContactVisaRepository;
        
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_ContactHistory> _contactHistoryRepository;
        private IGenericRepository<tbl_AppointmentHistory> _appointmentHistoryRepository;
        private IGenericRepository<tbl_Tour> _tourRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_TourCustomerVisa> _tourCustomerVisaRepository;
        private IGenericRepository<tbl_Quotation> _quotationRepository;
        private DataContext _db;

        public TourOtherTabController(
            IGenericRepository<tbl_LiabilityCustomer> liabilityCustomerRepository,
            IGenericRepository<tbl_LiabilityPartner> liabilityPartnerRepository,
            IGenericRepository<tbl_ReviewTour> reviewTourRepository,
            IGenericRepository<tbl_ReviewTourDetail> reviewTourDetailRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Customer> customerRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IGenericRepository<tbl_Task> taskRepository,
            IGenericRepository<tbl_CustomerContact> customerContactRepository,
            IGenericRepository<tbl_CustomerVisa> customerVisaRepository,
            
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_CustomerContactVisa> customerContactVisaRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_ContactHistory> contactHistoryRepository,
            IGenericRepository<tbl_AppointmentHistory> appointmentHistoryRepository,
            IGenericRepository<tbl_Tour> tourRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_TourCustomerVisa> tourCustomerVisaRepository,
            IGenericRepository<tbl_Quotation> quotationRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._liabilityCustomerRepository = liabilityCustomerRepository;
            this._liabilityPartnerRepository = liabilityPartnerRepository;
            this._reviewTourRepository = reviewTourRepository;
            this._reviewTourDetailRepository = reviewTourDetailRepository;
            this._partnerRepository = partnerRepository;
            this._contractRepository = contractRepository;
            this._customerRepository = customerRepository;
            this._customerContactRepository = customerContactRepository;
            this._tagsRepository = tagsRepository;
            this._taskRepository = taskRepository;
            this._customerVisaRepository = customerVisaRepository;
            this._customerContactVisaRepository = customerContactVisaRepository;
            
            this._dictionaryRepository = dictionaryRepository;
            this._documentFileRepository = documentFileRepository;
            this._contactHistoryRepository = contactHistoryRepository;
            this._appointmentHistoryRepository = appointmentHistoryRepository;
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            this._tourRepository = tourRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tourCustomerVisaRepository = tourCustomerVisaRepository;
            this._quotationRepository = quotationRepository;
            _db = new DataContext();
        }

        #endregion
        void Permission(int PermissionsId, int formId)
        {
            var list = _db.tbl_ActionData.Where(p => p.FormId == formId & p.PermissionsId == PermissionsId).Select(p => p.FunctionId).ToList();
            ViewBag.IsAdd = list.Contains(1);
            ViewBag.IsEdit = list.Contains(3);
            ViewBag.IsDelete = list.Contains(2);
            ViewBag.IsImport = list.Contains(4);
        }

        #region Lịch hẹn
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAppointment(tbl_AppointmentHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 87);
                model.TourId = Convert.ToInt32(Session["idTour"].ToString());
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;

                if (await _appointmentHistoryRepository.Create(model))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
            }
        }

        public JsonResult LoadPartner(int id)
        {
            var model = new SelectList(_partnerRepository.GetAllAsQueryable().Where(p => p.DictionaryId == id && p.IsDelete == false).ToList(), "Id", "Name");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadServicePartner(int id)
        {
            var model = new SelectList(_servicesPartnerRepository.GetAllAsQueryable().Where(p => p.PartnerId == id && p.IsDelete == false).ToList(), "Id", "Name");
            return Json(model, JsonRequestBehavior.AllowGet);
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
                Permission(clsPermission.GetUser().PermissionID, 87);
                model.ModifiedDate = DateTime.Now;
                if (await _appointmentHistoryRepository.Update(model))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            int tourId = _appointmentHistoryRepository.FindId(id).TourId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 87);
                if (await _appointmentHistoryRepository.Delete(id, false))
                {
                    var list = _appointmentHistoryRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichHen.cshtml");
            }
        }
        #endregion

        #region Lịch sử liên hệ
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateContactHistory(tbl_ContactHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 89);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.TourId = Int32.Parse(Session["idTour"].ToString());
                if (await _contactHistoryRepository.Create(model))
                {
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                       .Select(p => new tbl_ContactHistory
                       {
                           Id = p.Id,
                           ContactDate = p.ContactDate,
                           Request = p.Request,
                           Note = p.Note,
                           tbl_Staff = _staffRepository.FindId(p.StaffId),
                           tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId)
                       }).ToList();
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditContactHistory(int id)
        {
            var m = await _contactHistoryRepository.GetById(id);
            return PartialView("_Partial_EditContactHistory", await _contactHistoryRepository.GetById(id));
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContactHistory(tbl_ContactHistory model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 89);
                model.ModifiedDate = DateTime.Now;
                if (await _contactHistoryRepository.Update(model))
                {
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                        .Select(p => new tbl_ContactHistory
                        {
                            Id = p.Id,
                            ContactDate = p.ContactDate,
                            Request = p.Request,
                            Note = p.Note,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId)
                        }).ToList();
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContactHistory(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 89);
                int tourId = _contactHistoryRepository.FindId(id).TourId ?? 0;
                if (await _contactHistoryRepository.Delete(id, false))
                {
                    var list = _db.tbl_ContactHistory.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
                        .Select(p => new tbl_ContactHistory
                        {
                            ContactDate = p.ContactDate,
                            Request = p.Request,
                            Note = p.Note,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId)
                        }).ToList();
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_LichSuLienHe.cshtml");
            }
        }
        #endregion

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["TourFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 88);
                string id = Session["idTour"].ToString();
                if (ModelState.IsValid)
                {
                    model.TourId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file
                    HttpPostedFileBase FileName = Session["TourFile"] as HttpPostedFileBase;
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
                        Session["TourFile"] = null;
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TourId.ToString() == id).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
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
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1 && p.IsDelete == false), "Id", "Name", model.DictionaryId);
            return PartialView("_Partial_EditDocument", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 88);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    if (Session["TourFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["TourFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
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
                        Session["TourFile"] = null;
                        var list = _documentFileRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 88);
                int tourId = _documentFileRepository.FindId(id).TourId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                if (await _documentFileRepository.Delete(id, false))
                {
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_TaiLieuMau.cshtml");
            }
        }

        #endregion

        #region Nhiệm vụ

        [HttpPost]
        public async Task<ActionResult> EditTask(int id)
        {
            var model = await _taskRepository.GetById(id);
            int depId = _staffRepository.FindId(Convert.ToInt32(model.Permission)).DepartmentId ?? 0;
            ViewBag.DepartmentId = depId;
            ViewBag.PermissionList = _staffRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.DepartmentId == depId);
            return PartialView("_Partial_EditTaskTour", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateTask(tbl_Task model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 78);
                model.ModifiedDate = DateTime.Now;
                var cus = _customerRepository.FindId(model.CustomerId);
                //model.Email = cus.PersonalEmail != null ? cus.PersonalEmail : cus.CompanyEmail != null ? cus.CompanyEmail : null;
                //model.Phone = cus.Phone != null ? cus.Phone : null;
                model.Time = Int32.Parse((model.EndDate - model.StartDate).TotalDays.ToString());
                if (await _taskRepository.Update(model))
                {
                    var list = _taskRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_Task
                            {
                                Id = p.Id,
                                tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                                Name = p.Name,
                                Permission = p.Permission,
                                StartDate = p.StartDate,
                                EndDate = p.EndDate,
                                Time = p.Time,
                                TimeType = p.TimeType,
                                FinishDate = p.FinishDate,
                                PercentFinish = p.PercentFinish,
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note
                            }).ToList();
                    return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTask(int id)
        {
            int tourId = _taskRepository.FindId(id).TourId ?? 0;
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 78);
                if (await _taskRepository.Delete(id, false))
                {
                    var list = _taskRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_Task
                            {
                                Id = p.Id,
                                tbl_DictionaryTaskType = _dictionaryRepository.FindId(p.TaskTypeId),
                                Name = p.Name,
                                Permission = p.Permission,
                                StartDate = p.StartDate,
                                EndDate = p.EndDate,
                                Time = p.Time,
                                TimeType = p.TimeType,
                                FinishDate = p.FinishDate,
                                PercentFinish = p.PercentFinish,
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                Note = p.Note
                            }).ToList();
                    return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_NhiemVu.cshtml");
            }
        }
        #endregion

        #region Chương trình
        /********** Quản lý chương trình ************/

        [HttpPost]
        public ActionResult UploadProgram(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["ProgramTourFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateProgram(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 80);
                string id = Session["idTour"].ToString();
                if (ModelState.IsValid)
                {
                    model.TourId = Convert.ToInt32(id);
                    model.DictionaryId = 30;
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file
                    HttpPostedFileBase FileName = Session["ProgramTourFile"] as HttpPostedFileBase;
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
                        Session["ProgramTourFile"] = null;
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TourId.ToString() == id && p.DictionaryId == 30).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> EditInfoProgram(int id)
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
            return PartialView("_Partial_EditProgram", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateProgram(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 80);
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    if (Session["ProgramTourFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["ProgramTourFile"] as HttpPostedFileBase;
                        string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                        String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
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
                        Session["ProgramTourFile"] = null;
                        var list = _documentFileRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId && p.DictionaryId == 30).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProgram(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 80);
                int tourId = _documentFileRepository.FindId(id).TourId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                if (await _documentFileRepository.Delete(id, true))
                {
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.TourId == tourId && p.DictionaryId == 30).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_ChuongTrinh.cshtml");
            }
        }

        #endregion

        #region Hợp đồng

        [HttpPost]
        public ActionResult UploadContract(HttpPostedFileBase FileNameContract)
        {
            if (FileNameContract != null && FileNameContract.ContentLength > 0)
            {
                Session["ContractTourFile"] = FileNameContract;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EditContract(int id)
        {
            var model = await _contractRepository.GetById(id);
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
            return PartialView("_Partial_EditContract", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateContract(tbl_Contract model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 81);
                string id = Session["idTour"].ToString();
                model.TourId = Convert.ToInt32(id);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                if (await _contractRepository.Create(model))
                {
                    UpdateHistory.SaveContract(model.Id, 9, "Thêm mới hợp đồng, code: " + model.Code);

                    // upload file
                    HttpPostedFileBase FileName = Session["ContractTourFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    if (newName != null && FileSize != null)
                    {
                        Random rd =new Random();
                        var file = new tbl_DocumentFile
                        {
                            Code = rd.Next(11111, 99999).ToString(),
                            ContractId = model.Id,
                            CreatedDate = DateTime.Now,
                            CustomerId = _tourRepository.FindId(model.TourId).CustomerId,
                            DictionaryId = 28,
                            FileName = newName,
                            FileSize = FileSize,
                            IsCustomer = false,
                            IsDelete = false,
                            IsRead = false,
                            ModifiedDate = DateTime.Now,
                            PermissionStaff = model.StaffId.ToString(),
                            StaffId = model.StaffId,
                            TagsId = form["TagsId"].ToString(),
                            TourId = model.TourId
                        };
                        await _documentFileRepository.Create(file);
                    }
                    //

                    var list = _contractRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                       .Select(p => new tbl_Contract
                       {
                           Id = p.Id,
                           Permission = p.Permission,
                           Code = p.Code,
                           ContractDate = p.ContractDate,
                           StartDate = p.StartDate,
                           EndDate = p.EndDate,
                           tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusContractId),
                           NumberDay = p.NumberDay,
                           tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId),
                           TotalPrice = p.TotalPrice,
                           tbl_Staff = _staffRepository.FindId(p.StaffId),
                           CreatedDate = p.CreatedDate,
                           tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId)
                       }).ToList();

                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateContract(tbl_Contract model)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 81);
                model.ModifiedDate = DateTime.Now;
                if (await _contractRepository.Update(model))
                {
                    UpdateHistory.SaveContract(model.Id, 9, "Cập nhật hợp đồng, code: " + model.Code);
                    var list = _contractRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                        .Select(p => new tbl_Contract
                        {
                            Id = p.Id,
                            Permission = p.Permission,
                            Code = p.Code,
                            ContractDate = p.ContractDate,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusContractId),
                            NumberDay = p.NumberDay,
                            tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId),
                            TotalPrice = p.TotalPrice,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId)
                        }).ToList();

                    // upload file

                    //

                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteContract(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 81);
                int tourId = _contractRepository.FindId(id).TourId ?? 0;
                var history = _updateHistoryRepository.GetAllAsQueryable().Where(p => p.ContractId == id).Where(p => p.IsDelete == false).ToList();
                foreach (var item in history)
                {
                    await _updateHistoryRepository.Delete(item.Id, false);
                }
                if (await _contractRepository.Delete(id, false))
                {
                    var list = _contractRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
                        .Select(p => new tbl_Contract
                        {
                            Id = p.Id,
                            Permission = p.Permission,
                            Code = p.Code,
                            ContractDate = p.ContractDate,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            tbl_DictionaryStatus = _dictionaryRepository.FindId(p.StatusContractId),
                            NumberDay = p.NumberDay,
                            tbl_Dictionary = _dictionaryRepository.FindId(p.DictionaryId),
                            TotalPrice = p.TotalPrice,
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId)
                        }).ToList();
                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_HopDong.cshtml");
            }
        }

        #endregion

        #region Đánh giá
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateReviewTour(tbl_ReviewTour model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 86);
                string id = Session["idTour"].ToString();
                model.TourId = Convert.ToInt32(id);
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                int count = Convert.ToInt32(form["countService"].ToString());
                double mark = 0;
                for (int i = 1; i <= count; i++)
                {
                    mark += Convert.ToInt32(form["Mark" + i].ToString());
                }
                model.AverageMark = mark / count;

                if (await _reviewTourRepository.Create(model))
                {
                    for (int i = 1; i <= count; i++)
                    {
                        var detail = new tbl_ReviewTourDetail
                        {
                            CreatedDate = DateTime.Now,
                            DictionaryId = Convert.ToInt32(form["DictionaryId" + i]),
                            Mark = Convert.ToInt32(form["Mark" + i].ToString()),
                            ModifiedDate = DateTime.Now,
                            ReviewTourId = model.Id
                        };
                        await _reviewTourDetailRepository.Create(detail);
                    }

                    var list = _db.tbl_ReviewTourDetail.AsEnumerable().Where(p => p.tbl_ReviewTour.TourId.ToString() == id).Where(p => p.IsDelete == false)
                        .Select(p => new ReviewTourModel
                        {
                            Id = p.Id,
                            Email = p.tbl_ReviewTour.tbl_Customer.PersonalEmail,
                            Name = p.tbl_ReviewTour.tbl_Customer.FullName,
                            Note = p.tbl_ReviewTour.Note,
                            Phone = p.tbl_ReviewTour.tbl_Customer.MobilePhone,
                            Service = p.tbl_Dictionary.Name,
                            Mark = p.Mark,
                            Staff = p.tbl_ReviewTour.tbl_Staff.FullName,
                            Date = p.tbl_ReviewTour.CreatedDate
                        }).ToList();
                    return PartialView("~/Views/TourTabInfo/_DanhGia.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_DanhGia.cshtml");
                }
            }
            catch { }

            return PartialView("~/Views/TourTabInfo/_DanhGia.cshtml");
        }
        #endregion

        #region Công nợ đối tác
        public async Task<ActionResult> EditLiabilityPartner(int id)
        {
            var model = await _liabilityPartnerRepository.GetById(id);
            Session["idTour"] = model.TourId;
            return PartialView("_Partial_EditCongNoDoiTac", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateLiabilityPartner(FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 85);
                string id = Session["idTour"].ToString();
                for (int i = 1; i <= Convert.ToInt32(form["NumberOptionCongNo"]); i++)
                {
                    var model = new tbl_LiabilityPartner()
                    {
                        TourId = Convert.ToInt32(id),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        StaffId = 9,
                        PartnerId = Convert.ToInt32(form["PartnerId" + i].ToString()),
                        Note = form["Note" + i] != "" ? form["Note" + i].ToString() : "",
                        PaymentMethod = form["PaymentMethod" + i] != "" ? Convert.ToInt32(form["PaymentMethod" + i].ToString()) : 0,
                        SecondPayment = form["SecondPayment" + i] != "" ? Convert.ToDecimal(form["SecondPayment" + i].ToString()) : 0,
                        FirstPayment = form["FirstPayment" + i] != "" ? Convert.ToDecimal(form["FirstPayment" + i].ToString()) : 0,
                        TotalRemaining = form["TotalRemaining" + i] != "" ? Convert.ToDecimal(form["TotalRemaining" + i].ToString()) : 0,
                        ServicePrice = form["ServicePrice" + i] != "" ? Convert.ToDecimal(form["ServicePrice" + i].ToString()) : 0,
                        FirstCurrencyType = Convert.ToInt32(form["FirstCurrencyType" + i].ToString()),
                        SecondCurrencyType = Convert.ToInt32(form["FirstCurrencyType" + i].ToString()),
                    };
                    await _liabilityPartnerRepository.Create(model);
                }
                var list = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId.ToString() == id).Where(p => p.IsDelete == false)
                            .Select(p => new tbl_LiabilityPartner
                            {
                                Id = p.Id,
                                tbl_Staff = _staffRepository.FindId(p.StaffId),
                                tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                                PaymentMethod = p.PaymentMethod,
                                ServicePrice = p.ServicePrice,
                                FirstPayment = p.FirstPayment,
                                SecondPayment = p.SecondPayment,
                                TotalRemaining = p.TotalRemaining,
                                tbl_DictionaryCurrencyType1 = _dictionaryRepository.FindId(p.FirstCurrencyType),
                                Note = p.Note
                            }).ToList();

                return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml", list);
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateLiabilityPartner(tbl_LiabilityPartner model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 85);
                model.ModifiedDate = DateTime.Now;
                model.FirstPayment = form["FirstPayment"] != "" ? Convert.ToDecimal(form["FirstPayment"].ToString()) : 0;
                model.SecondPayment = form["SecondPayment"] != "" ? Convert.ToDecimal(form["SecondPayment"].ToString()) : 0;
                model.ServicePrice = form["ServicePrice"] != "" ? Convert.ToDecimal(form["ServicePrice"].ToString()) : 0;
                model.TotalRemaining = form["TotalRemaining"] != "" ? Convert.ToDecimal(form["TotalRemaining"].ToString()) : 0;
                if (await _liabilityPartnerRepository.Update(model))
                {
                    var list = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                       .Select(p => new tbl_LiabilityPartner
                       {
                           Id = p.Id,
                           tbl_Staff = _staffRepository.FindId(p.StaffId),
                           tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                           PaymentMethod = p.PaymentMethod,
                           ServicePrice = p.ServicePrice,
                           FirstPayment = p.FirstPayment,
                           SecondPayment = p.SecondPayment,
                           TotalRemaining = p.TotalRemaining,
                           tbl_DictionaryCurrencyType1 = _dictionaryRepository.FindId(p.FirstCurrencyType),
                           Note = p.Note
                       }).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteLiabilityPartner(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 85);
                int tourId = _liabilityPartnerRepository.FindId(id).TourId;
                Session["idTour"] = tourId;
                if (await _liabilityPartnerRepository.Delete(id, false))
                {
                    var list = _liabilityPartnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
                       .Select(p => new tbl_LiabilityPartner
                       {
                           Id = p.Id,
                           tbl_Staff = _staffRepository.FindId(p.StaffId),
                           tbl_Partner = _partnerRepository.FindId(p.PartnerId),
                           PaymentMethod = p.PaymentMethod,
                           ServicePrice = p.ServicePrice,
                           FirstPayment = p.FirstPayment,
                           SecondPayment = p.SecondPayment,
                           TotalRemaining = p.TotalRemaining
                       }).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_CongNoDoiTac.cshtml");
            }
        }
        #endregion

        #region Công nợ khách hàng
        public async Task<ActionResult> EditLiabilityCustomer(int id)
        {
            var model = await _liabilityCustomerRepository.GetById(id);
            Session["idTour"] = model.TourId;
            return PartialView("_Partial_EditCongNoKhachHang", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateLiabilityCustomer(tbl_LiabilityCustomer model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 84);
                string id = Session["idTour"].ToString();
                model.TourId = Convert.ToInt32(id);
                model.CreateDate = DateTime.Now;
                model.CustomerId = 9;
                model.StaffId = clsPermission.GetUser().StaffID;

                if (await _liabilityCustomerRepository.Create(model))
                {
                    var list = _liabilityCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateLiabilityCustomer(tbl_LiabilityCustomer model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 84);
                model.TotalContract = form["TotalContract"] != "" ? Convert.ToDecimal(form["TotalContract"].ToString()) : 0;
                model.FirstPrice = form["FirstPrice"] != "" ? Convert.ToDecimal(form["FirstPrice"].ToString()) : 0;
                model.SecondPrice = form["SecondPrice"] != "" ? Convert.ToDecimal(form["SecondPrice"].ToString()) : 0;
                model.ThirdPrice = form["ThirdPrice"] != "" ? Convert.ToDecimal(form["ThirdPrice"].ToString()) : 0;
                model.TotalLiquidation = form["TotalLiquidation"] != "" ? Convert.ToDecimal(form["TotalLiquidation"].ToString()) : 0;
                model.TotalRemaining = form["TotalRemaining"] != "" ? Convert.ToDecimal(form["TotalRemaining"].ToString()) : 0;

                if (await _liabilityCustomerRepository.Update(model))
                {
                    var list = _liabilityCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
                }
            }
            catch
            { }

            return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteLiabilityCustomer(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 84);
                int tourId = _liabilityCustomerRepository.FindId(id).TourId;
                Session["idTour"] = tourId;
                if (await _liabilityCustomerRepository.Delete(id, false))
                {
                    var list = _liabilityCustomerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false).ToList();
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_CongNoKH.cshtml");
            }
        }
        #endregion

        #region Onsuccsess
        public JsonResult CNKH()
        {
            int id = Int32.Parse(Session["idTour"].ToString());
            decimal CongNoKhachHang = _liabilityCustomerRepository.GetAllAsQueryable().Where(c => c.TourId == id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;

            var obj = new
            {
                Id = id,
                CongNo = CongNoKhachHang
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CNDT()
        {
            int id = Int32.Parse(Session["idTour"].ToString());
            decimal CongNoDoiTac = _liabilityPartnerRepository.GetAllAsQueryable().Where(c => c.TourId == id && c.IsDelete == false).Sum(c => c.TotalRemaining) ?? 0;

            var obj = new
            {
                Id = id,
                CongNo = CongNoDoiTac
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Visa
        [HttpPost]
        public async Task<ActionResult> DeleteVisa(int id)
        {
            int tourId = Int16.Parse(Session["idTour"].ToString());
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 82);
                int tourvisa = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.CustomerId == id & c.TourId == tourId).Select(c => c.Id).SingleOrDefault();
                if (await _tourCustomerVisaRepository.Delete(tourvisa, false))
                {
                    var list = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == tourId).Where(p => p.IsDelete == false).Select(c => c.tbl_CustomerVisa).ToList();
                    return PartialView("~/Views/TourTabInfo/_Visa.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_Visa.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_Visa.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateVisa(tbl_CustomerVisa model, FormCollection form)
        {
            try
            {
                string note = form["NoteVisa"];
                Permission(clsPermission.GetUser().PermissionID, 82);
                int idTour = Int16.Parse(Session["idTour"].ToString());
                if (form["listVisaId"] != null && form["listVisaId"] != "")
                {
                    var listIds = form["listVisaId"].Split(',');
                    listIds = listIds.Take(listIds.Count() - 1).ToArray();
                    if (listIds.Count() > 0)
                    {
                        foreach (var _id in listIds)
                        {
                            int id = Int16.Parse(_id);
                            var visa = _customerVisaRepository.FindId(id);
                            visa.DictionaryId = model.DictionaryId;
                            visa.tbl_Customer.NoteVisa = note;
                            await _customerVisaRepository.Update(visa);
                        }
                    }
                }
                var list = _tourCustomerVisaRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.TourId == idTour).Where(p => p.IsDelete == false).Select(c => c.tbl_CustomerVisa).ToList();

                return PartialView("~/Views/TourTabInfo/_Visa.cshtml", list);
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_Visa.cshtml");
            }
        }

        #endregion

        #region Báo giá

        [HttpPost]
        public ActionResult UploadFileQuotation(HttpPostedFileBase FileNameQuotation)
        {
            if (FileNameQuotation != null && FileNameQuotation.ContentLength > 0)
            {
                Session["QuotationFile"] = FileNameQuotation;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> EditQuotation(int id)
        {
            var model = await _quotationRepository.GetById(id);
            return PartialView("_Partial_EditQuotation", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateQuotation(tbl_Quotation model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 83);
                string id = Session["idTour"].ToString();
                model.TourId = Convert.ToInt32(id);
                model.StartDate = DateTime.Now;
                model.EndDate = DateTime.Now;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();
                model.DictionaryId = 29;
                model.StaffId = clsPermission.GetUser().StaffID;
                if (form["QuotationDate"] != null)
                {
                    model.QuotationDate = Convert.ToDateTime(form["QuotationDate"].ToString());
                }
                if (Session["QuotationFile"] != null)
                {
                    //file
                    HttpPostedFileBase FileName = Session["QuotationFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file

                    if (FileName != null && FileSize != null)
                    {
                        String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                        if (System.IO.File.Exists(pathOld))
                            System.IO.File.Delete(pathOld);
                        model.FileName = newName;
                    }
                }

                if (await _quotationRepository.Create(model))
                {
                    Session["QuotationFile"] = null;
                    var list = _quotationRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                        .Select(p => new tbl_Quotation
                        {
                            Id = p.Id,
                            Code = p.Code,
                            QuotationDate = p.QuotationDate,
                            tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            PriceTour = p.PriceTour,
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            FileName = p.FileName,
                            Note = p.Note,
                            CreatedDate = p.CreatedDate,
                            ModifiedDate = p.ModifiedDate
                        }).ToList();
                    return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateQuotation(tbl_Quotation model, FormCollection form)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 83);
                model.ModifiedDate = DateTime.Now;
                model.TagsId = form["TagsId"].ToString();
                model.StaffId = clsPermission.GetUser().StaffID;
                if (Session["QuotationFile"] != null)
                {
                    //file
                    HttpPostedFileBase FileName = Session["QuotationFile"] as HttpPostedFileBase;
                    string FileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file

                    if (FileName != null && FileSize != null)
                    {
                        String pathOld = Server.MapPath("~/Upload/file/" + model.FileName);
                        if (System.IO.File.Exists(pathOld))
                            System.IO.File.Delete(pathOld);
                        model.FileName = newName;
                    }
                }

                if (await _quotationRepository.Update(model))
                {
                    Session["QuotationFile"] = null;
                    var list = _quotationRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == model.TourId).Where(p => p.IsDelete == false)
                        .Select(p => new tbl_Quotation
                        {
                            Id = p.Id,
                            Code = p.Code,
                            QuotationDate = p.QuotationDate,
                            tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            PriceTour = p.PriceTour,
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            FileName = p.FileName,
                            Note = p.Note,
                            CreatedDate = p.CreatedDate,
                            ModifiedDate = p.ModifiedDate
                        }).ToList();
                    return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteQuotation(int id)
        {
            try
            {
                Permission(clsPermission.GetUser().PermissionID, 83);
                int tourId = _quotationRepository.FindId(id).TourId ?? 0;
                //file
                tbl_Quotation documentFile = _quotationRepository.FindId(id) ?? new tbl_Quotation();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file
                if (await _quotationRepository.Delete(id, false))
                {
                    var list = _quotationRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.TourId == tourId).Where(p => p.IsDelete == false)
                        .Select(p => new tbl_Quotation
                        {
                            Id = p.Id,
                            Code = p.Code,
                            QuotationDate = p.QuotationDate,
                            tbl_StaffQuotation = _staffRepository.FindId(p.StaffQuotationId),
                            tbl_Staff = _staffRepository.FindId(p.StaffId),
                            PriceTour = p.PriceTour,
                            tbl_DictionaryCurrency = _dictionaryRepository.FindId(p.CurrencyId),
                            FileName = p.FileName,
                            Note = p.Note,
                            CreatedDate = p.CreatedDate,
                            ModifiedDate = p.ModifiedDate
                        }).ToList();
                    return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/TourTabInfo/_ViettourBaoGia.cshtml");
            }
        }

        #endregion
    }
}
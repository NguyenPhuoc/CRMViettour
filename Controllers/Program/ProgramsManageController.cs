using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using CRMViettour.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    [Authorize]
    public class ProgramsManageController : BaseController
    {
        //
        // GET: /ProgramsManage/

        #region Init

        private IGenericRepository<tbl_Program> _programRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public ProgramsManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Program> programRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._programRepository = programRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }

        #endregion
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
        #region GetIdProgram
        [HttpPost]
        public ActionResult GetIdProgram(int id)
        {
            Session["idProgram"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region List
        public ActionResult Index()
        {
            Permission(clsPermission.GetUser().PermissionID, 18);

            if (SDBID == 6)
                return View(new List<tbl_Program>());

            var model = _programRepository.GetAllAsQueryable().Where(p => (p.StaffId == maNV | maNV == 0)
                    & (p.tbl_Staff.DepartmentId == maPB | maPB == 0)
                    & (p.tbl_Staff.StaffGroupId == maNKD | maNKD == 0)
                    & (p.tbl_Staff.HeadquarterId == maCN | maCN == 0)
                    & (p.IsDelete == false)).ToList();
            return View(model);
        }
        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Program model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Permission = "";
                model.DictionaryId = 30;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.TagsId = form["TagsId"].ToString();

                if (await _programRepository.Create(model))
                {
                    UpdateHistory.SaveProgram(model.Id, 9, "Thêm mới chương trình, code: " + model.Code);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch { }

            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        //[ChildActionOnly]
        //public ActionResult _Partial_EditProgram()
        //{
        //    return PartialView("_Partial_EditProgram", new tbl_Program());
        //}

        [HttpPost]
        public ActionResult ProgramInfomation(int id)
        {
            var model = _db.tbl_Program.Find(id);
            return PartialView("_Partial_EditProgram", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Program model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.StaffId = clsPermission.GetUser().StaffID;
                model.TagsId = form["TagsId"].ToString();
                if (await _programRepository.Update(model))
                {
                    UpdateHistory.SaveProgram(model.Id, 9, "Cập nhật chương trình, code: " + model.Code);
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
                        if (await _programRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ProgramsManage") }, JsonRequestBehavior.AllowGet);
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

        #region Document
        /********** Quản lý tài liệu ************/

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileName)
        {
            if (FileName != null && FileName.ContentLength > 0)
            {
                Session["ProgramFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                string id = Session["idProgram"].ToString();
                if (ModelState.IsValid)
                {
                    model.ProgramId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    model.StaffId = clsPermission.GetUser().StaffID;
                    //file
                    HttpPostedFileBase FileName = Session["ProgramFile"] as HttpPostedFileBase;
                    string CustomerFileSize = Common.ConvertFileSize(FileName.ContentLength);
                    String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ffffssmmHHddMMyyyy}", DateTime.Now));
                    String path = Server.MapPath("~/Upload/file/" + newName);
                    FileName.SaveAs(path);
                    //end file
                    if (newName != null && CustomerFileSize != null)
                    {
                        model.FileName = newName;
                        model.FileSize = CustomerFileSize;
                    }

                    if (await _documentFileRepository.Create(model))
                    {
                        Session["ProgramFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId.ToString() == id).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
        }

        //[ChildActionOnly]
        //public ActionResult _Partial_EditDocument()
        //{
        //    List<SelectListItem> lstTag = new List<SelectListItem>();
        //    List<SelectListItem> lstDictionary = new List<SelectListItem>();
        //    ViewData["TagsId"] = lstTag;
        //    ViewBag.DictionaryId = lstDictionary;
        //    return PartialView("_Partial_EditDocument", new tbl_DocumentFile());
        //}

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
                if (ModelState.IsValid)
                {
                    model.IsRead = true;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    if (Session["ProgramFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["ProgramFile"] as HttpPostedFileBase;
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
                        Session["ProgramFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == model.ProgramId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == model.ProgramId).Where(p => p.IsDelete == false)
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
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                int proId = _documentFileRepository.FindId(id).ProgramId ?? 0;
                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file

                if (await _documentFileRepository.Delete(id, false))
                {
                    //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == proId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ProgramId == proId).Where(p => p.IsDelete == false)
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
                    return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/ProgramTabInfo/_TaiLieuMau.cshtml");
            }
        }

        #endregion


        #region Export
        /// <summary>
        /// Export file excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFile()
        {
            var programs = _programRepository.GetAllAsQueryable().Where(p => p.IsDelete == false).ToList();

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, programs);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Programs.xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }


        public virtual void ExportCustomersToXlsx(Stream stream, IList<tbl_Program> programs)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Programs");

                var properties = new[]
                    {
                        "Mã chương trình",
                        "Tên chương trình",
                        "Tour",
                        "Khách hàng",
                        "Địa điểm",
                        "Ngày bắt đầu",
                        "Ngày kết thúc",
                        "Số ngày",
                        "Tổng giá trị",
                        "Ghi chú"
                    };



                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                }


                int row = 1;
                foreach (var program in programs)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = program.Code;
                    col++;

                    worksheet.Cells[row, col].Value = program.Name == null ? "" : program.Name;
                    col++;

                    worksheet.Cells[row, col].Value = program.tbl_Tour.Name;
                    col++;

                    worksheet.Cells[row, col].Value = program.tbl_Customer.FullName;
                    col++;

                    worksheet.Cells[row, col].Value = program.TagsId != null ? LoadData.LocationTags(program.TagsId) : "";
                    col++;

                    worksheet.Cells[row, col].Value = program.StartDate.ToString("dd/MM/yyyy");
                    col++;

                    worksheet.Cells[row, col].Value = program.EndDate.ToString("dd/MM/yyyy");
                    col++;

                    worksheet.Cells[row, col].Value = program.NumberDay;
                    col++;

                    worksheet.Cells[row, col].Value = program.TotalPrice;
                    col++;

                    worksheet.Cells[row, col].Value = program.Note == null ? "" : Regex.Replace(program.Note, "<.*?>", string.Empty);

                }
                worksheet.Cells["A1:J" + row].Style.Font.SetFromFont(new Font("Arial", 12));

                worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["A1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

                row++;

                worksheet.Cells["A1:J" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["A1:J" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["A1:J" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["A1:J" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:J" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));


                worksheet.Cells["A" + row + ":J" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A" + row + ":J" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["C" + row + ":H" + row].Merge = true;
                worksheet.Cells["B" + row].Value = row - 2;
                worksheet.Cells["I" + row].Formula = String.Format("=SUM(I2:I{0})", row - 1);
                worksheet.Cells["I2:I" + row].Style.Numberformat.Format = "#,#";

                worksheet.Cells["A1:J" + row].AutoFitColumns();

                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 25;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 17;
                worksheet.Column(7).Width = 17;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 25;
                worksheet.Column(10).Width = 30;


                xlPackage.Save();
            }
        }
        #endregion

    }
}

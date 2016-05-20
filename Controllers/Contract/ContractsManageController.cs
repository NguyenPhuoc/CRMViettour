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
    public class ContractsManageController : BaseController
    {
        //
        // GET: /ContractsManage/

        #region Init

        private IGenericRepository<tbl_UpdateHistory> _updateHistoryRepository;
        private IGenericRepository<tbl_Contract> _contractRepository;
        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public ContractsManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_UpdateHistory> updateHistoryRepository,
            IGenericRepository<tbl_Contract> contractRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._updateHistoryRepository = updateHistoryRepository;
            this._staffRepository = staffRepository;
            this._contractRepository = contractRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagRepository = tagRepository;
            _db = new DataContext();
        }

        #endregion

        #region List

        public ActionResult Index()
        {
            var model = _contractRepository.GetAllAsQueryable().ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetIdContract(int id)
        {
            Session["idContract"] = id;
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(tbl_Contract model, FormCollection form)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.Permission = "";
                model.DictionaryId = 28;
                model.StaffId = 9;
                model.TagsId = "";

                if (await _contractRepository.Create(model))
                {
                    UpdateHistory.SavePartner(model.Id, 9, "Thêm mới hợp đồng, code: " + model.Code);
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
        public ActionResult ContractInfomation(int id)
        {
            var model = _db.tbl_Contract.Find(id);
            return PartialView("_Partial_EditContract", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tbl_Contract model, FormCollection form)
        {
            try
            {
                model.ModifiedDate = DateTime.Now;
                model.StaffId = 9;
                model.TagsId = "";
                if (await _contractRepository.Update(model))
                {
                    UpdateHistory.SavePartner(model.Id, 9, "Cập nhật hợp đồng, code: " + model.Code);
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
                        foreach (string id in listIds)
                        {
                            var update = _db.tbl_UpdateHistory.AsEnumerable().FirstOrDefault(p => p.ContractId.ToString() == id);
                            _db.tbl_UpdateHistory.Remove(update);
                        }
                        if (await _contractRepository.DeleteMany(listIds, false))
                        {
                            return Json(new ActionModel() { Succeed = true, Code = "200", View = "", Message = "Xóa dữ liệu thành công !", IsPartialView = false, RedirectTo = Url.Action("Index", "ContractsManage") }, JsonRequestBehavior.AllowGet);
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
                Session["ContractFile"] = FileName;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateDocument(tbl_DocumentFile model, FormCollection form)
        {
            try
            {
                string id = Session["idContract"].ToString();
                if (ModelState.IsValid)
                {
                    model.ContractId = Convert.ToInt32(id);
                    model.CreatedDate = DateTime.Now;
                    model.IsRead = false;
                    model.ModifiedDate = DateTime.Now;
                    model.TagsId = form["TagsId"].ToString();
                    model.StaffId = 9;
                    //file
                    HttpPostedFileBase FileName = Session["ContractFile"] as HttpPostedFileBase;
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
                        Session["ContractFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId.ToString() == id).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId.ToString() == id)
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
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch { }
            return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
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
            ViewBag.DictionaryId = new SelectList(_dictionaryRepository.GetAllAsQueryable().Where(p => p.DictionaryCategoryId == 1), "Id", "Name", model.DictionaryId);
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
                    if (Session["ContractFile"] != null)
                    {
                        //file
                        HttpPostedFileBase FileName = Session["ContractFile"] as HttpPostedFileBase;
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
                        Session["ContractFile"] = null;
                        //var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId == model.ContractId).ToList();
                        var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId == model.ContractId)
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
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml", list);
                    }
                    else
                    {
                        return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
                    }
                }
            }
            catch
            {
            }

            return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            try
            {
                int conId = _documentFileRepository.FindId(id).ContractId ?? 0;

                //file
                tbl_DocumentFile documentFile = _documentFileRepository.FindId(id) ?? new tbl_DocumentFile();
                String path = Server.MapPath("~/Upload/file/" + documentFile.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                //end file

                if (await _documentFileRepository.Delete(id, false))
                {
                    // var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.CustomerId == cusId).ToList();
                    var list = _db.tbl_DocumentFile.AsEnumerable().Where(p => p.ContractId == conId)
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
                    return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml", list);
                }
                else
                {
                    return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
                }
            }
            catch
            {
                return PartialView("~/Views/ContractTabInfo/_TaiLieuMau.cshtml");
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
            var contracts = _contractRepository.GetAllAsQueryable().AsEnumerable()
                .Select(c => new tbl_Contract
            {
                Code = c.Code,
                ContractDate = c.ContractDate,
                Name = c.Name,
                tbl_Customer = c.tbl_Customer,
                tbl_Staff = c.tbl_Staff,
                StartDate = c.StartDate,
                NumberDay = c.NumberDay,
                tbl_DictionaryStatus = c.tbl_DictionaryStatus,
                tbl_Dictionary = c.tbl_Dictionary,
                Note = c.Note,
                TotalPrice = c.TotalPrice,
                CreatedDate = c.CreatedDate,
                ModifiedDate = c.ModifiedDate,

            }).ToList();
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    ExportCustomersToXlsx(stream, contracts);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Contracts.xlsx");
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Index");
        }


        public virtual void ExportCustomersToXlsx(Stream stream, IList<tbl_Contract> contracts)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add("Contracts");

                var properties = new[]
                    {
                        "Mã hợp đồng",
                        "Ngày ký",
                        "Tên hợp đồng",
                        "khách hàng",
                        "Điện thoại",
                        "Cơ hội",
                        "Báo giá",
                        "Nhân viên quản lý",
                        "Nhân viên hỗ trợ",
                        "Ngày hiệu lực",
                        "Thời hạn",
                        "Tình trạng",
                        "Phân loại",
                        "Diễn giải",
                        "Số tiền",
                        "Loại tiền",
                        "Người tạo",
                        "Ngày tạo",
                        "Người sửa",
                        "Ngày sửa"
                    };



                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                }


                int row = 1;
                foreach (var contract in contracts)
                {
                    row++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = contract.Code;
                    col++;

                    worksheet.Cells[row, col].Value = contract.ContractDate.ToString("d/M/yyyy");
                    col++;

                    worksheet.Cells[row, col].Value = contract.Name == null ? "" : contract.Name;
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Customer != null ? contract.tbl_Customer.FullName : "";
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Customer != null ? contract.tbl_Customer.Phone : "";
                    col++;

                    //worksheet.Cells[row, col].Value = contract.;
                    col++;

                    //worksheet.Cells[row, col].Value = contract.;
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Staff.FullName;
                    col++;

                    //worksheet.Cells[row, col].Value = contract.Position;
                    col++;

                    worksheet.Cells[row, col].Value = contract.StartDate.ToString("d/M/yyyy");
                    col++;

                    worksheet.Cells[row, col].Value = contract.NumberDay;
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_DictionaryStatus.Name;
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Dictionary.Name;
                    col++;

                    worksheet.Cells[row, col].Value = contract.Note == null ? "" : Regex.Replace(contract.Note, "<.*?>", string.Empty);
                    col++;

                    worksheet.Cells[row, col].Value = contract.TotalPrice;
                    col++;

                    //worksheet.Cells[row, col].Value = "VNĐ";
                    col++;

                    worksheet.Cells[row, col].Value = contract.tbl_Staff.FullName;
                    col++;

                    worksheet.Cells[row, col].Value = contract.CreatedDate.ToString("d/M/yyyy");
                    col++;

                    //worksheet.Cells[row, col].Value = contract.tbl_UpdateHistory == null ? "" : contract.tbl_UpdateHistory.Last().tbl_Staff.FullName;
                    col++;

                    worksheet.Cells[row, col].Value = contract.ModifiedDate.ToString("d/M/yyyy");
                    col++;

                }
                worksheet.Cells["a1:t" + row].Style.Font.SetFromFont(new Font("Tahoma", 8));

                worksheet.Cells["a1:t1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["a1:t1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a1:t1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));


                worksheet.Cells["a1:t" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:t" + row].Style.Border.Top.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:t" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:t" + row].Style.Border.Left.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:t" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:t" + row].Style.Border.Bottom.Color.SetColor(Color.FromArgb(169, 169, 169));
                worksheet.Cells["a1:t" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["a1:t" + row].Style.Border.Right.Color.SetColor(Color.FromArgb(169, 169, 169));

                row++;

                worksheet.Cells["a" + row + ":t" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["a" + row + ":t" + row].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                worksheet.Cells["b" + row + ":n" + row].Merge = true;
                worksheet.Cells["p" + row + ":t" + row].Merge = true;
                worksheet.Cells["a" + row].Value = row - 2;
                worksheet.Cells["o" + row].Formula = "=SUM(o2:o" + (row - 1) + ")";
                worksheet.Cells["o2:o" + row].Style.Numberformat.Format = "#,#";

                worksheet.Cells["a1:t" + row].AutoFitColumns();

                worksheet.Column(1).Width = 12;
                worksheet.Column(2).Width = 10;
                worksheet.Column(3).Width = 25;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 10;
                worksheet.Column(7).Width = 10;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 25;
                worksheet.Column(10).Width = 10;
                worksheet.Column(11).Width = 7;
                worksheet.Column(12).Width = 15;
                worksheet.Column(13).Width = 10;
                worksheet.Column(14).Width = 20;
                worksheet.Column(15).Width = 15;
                worksheet.Column(16).Width = 7;
                worksheet.Column(17).Width = 15;
                worksheet.Column(18).Width = 10;
                worksheet.Column(19).Width = 15;
                worksheet.Column(20).Width = 10;

                xlPackage.Save();
            }
        }
        #endregion
    }
}

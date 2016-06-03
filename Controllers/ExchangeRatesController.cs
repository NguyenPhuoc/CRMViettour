using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using CRMViettour.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    [Authorize]
    public class ExchangeRatesController : BaseController
    {
        //
        // GET: /ExchangeRates/
        #region Init

        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_Tags> _tagsRepository;

        private DataContext _db;

        public ExchangeRatesController(IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_Tags> tagsRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._tagsRepository = tagsRepository;
            _db = new DataContext();
        }

        #endregion
        public ActionResult Index()
        {
            ViewBag.IsAdd = true;
            ViewBag.IsEdit = true;
            ViewBag.IsDelete = true;
            var model = _dictionaryRepository.GetAllAsQueryable().AsEnumerable().Where(c => c.DictionaryCategoryId == 26 && c.IsDelete == false).ToList();
            return View(model);
        }

        public ActionResult ReadFileOnline()
        {
            return View();
        }

        #region Tỷ giá
        [ChildActionOnly]
        public ActionResult _TyGia()
        {
            return PartialView("_TyGia");
        }

        public ActionResult InfoTyGia(int id)
        {
            string filename = _dictionaryRepository.FindId(id).Note;
            FileStream _FileStream = new FileStream(Server.MapPath(@"~/Upload/file/" + filename),FileMode.Open, FileAccess.Read);
            using (var excelPackage = new ExcelPackage(_FileStream))
            {
                List<TyGiaModel> list = new List<TyGiaModel>();
                var worksheet = excelPackage.Workbook.Worksheets[1];
                var lastRow = worksheet.Dimension.End.Row;
                for (int row = 2; row <= lastRow; row++)
                {
                    var tygia = new TyGiaModel
                    {
                        Ma = worksheet.Cells["a" + row].Value != null ? worksheet.Cells["a" + row].Text : null,
                        Ten = worksheet.Cells["b" + row].Value != null ? worksheet.Cells["b" + row].Text : null,
                        MuaVao = worksheet.Cells["c" + row].Value != null ? worksheet.Cells["c" + row].Text : null,
                        ChuyenKhoan = worksheet.Cells["d" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                        BanRa = worksheet.Cells["e" + row].Value != null ? worksheet.Cells["e" + row].Text : null,
                    };
                    list.Add(tygia);
                }
                return PartialView("_TyGia", list);
            }
        }
        #endregion

        #region Import
        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase FileName)
        {
            try
            {
                using (var excelPackage = new ExcelPackage(FileName.InputStream))
                {
                    List<TyGiaModel> list = new List<TyGiaModel>();
                    var worksheet = excelPackage.Workbook.Worksheets[1];
                    var lastRow = worksheet.Dimension.End.Row;
                    for (int row = 2; row <= lastRow; row++)
                    {
                        var tygia = new TyGiaModel
                        {
                            Ma = worksheet.Cells["a" + row].Value != null ? worksheet.Cells["a" + row].Text : null,
                            Ten = worksheet.Cells["b" + row].Value != null ? worksheet.Cells["b" + row].Text : null,
                            MuaVao = worksheet.Cells["c" + row].Value != null ? worksheet.Cells["c" + row].Text : null,
                            ChuyenKhoan = worksheet.Cells["d" + row].Value != null ? worksheet.Cells["d" + row].Text : null,
                            BanRa = worksheet.Cells["e" + row].Value != null ? worksheet.Cells["e" + row].Text : null,
                        };
                        list.Add(tygia);
                    }
                    Session["fileTyGia"] = FileName;
                    return PartialView("_TyGia", list);
                }
            }
            catch (Exception)
            {
                return PartialView("_TyGia", new List<TyGiaModel>());
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImport()
        {
            try
            {
                HttpPostedFileBase FileName = Session["fileTyGia"] as HttpPostedFileBase;
                String newName = FileName.FileName.Insert(FileName.FileName.LastIndexOf('.'), String.Format("{0:_ddMMyyyy}", DateTime.Now));
                String path = Server.MapPath("~/Upload/file/" + newName);
                FileName.SaveAs(path);

                var dis = new tbl_Dictionary
                {
                    DictionaryCategoryId = 26,
                    Name = DateTime.Now.ToString("dd-MM-yyyy"),
                    Note = newName
                };
                await _dictionaryRepository.Create(dis);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}

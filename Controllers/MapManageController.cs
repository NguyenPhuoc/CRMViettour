using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using CRMViettour.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRMViettour.Controllers
{
    [Authorize]
    public class MapManageController : BaseController
    {
        //
        // GET: /MapManage/

        #region Init

        private IGenericRepository<tbl_Tags> _tagRepository;
        private IGenericRepository<tbl_PartnerNote> _partnerNoteRepository;
        private IGenericRepository<tbl_Dictionary> _dictionaryRepository;
        private IGenericRepository<tbl_DocumentFile> _documentFileRepository;
        private IGenericRepository<tbl_Partner> _partnerRepository;
        private IGenericRepository<tbl_ServicesPartner> _servicesPartnerRepository;
        private IGenericRepository<tbl_Staff> _staffRepository;
        private DataContext _db;

        public MapManageController(IGenericRepository<tbl_Dictionary> dictionaryRepository,
            IGenericRepository<tbl_PartnerNote> partnerNoteRepository,
            IGenericRepository<tbl_DocumentFile> documentFileRepository,
            IGenericRepository<tbl_Tags> tagRepository,
            IGenericRepository<tbl_Partner> partnerRepository,
            IGenericRepository<tbl_ServicesPartner> servicesPartnerRepository,
            IGenericRepository<tbl_Staff> staffRepository,
            IBaseRepository baseRepository)
            : base(baseRepository)
        {
            this._partnerNoteRepository = partnerNoteRepository;
            this._documentFileRepository = documentFileRepository;
            this._dictionaryRepository = dictionaryRepository;
            this._partnerRepository = partnerRepository;
            this._servicesPartnerRepository = servicesPartnerRepository;
            this._tagRepository = tagRepository;
            this._staffRepository = staffRepository;
            _db = new DataContext();
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Partial_ListPartner()
        {
            var model = _partnerRepository.GetAllAsQueryable().AsEnumerable().Where(p=>p.DictionaryId == 1047)
                .Select(p => new PartnerViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    xMap = p.xMap,
                    yMap = p.yMap,
                    AddressMap = p.AddressMap
                });
            return PartialView("_Partial_ListPartner", model);
        }

        [HttpPost]
        public ActionResult ListPartner(int id)
        {
            var model = _partnerRepository.GetAllAsQueryable().AsEnumerable().Where(p => p.DictionaryId == id)
               .Select(p => new PartnerViewModel
               {
                   Id = p.Id,
                   Name = p.Name,
                   Code = p.Code,
                   xMap = p.xMap,
                   yMap = p.yMap,
                   AddressMap = p.AddressMap
               });
            return PartialView("_Partial_ListPartner", model);
        }

        [HttpPost]
        public JsonResult LoadMarker()
        {
            return Json(new SelectList(_partnerRepository.GetAllAsQueryable(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        
    }
}

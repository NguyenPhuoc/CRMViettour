using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMViettour.Models;
using CRM.Core;

namespace CRMViettour.Models
{
    public class PartnerViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Contact { get; set; }
        public string Tags { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
    }

    public class PartnerListViewModel
    {
        public tbl_Partner SinglePartner { get; set; }
        public List<DocumentFileViewModel> ListDocument { get; set; }
        public List<tbl_PartnerNote> ListPartnerNote { get; set; }
    }
}
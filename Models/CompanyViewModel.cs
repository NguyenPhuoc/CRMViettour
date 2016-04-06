using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class CompanyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TagsId { get; set; }
        public string TaxCode { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
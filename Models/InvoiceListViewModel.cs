using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class InvoiceListViewModel
    {
        public int Id { get; set; }
        public string Partner { get; set; }
        public string Service { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string CodeTour { get; set; }
        public string NameTour { get; set; }
        public string NameStaff { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class TourServiceViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string StaffContact { get; set; }
        public string Phone { get; set; }
        public decimal? Price { get; set; }
        public string Note { get; set; }
        public Nullable<DateTime> Deadline { get; set; }
        public int TourId { get; set; }
        public int TourOptionId { get; set; }
    }
}
using CRM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class TourListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CustomerName { get; set; }
        public string DestinationPlace { get; set; }
        public int NumberDay { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int NumberCustomer { get; set; }
        public string TourGuide { get; set; }
        public string TourType { get; set; }
        public decimal CongNoKhachHang { get; set; }
        public decimal CongNoDoiTac { get; set; }
        public string Status { get; set; }
    }

    public class TourViewModel
    {
        public tbl_Tour SingleTour { get; set; }
        public tbl_TourGuide SingleTourGuide { get; set; }
        public Nullable<DateTime> StartDateTour { get; set; }
        public Nullable<DateTime> EndDateTour { get; set; }
        public Nullable<DateTime> StartDateTourGuide { get; set; }
        public Nullable<DateTime> EndDateTourGuide { get; set; }
    }
}
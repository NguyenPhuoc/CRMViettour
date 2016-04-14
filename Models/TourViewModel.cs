using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class TourViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CustomerName { get; set; }
        public string DestinationPlace { get; set; }
        public int NumberDay { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberCustomer { get; set; }
        public string TourGuide { get; set; }
        public string TourType { get; set; }
    }
}
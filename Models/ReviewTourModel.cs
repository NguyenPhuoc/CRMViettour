using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class ReviewTourModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Service { get; set; }
        public double Mark { get; set; }
        public string Note { get; set; }
        public string Staff { get; set; }
        public DateTime Date { get; set; }
    }
}
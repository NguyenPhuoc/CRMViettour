using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;

namespace CRMViettour.Models
{
    public class StaffListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Fullname { get; set; }
        public string Birthday { get; set; }
        public string Passport { get; set; }
        public string CreateDatePassport { get; set; }
        public string ExpiredDatePassport { get; set; }
        public string Phone { get; set; }
        public int InternalNumber { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public Boolean IsLock { get; set; }
        public string Hometown { get; set; }
        public string Address { get; set; }
        public string IdentityCard { get; set; }
        public string CreateDateIdentity { get; set; }
        public string PlaceIdentity { get; set; }
        public string StaffGroup { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
    }

    public class StaffViewModel
    {
        public tbl_Staff SingleStaff { get; set; }
        public SingleStaffVisa SingleStaffVisa { get; set; }
        public List<tbl_StaffVisa> ListStaffVisa { get; set; }
        public string IdentityCard { get; set; }
        public DateTime CreatedDateIdentity { get; set; }
        public int IdentityTagId { get; set; }
        public string PassportCard { get; set; }
        public DateTime CreatedDatePassport { get; set; }
        public DateTime ExpiredDatePassport { get; set; }
        public int PassportTagId { get; set; }
    }

    public class SingleStaffVisa
    {
        public string VisaNumber { get; set; }
        public int TagsId { get; set; }
        public DateTime CreatedDateVisa { get; set; }
        public DateTime ExpiredDateVisa { get; set; }
    }

    public class InfoStaffViewModel
    {
        public tbl_Staff SingleStaff { get; set; }
        public List<tbl_StaffVisa> ListStaffVisa { get; set; }
        public List<DocumentFileViewModel> ListStaffDocument { get; set; }
    }
}
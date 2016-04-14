using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;

namespace CRMViettour.Models
{
    public class CustomerListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Fullname { get; set; }
        public string Birthday { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string OtherPhone { get; set; }
        public string Address { get; set; }
        public string TagsId { get; set; }
        public string Career { get; set; }
        public string Passport { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string IdentityCard { get; set; }
        public string NameCustomerContract { get; set; }
        public string PhoneCustomerContract { get; set; }
        public string Note { get; set; }
    }

    public class CustomerViewModel
    {
        public tbl_Customer SinglePersonal { get; set; }
        public tbl_Customer SingleCompany { get; set; }
        public tbl_CustomerContact SingleContact { get; set; }
        public string IdentityCard { get; set; }
        public DateTime CreatedDateIdentity { get; set; }
        public int IdentityTagId { get; set; }
        public string PassportCard { get; set; }
        public DateTime CreatedDatePassport { get; set; }
        public DateTime ExpiredDatePassport { get; set; }
        public int PassportTagId { get; set; }
        public SingleVisa SingleVisa { get; set; }
        public List<tbl_CustomerVisa> ListCustomerVisa { get; set; }
    }

    public class SingleVisa
    {
        public string VisaNumber { get; set; }
        public int TagsId { get; set; }
        public DateTime CreatedDateVisa { get; set; }
        public DateTime ExpiredDateVisa { get; set; }
    }

    public class InfoCustomerViewModel
    {
        public tbl_Customer SingleCustomer { get; set; }
        public List<tbl_CustomerVisa> ListCustomerVisa { get; set; }
        public List<DocumentFileViewModel> ListCustomerFile { get; set; }
        public List<DocumentFileViewModel> ListCustomerDocument { get; set; }
        public List<tbl_AppointmentHistory> ListCustomerAppointment { get; set; }
        public List<tbl_ContactHistory> ListCustomerContactHistory { get; set; }
        public List<tbl_UpdateHistory> ListCustomerUpdateHistory { get; set; }
    }

}
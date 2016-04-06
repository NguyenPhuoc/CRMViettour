using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;
using CRM.Infrastructure;

namespace CRMViettour.Utilities
{
    public static class UpdateHistory
    {
        private static DataContext _db = new DataContext();

        public static void SaveCustomer(int customerId, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                CustomerId = customerId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SavePartner(int partnerId, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                PartnerId = partnerId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveStaff(int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveProgram(int programId, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                ProgramId = programId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveTour(int tourId, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                TourId = tourId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

        public static void SaveContract(int contractId, int staffId, string note)
        {
            var item = new tbl_UpdateHistory
            {
                ContractId = contractId,
                StaffId = staffId,
                Note = note,
                CreatedDate = DateTime.Now,
                DictionaryId = 1148
            };
            _db.tbl_UpdateHistory.Add(item);
            _db.SaveChanges();
        }

    }
}
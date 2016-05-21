using CRM.Core;
using CRM.Enum;
using CRM.Infrastructure;
using CRMViettour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Utilities
{
    //hello
    public static class LoadData
    {
        private static DataContext _db = new DataContext();

        public static string LocationTags(string tagsId)
        {
            string kq = "";
            var array = tagsId.Split(',');
            for (int i = array.Count() - 1; i >= 0; i--)
            {
                if (array[i] != "")
                {
                    if (i == 0)
                    {
                        kq += _db.tbl_Tags.Find(Convert.ToInt32(array[i])).Tag;
                    }
                    else
                    {
                        kq += _db.tbl_Tags.Find(Convert.ToInt32(array[i])).Tag + ", ";
                    }
                }
            }

            return kq;
        }

        /// <summary>
        /// load phòng ban
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public static string Department(string staffId)
        {
            string kq = _db.tbl_Staff.Find(Convert.ToInt32(staffId)).tbl_DictionaryDepartment.Name;
            return kq;
        }

        /// <summary>
        /// nhân viên thực hiện nhiệm vụ
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public static string StaffTask(string staffId)
        {
            return _db.tbl_Staff.Find(Convert.ToInt32(staffId)).FullName;
        }

        /// <summary>
        /// lấy ra tên các nhân viên
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public static string StaffPermission(string staffId)
        {
            string kq = "";
            var array = staffId.Split(',');
            for (int i = array.Count() - 1; i >= 0; i--)
            {
                if (array[i] != "")
                {
                    if (i == 0)
                    {
                        kq += _db.tbl_Staff.Find(Convert.ToInt32(array[i])).FullName;
                    }
                    else
                    {
                        kq += _db.tbl_Staff.Find(Convert.ToInt32(array[i])).FullName + ", ";
                    }
                }
            }

            return kq;
        }

        /// <summary>
        /// các tag vị trí địa lý
        /// </summary>
        /// <returns></returns>
        public static List<TagsViewModel> DropdownlistLocation()
        {
            var model = CacheLayer.Get<List<TagsViewModel>>("tagLocationList");
            if (model == null)
            {
                model = _db.tbl_Tags.Select(p => new TagsViewModel
                {
                    Id = p.Id,
                    Tags = p.Tag
                }).ToList();
                CacheLayer.Add<List<TagsViewModel>>(model, "tagLocationList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách quốc gia
        /// </summary>
        /// <returns></returns>
        public static List<TagsViewModel> DropdownlistCountry()
        {
            var model = CacheLayer.Get<List<TagsViewModel>>("tagCountryList");
            if (model == null)
            {
                model = _db.tbl_Tags.Where(p => p.TypeTag == 3).Select(p => new TagsViewModel
                {
                    Id = p.Id,
                    Tags = p.Tag
                }).ToList();
                CacheLayer.Add<List<TagsViewModel>>(model, "tagCountryList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách tất cả các đối tác
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> PartnerAllList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("partnerAllList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Select(p => new tbl_Partner
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "partnerAllList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách đối tác
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> PartnerList(int id)
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("partnerList" + id);
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.DictionaryId == id).Select(p => new tbl_Partner
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "partnerList" + id, 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại tài liệu
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> DocumentTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("documentTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 1).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "documentTypeList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách chuyến bay
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> FlightList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("flightList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 25).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "flightList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại tour
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TourTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("tourTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 19).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "tourTypeList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TaskTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("taskTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 21).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "taskTypeList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách trạng thái nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TaskStatusList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("taskStatusList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 22).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "taskStatusList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách độ ưu tiên nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> TaskPriorityList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("taskPriorityList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 23).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "taskPriorityList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách loại lịch hẹn
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> AppointmentTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("appointmentTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 20).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "appointmentTypeList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách tình trạng visa
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> VisaStatusList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("visaStatusList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 14).Select(p => new DictionaryViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "visaStatusList", 10080);
            }

            return model;
        }

        /// <summary>
        /// loại visa
        /// </summary>
        /// <returns></returns>
        public static List<DictionaryViewModel> VisaTypeList()
        {
            var model = CacheLayer.Get<List<DictionaryViewModel>>("visaTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.AsEnumerable().Where(p => p.DictionaryCategoryId == 15).Select(p => new DictionaryViewModel
                {
                    Id = Convert.ToInt32(p.Note),
                    Name = p.Name
                }).ToList();
                CacheLayer.Add<List<DictionaryViewModel>>(model, "visaTypeList", 10080);
            }

            return model;
        }

        /// <summary>
        /// nhiệm vụ
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Task> TaskList()
        {
            var model = CacheLayer.Get<List<tbl_Task>>("taskList");
            if (model == null)
            {
                model = _db.tbl_Task.AsEnumerable().Select(p => new tbl_Task
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code
                }).ToList();
                CacheLayer.Add<List<tbl_Task>>(model, "taskList", 10080);
            }

            return model;
        }

        /// <summary>
        /// chương trình
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Program> ProgramList()
        {
            var model = CacheLayer.Get<List<tbl_Program>>("programList");
            if (model == null)
            {
                model = _db.tbl_Program.AsEnumerable().Select(p => new tbl_Program
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code
                }).ToList();
                CacheLayer.Add<List<tbl_Program>>(model, "programList", 10080);
            }

            return model;
        }

        /// <summary>
        /// công ty
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Company> CompanyList()
        {
            var company = CacheLayer.Get<List<tbl_Company>>("companyList");
            if (company == null)
            {
                company = _db.tbl_Company.ToList();
                CacheLayer.Add<List<tbl_Company>>(company, "companyList", 10080);
            }

            return company;
        }

        /// <summary>
        /// danh sách khách hàng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Customer> CustomerList()
        {
            var customer = CacheLayer.Get<List<tbl_Customer>>("customerList");
            if (customer == null)
            {
                customer = _db.tbl_Customer.AsEnumerable()
                    .Select(p => new tbl_Customer
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        Code = p.Code
                    }).ToList();
                CacheLayer.Add<List<tbl_Customer>>(customer, "customerList", 10080);
            }

            return customer;
        }

        /// <summary>
        /// danh sách hợp đồng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Contract> ContractList()
        {
            var contract = CacheLayer.Get<List<tbl_Contract>>("contractlist");
            if (contract == null)
            {
                contract = _db.tbl_Contract.AsEnumerable()
                    .Select(p => new tbl_Contract
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code
                    }).ToList();
                CacheLayer.Add<List<tbl_Contract>>(contract, "contractlist", 10080);
            }

            return contract;
        }

        /// <summary>
        /// danh sách nhân viên
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Staff> StaffList()
        {
            var staff = CacheLayer.Get<List<tbl_Staff>>("staffList");
            if (staff == null)
            {
                staff = _db.tbl_Staff.AsEnumerable()
                    .Select(p => new tbl_Staff
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        Code = p.Code,
                        Birthday = p.Birthday,
                        Address = p.Address,
                        PositionId = p.PositionId,
                        DepartmentId = p.DepartmentId,
                        StaffGroupId = p.StaffGroupId
                    }).ToList();
                CacheLayer.Add<List<tbl_Staff>>(staff, "staffList", 10080);
            }

            return staff;
        }

        /// <summary>
        /// nguồn đến
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> OriginList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("originList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 4).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "originList", 10080);
            }

            return model;
        }

        /// <summary>
        /// ngành nghề
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CareerList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("careerList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 2).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "careerList", 10080);
            }

            return model;
        }

        /// <summary>
        /// nhóm khách hàng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CustomerGroupList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("customerGroupList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 3).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "customerGroupList", 10080);
            }

            return model;
        }

        /// <summary>
        /// trạng thái xử lý
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> StatusProcessList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("statusProcessList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 17).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "statusProcessList", 10080);
            }

            return model;
        }

        /// <summary>
        /// vị trí
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> PositionList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("positionList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 5).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "positionList", 10080);
            }

            return model;
        }

        /// <summary>
        /// phòng ban
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> DepartmentList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("departmentList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 6).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "departmentList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh xưng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> NameTypeList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("nameTypeList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 7).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "nameTypeList", 10080);
            }

            return model;
        }

        /// <summary>
        /// tình trạng hợp đồng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> StatusContractList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("statusContractList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 18).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "statusContractList", 10080);
            }

            return model;
        }

        /// <summary>
        /// bằng cấp
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CertificateList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("certificateList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 12).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "certificateList", 10080);
            }

            return model;
        }

        /// <summary>
        /// dân tộc
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> NationList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("nationList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 10).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "nationList", 10080);
            }

            return model;
        }

        /// <summary>
        /// tôn giáo
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> ReligionList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("religionList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 11).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "religionList", 10080);
            }

            return model;
        }

        /// <summary>
        /// nhóm nhân viên
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> StaffGroupList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("staffGroupList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 16).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "staffGroupList", 10080);
            }

            return model;
        }

        /// <summary>
        /// loại tiền
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> CurrencyList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("currencyList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.Where(p => p.DictionaryCategoryId == 24).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "currencyList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách trụ sở chi nhánh
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Headquater> HeadquarterList()
        {
            var model = CacheLayer.Get<List<tbl_Headquater>>("headquarterList");
            if (model == null)
            {
                model = _db.tbl_Headquater.AsEnumerable().Select(p => new tbl_Headquater { Id = p.Id, ShortName = p.ShortName }).ToList();
                CacheLayer.Add<List<tbl_Headquater>>(model, "headquarterList", 10080);
            }

            return model;
        }

        /// <summary>
        /// dịch vụ
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Dictionary> ServiceList()
        {
            var model = CacheLayer.Get<List<tbl_Dictionary>>("serviceList");
            if (model == null)
            {
                model = _db.tbl_Dictionary.AsEnumerable().Where(p => p.DictionaryCategoryId == 13).Select(p => new tbl_Dictionary { Id = p.Id, Name = p.Name }).ToList();
                CacheLayer.Add<List<tbl_Dictionary>>(model, "serviceList", 10080);
            }

            return model;
        }

        /// <summary>
        /// tour
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Tour> TourList()
        {
            var model = CacheLayer.Get<List<tbl_Tour>>("tourList");
            if (model == null)
            {
                model = _db.tbl_Tour.AsEnumerable().Select(p => new tbl_Tour { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Tour>>(model, "tourList", 10080);
            }

            return model;
        }

        /// <summary>
        /// new code staff
        /// </summary>
        /// <returns></returns>
        public static string NewCodeStaff()
        {
            var staf = _db.tbl_Staff.AsEnumerable().Last();
            string num = staf.Code.Substring(2);
            int codenum = Int32.Parse(num);
            codenum++;
            string newcode = "NV" + codenum.ToString("D4");
            return newcode;
        }

        /// <summary>
        /// new code customer Personal
        /// </summary>
        /// <returns></returns>
        public static string NewCodeCustomerPersonal()
        {
            _db = new DataContext();
            var staf = _db.tbl_Customer.AsEnumerable().Where(c => c.CustomerType == CustomerType.Personal && c.IsTemp == false).Last();
            string num = staf.Code.Substring(3);
            string codechar = staf.Code.Substring(2, 1);
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int i = alphabet.IndexOf(codechar);
            int codenum = Int32.Parse(num);
            codenum++;
            if (codenum == 100000)
            {
                codenum = 1;
                codechar = alphabet[i++].ToString();
            }
            string newcode = "KH" + codechar + codenum.ToString("D5");
            return newcode;
        }

        /// <summary>
        /// danh sách đối tác khách sạn
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> HotelList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("hotelList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.DictionaryId == 1048).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "hotelList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách đối tác nhà hàng
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> RestaurantList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("restaurantList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.DictionaryId == 1047).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "restaurantList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách đối tác vé máy bay
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> PlaneList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("planeList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.DictionaryId == 1049).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "planeList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách đối tác vận chuyển
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> TransportList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("transportList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.DictionaryId == 1050).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "transportList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách đối tác sự kiện
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> EventList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("eventList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.DictionaryId == 1051).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "eventList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách các đối tác khác
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Partner> OtherList()
        {
            var model = CacheLayer.Get<List<tbl_Partner>>("otherList");
            if (model == null)
            {
                model = _db.tbl_Partner.AsEnumerable().Where(p => p.DictionaryId == 1052).Select(p => new tbl_Partner { Id = p.Id, Name = p.Name, Code = p.Code }).ToList();
                CacheLayer.Add<List<tbl_Partner>>(model, "otherList", 10080);
            }

            return model;
        }

        /// <summary>
        /// danh sách các tag theo type
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Tags> LoadTagsByType(int id)
        {
            var model = CacheLayer.Get<List<tbl_Tags>>("loadTagsByType" + id);
            if (model == null)
            {
                model = _db.tbl_Tags.AsEnumerable().Where(c => c.IsDelete == false && c.TypeTag == id).Select(p => new tbl_Tags { Id = p.Id, Tag = p.Tag }).ToList();
                CacheLayer.Add<List<tbl_Tags>>(model, "loadTagsByType" + id, 10080);
            }
            return model;
        }

        /// <summary>
        /// danh sách các function
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Function> FunctionList()
        {
            var model = CacheLayer.Get<List<tbl_Function>>("functionList");
            if (model == null)
            {
                model = _db.tbl_Function.AsEnumerable().Where(c => c.IsDelete == false).Select(p => new tbl_Function { Id = p.Id, Name = p.Name }).ToList();
                CacheLayer.Add<List<tbl_Function>>(model, "functionList", 10080);
            }
            return model;
        }

        /// <summary>
        /// danh sách module vs form
        /// </summary>
        /// <returns></returns>
        public static List<tbl_Module> ModuleFormList()
        {
            var model = CacheLayer.Get<List<tbl_Module>>("moduleformlist");
            if (model == null)
            {
                model = _db.tbl_Module.AsEnumerable().Select(p => new tbl_Module
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();
                foreach (var item in model)
                {
                    item.tbl_Form = _db.tbl_Form.AsEnumerable().Where(c => c.ModuleId == item.Id).Select(c => new tbl_Form
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();
                }
                CacheLayer.Add<List<tbl_Module>>(model, "moduleformlist", 10080);
            }
            return model;
        }

        /// <summary>
        /// danh sách quyền truy cập
        /// </summary>
        /// <returns></returns>
        public static List<tbl_ShowDataBy> ShowDataByList()
        {
            var model = CacheLayer.Get<List<tbl_ShowDataBy>>("showdatabylist");
            if (model == null)
            {
                model = _db.tbl_ShowDataBy.AsEnumerable().ToList();
                CacheLayer.Add<List<tbl_ShowDataBy>>(model, "showdatabylist", 10080);
            }
            return model;
        }
        public static bool TourUpdate(int id)
        {
            _db = new DataContext();
            return _db.tbl_Tour.AsEnumerable().Where(c => c.Id == id).Select(c => c.IsUpdate).Single();
        }
    }
}
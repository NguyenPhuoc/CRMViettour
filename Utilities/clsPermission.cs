using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMViettour.Models;

namespace CRMViettour.Utilities
{
    public static class clsPermission
    {
        public static CookieUser GetUser()
        {
            var item = new CookieUser();
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                string user = HttpContext.Current.User.Identity.Name;
                if (HttpContext.Current.Request.Cookies["CookieUser" + user] != null)
                {
                    item.StaffID = Convert.ToInt32(HttpContext.Current.Request.Cookies["CookieUser" + user]["MaNV"]);
                    item.BranchID = Convert.ToInt32(HttpContext.Current.Request.Cookies["CookieUser" + user]["MaCN"]);
                    item.DepartmentID = Convert.ToInt32(HttpContext.Current.Request.Cookies["CookieUser" + user]["MaPB"]);
                    item.GroupID = Convert.ToInt32(HttpContext.Current.Request.Cookies["CookieUser" + user]["MaNKD"]);
                }
            }
            return item;
        }
    }
}
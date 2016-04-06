using CRM.Core;
using CRM.Infrastructure;
using CRMViettour.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading;

namespace CRMViettour.Utilities
{
    public class Common
    {
        private static readonly char[] chars = new char[] { ',', '(', ')', ' ', '+', '=', '>', '<', '&' };

        #region Orthers
        private static readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveUnicode(string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }

        public static string CutdownDescription(string str, int numberWords)
        {
            if(str != null && str != "")
            {
                char[] characters = new char[] { ' ' };
                string[] splitStr = str.Split(characters);
                if (numberWords > splitStr.Length)
                {
                    return str;
                }
                else
                {
                    var tempDes = "";
                    for (var i = 0; i < numberWords; i++)
                    {
                        tempDes += splitStr[i] + " ";
                    }
                    return tempDes;
                }
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Render View
        /// <summary>
        /// Render Razor View to String
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static String RenderRazorViewToString(ControllerContext controllerContext, String viewName, Object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var ViewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var ViewContext = new ViewContext(controllerContext, ViewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                ViewResult.View.Render(ViewContext, sw);
                ViewResult.ViewEngine.ReleaseView(controllerContext, ViewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion

        public static string ConvertFileSize(double filesize)
        {
            string[] sizes = { "Byte", "KB", "MB", "GB" };
            int order = 0;
            while (filesize >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                filesize = filesize / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", filesize, sizes[order]);
        }


        public static string MakeUrlSlug(string strInput)
        {
            try
            {
                for (int i = 1; i < VietNamChar.Length; i++)
                {
                    for (int j = 0; j < VietNamChar[i].Length; j++)
                    {
                        strInput = strInput.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                    }
                }
                string KyTuDacBiet = "~!@#$%^&*+=|_«»\\/'\",.?/:;`<>(){}[]–";
                for (byte i = 0; i < KyTuDacBiet.Length; i++)
                {
                    strInput = strInput.Replace(KyTuDacBiet.Substring(i, 1), "");
                }
                strInput = strInput.Trim().Replace(" ", "-");
                //lặp cho đến khi khoog còn chuỗi -- thì thoát khỏi lặp
                for (int i = 0; i < 1000; i++)
                {
                    if (strInput.Contains("--"))
                        strInput = strInput.Replace("--", "-");
                    else
                        break;
                }
                return strInput.Replace("-–-","-").ToLower();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}
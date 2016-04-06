using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CRMViettour.Utilities
{
    public static class FileAction
    {
        public static string[] ImageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".JPG", ".PNG", ".PNG" };
        public static bool CheckImageExtension(string extension)
        {
            if (Array.IndexOf(ImageExtensions, extension) != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string SaveImageFile(HttpPostedFileBase file, string folder)
        {
            var fileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            try
            {
                if (CheckImageExtension(extension))
                {
                    var path = Path.Combine(HttpContext.Current.Server.MapPath(folder), fileName);
                    file.SaveAs(path);
                    return fileName;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }

        }
    }
}
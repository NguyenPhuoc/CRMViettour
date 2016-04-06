using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Core;

namespace CRMViettour.Models
{
    public class ActionModel
    {
        public bool Succeed { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string View { get; set; }
        public string RedirectTo { get; set; }
        public bool IsPartialView { get; set; }
    }
}
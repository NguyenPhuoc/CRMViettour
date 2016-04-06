using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class DocumentFileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Code { get; set; }
        public string URL { get; set; }
        public string FileSize { get; set; }
        public string TagsId { get; set; }
        public int DictionaryId { get; set; }
        public string DictionaryName { get; set; }
        public string Note { get; set; }
        public bool IsRead { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
    }
}
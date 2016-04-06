using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMViettour.Models
{
    public class TagsViewModel
    {
        public int Id { get; set; }
        public string Tags { get; set; }
        public int ParentId { get; set; }
    }

    public class SeededTagsViewModel
    {
        public int Seed { get; set; }
        public IList<TagsViewModel> Tags { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace GTYApp.Models
{
    public class HomepageContent
    {
        public int Id { get; set; }
        public string SectionName { get; set; } = "";
        public string? Content { get; set; }
    }
}

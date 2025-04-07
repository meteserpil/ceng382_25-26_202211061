using System.Collections.Generic;

namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public List<ClassInformationTableItem> Classes { get; set; } = new List<ClassInformationTableItem>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
    }

    public class ClassInformationTableItem
    {
        public int Id { get; set; }  // Used for actions but not displayed
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using System.Linq;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterMinStudents { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterMaxStudents { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public List<ClassInformationModel> Classes { get; set; } = new();
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }

        public void OnGet()
        {
            var query = ClassInformationModel.GetAllClasses().AsQueryable();

            if (!string.IsNullOrEmpty(FilterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
            }

            if (FilterMinStudents.HasValue)
            {
                query = query.Where(c => c.StudentCount >= FilterMinStudents.Value);
            }

            if (FilterMaxStudents.HasValue)
            {
                query = query.Where(c => c.StudentCount <= FilterMaxStudents.Value);
            }

            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            Classes = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid) return Page();
            ClassInformationModel.AddClass(NewClass);
            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = ClassInformationModel.GetClassById(id);
            if (classToEdit != null)
            {
                EditId = id;
                NewClass = new ClassInformationModel
                {
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
            }
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (ModelState.IsValid && EditId.HasValue)
            {
                ClassInformationModel.EditClass(
                    EditId.Value,
                    NewClass.ClassName,
                    NewClass.StudentCount,
                    NewClass.Description
                );
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            ClassInformationModel.DeleteClass(id);
            return RedirectToPage();
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage();
        }
    }
}
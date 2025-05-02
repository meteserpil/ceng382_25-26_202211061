using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MyRazorApp.Pages
{
    public class ClassInfo
    {
        public int Id { get; set; }
        [Required]
        public string ClassName { get; set; } = string.Empty;
        [Range(1, 1000)]
        public int StudentCount { get; set; }
        public string? Description { get; set; }
    }

    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInfo NewClass { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        public List<ClassInfo> Classes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? FilterClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterMinStudents { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterMaxStudents { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        private static List<ClassInfo> _allClasses = new();

        public IActionResult OnGet(int? currentPage)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            CurrentPage = currentPage ?? 1;

            var query = _allClasses.AsQueryable();

            if (!string.IsNullOrEmpty(FilterClassName))
                query = query.Where(c => c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
            if (FilterMinStudents.HasValue)
                query = query.Where(c => c.StudentCount >= FilterMinStudents);
            if (FilterMaxStudents.HasValue)
                query = query.Where(c => c.StudentCount <= FilterMaxStudents);

            TotalItems = query.Count();
            Classes = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }

        private bool IsUserAuthenticated()
        {
            // First check if session exists
            var usernameSession = HttpContext.Session.GetString("username");
            var tokenSession = HttpContext.Session.GetString("token");
            var sessionIdSession = HttpContext.Session.GetString("session_id");

            if (string.IsNullOrEmpty(usernameSession) || 
                string.IsNullOrEmpty(tokenSession) || 
                string.IsNullOrEmpty(sessionIdSession))
            {
                return false;
            }

            // Then verify cookies match session
            var usernameCookie = Request.Cookies["username"];
            var tokenCookie = Request.Cookies["token"];
            var sessionIdCookie = Request.Cookies["session_id"];

            return usernameCookie == usernameSession && 
                   tokenCookie == tokenSession && 
                   sessionIdCookie == sessionIdSession;
        }

        public IActionResult OnPostAdd()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            if (!ModelState.IsValid)
            {
                OnGet(CurrentPage);
                return Page();
            }

            NewClass.Id = _allClasses.Any() ? _allClasses.Max(c => c.Id) + 1 : 1;
            _allClasses.Add(NewClass);

            TempData["HighlightClassId"] = NewClass.Id;
            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            var classToEdit = _allClasses.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                EditId = classToEdit.Id;
                NewClass = new ClassInfo
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
            }

            OnGet(CurrentPage);
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            if (!ModelState.IsValid || !EditId.HasValue)
            {
                OnGet(CurrentPage);
                return Page();
            }

            var existing = _allClasses.FirstOrDefault(c => c.Id == EditId.Value);
            if (existing != null)
            {
                existing.ClassName = NewClass.ClassName;
                existing.StudentCount = NewClass.StudentCount;
                existing.Description = NewClass.Description;

                TempData["HighlightClassId"] = existing.Id;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            var toRemove = _allClasses.FirstOrDefault(c => c.Id == id);
            if (toRemove != null)
                _allClasses.Remove(toRemove);

            return RedirectToPage();
        }

        public IActionResult OnPostCancel()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            return RedirectToPage();
        }

        public IActionResult OnPostExportJson(string selectedColumns, bool exportFiltered)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            try
            {
                var selectedProps = !string.IsNullOrEmpty(selectedColumns) ? 
                    selectedColumns.Split(',').ToList() : new List<string>();

                var query = _allClasses.AsQueryable();

                if (exportFiltered)
                {
                    if (!string.IsNullOrEmpty(FilterClassName))
                        query = query.Where(c => c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
                    if (FilterMinStudents.HasValue)
                        query = query.Where(c => c.StudentCount >= FilterMinStudents);
                    if (FilterMaxStudents.HasValue)
                        query = query.Where(c => c.StudentCount <= FilterMaxStudents);
                }

                var exportData = query.ToList();
                var json = Utils.Instance.ExportToJson(exportData, selectedProps);
                
                return File(Encoding.UTF8.GetBytes(json), "application/json", "classes.json");
            }
            catch (Exception ex)
            {
                return Content("Error generating JSON file: " + ex.Message);
            }
        }
    }
}
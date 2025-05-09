using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Helpers;
using MyRazorApp.Models;
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
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> OnGetAsync(int? currentPage)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            CurrentPage = currentPage ?? 1;

            var query = _context.Classes.Where(c => c.IsActive).AsQueryable(); // Only active classes

            if (!string.IsNullOrEmpty(FilterClassName))
                query = query.Where(c => c.Name.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
            if (FilterMinStudents.HasValue)
                query = query.Where(c => c.PersonCount >= FilterMinStudents);
            if (FilterMaxStudents.HasValue)
                query = query.Where(c => c.PersonCount <= FilterMaxStudents);

            TotalItems = await query.CountAsync();
            Classes = await query
                .OrderBy(c => c.Id)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInfo
                {
                    Id = c.Id,
                    ClassName = c.Name,
                    StudentCount = c.PersonCount,
                    Description = c.Description
                })
                .ToListAsync();

            return Page();
        }

        private bool IsUserAuthenticated()
        {
            var usernameSession = HttpContext.Session.GetString("username");
            var tokenSession = HttpContext.Session.GetString("token");
            var sessionIdSession = HttpContext.Session.GetString("session_id");

            if (string.IsNullOrEmpty(usernameSession) ||
                string.IsNullOrEmpty(tokenSession) ||
                string.IsNullOrEmpty(sessionIdSession))
            {
                return false;
            }

            var usernameCookie = Request.Cookies["username"];
            var tokenCookie = Request.Cookies["token"];
            var sessionIdCookie = Request.Cookies["session_id"];

            return usernameCookie == usernameSession &&
                   tokenCookie == tokenSession &&
                   sessionIdCookie == sessionIdSession;
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync(CurrentPage);
                return Page();
            }

            var newClass = new Class
            {
                Name = NewClass.ClassName,
                PersonCount = NewClass.StudentCount,
                Description = NewClass.Description,
                IsActive = true
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            TempData["HighlightClassId"] = newClass.Id;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            var classToEdit = await _context.Classes.FindAsync(id);
            if (classToEdit != null)
            {
                EditId = classToEdit.Id;
                NewClass = new ClassInfo
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.Name,
                    StudentCount = classToEdit.PersonCount,
                    Description = classToEdit.Description
                };
            }

            await OnGetAsync(CurrentPage);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            if (!ModelState.IsValid || !EditId.HasValue)
            {
                await OnGetAsync(CurrentPage);
                return Page();
            }

            var existing = await _context.Classes.FindAsync(EditId.Value);
            if (existing != null)
            {
                existing.Name = NewClass.ClassName;
                existing.PersonCount = NewClass.StudentCount;
                existing.Description = NewClass.Description;

                _context.Classes.Update(existing);
                await _context.SaveChangesAsync();

                TempData["HighlightClassId"] = existing.Id;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            var classToDeactivate = await _context.Classes.FindAsync(id);
            if (classToDeactivate != null)
            {
                classToDeactivate.IsActive = false; // Soft delete
                _context.Classes.Update(classToDeactivate);
                await _context.SaveChangesAsync();
            }

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

        public async Task<IActionResult> OnPostExportJsonAsync(string selectedColumns, bool exportFiltered)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("Login");
            }

            try
            {
                var selectedProps = !string.IsNullOrEmpty(selectedColumns) ?
                    selectedColumns.Split(',').ToList() : new List<string>();

                var query = _context.Classes.Where(c => c.IsActive).AsQueryable(); // Export only active

                if (exportFiltered)
                {
                    if (!string.IsNullOrEmpty(FilterClassName))
                        query = query.Where(c => c.Name.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
                    if (FilterMinStudents.HasValue)
                        query = query.Where(c => c.PersonCount >= FilterMinStudents);
                    if (FilterMaxStudents.HasValue)
                        query = query.Where(c => c.PersonCount <= FilterMaxStudents);
                }

                var exportData = await query
                    .Select(c => new ClassInfo
                    {
                        Id = c.Id,
                        ClassName = c.Name,
                        StudentCount = c.PersonCount,
                        Description = c.Description
                    })
                    .ToListAsync();

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

// Pages/Logout.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Clear cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToPage("Login");
        }
    }
}
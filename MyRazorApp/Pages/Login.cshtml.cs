// Pages/Login.cshtml.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var users = await LoadUsersAsync();
            var user = users.Find(u => 
                u.Username == Username && 
                u.Password == HashPassword(Password) && 
                u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }

            // Generate token
            var token = GenerateToken();

            // Store in session
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            // Store in cookies (match session settings)
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = !HttpContext.Request.Host.Value.Contains("localhost"),
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

            return RedirectToPage("Index");
        }

        private async Task<List<User>> LoadUsersAsync()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
            
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (!System.IO.File.Exists(path) || new FileInfo(path).Length == 0)
            {
                var defaultUsers = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        Password = HashPassword("admin123"),
                        Role = "Administrator",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await System.IO.File.WriteAllTextAsync(path, JsonSerializer.Serialize(defaultUsers));
                return defaultUsers;
            }

            try
            {
                var json = await System.IO.File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private string GenerateToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

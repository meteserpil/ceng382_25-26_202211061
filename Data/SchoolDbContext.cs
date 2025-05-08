using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace MyRazorApp.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<Class> Classes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>().HasData(GenerateInitialClasses());
        }

        private static List<Class> GenerateInitialClasses()
        {
            var random = new Random();
            var classList = new List<Class>();
            string[] subjects = { "Math", "Science", "History", "English", "Physics", "Chemistry", "Biology", "Art", "Music", "Geography" };
            string[] levels = { "101", "201", "301", "Advanced", "Basic", "Intro" };
            string[] descriptors = { "Fundamentals", "Principles", "Concepts", "Applications", "Theory" };

            for (int id = 1; id <= 100; id++)
            {
                classList.Add(new Class
                {
                    Id = id,
                    Name = $"{subjects[random.Next(subjects.Length)]} {levels[random.Next(levels.Length)]}",
                    PersonCount = random.Next(15, 50),
                    Description = $"{levels[random.Next(levels.Length)]} {descriptors[random.Next(descriptors.Length)]}",
                    IsActive = true
                });
            }
            return classList;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace MyRazorApp.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Description { get; set; } = string.Empty;

        private static readonly List<ClassInformationModel> _classes = new();
        private static int _nextId = 1;

        public static List<ClassInformationModel> GetAllClasses() => _classes;

        public static ClassInformationModel? GetClassById(int id) => _classes.FirstOrDefault(c => c.Id == id);

        public static void AddClass(ClassInformationModel classInfo)
        {
            classInfo.Id = _nextId++;
            _classes.Add(classInfo);
        }

        public static void EditClass(int id, string className, int studentCount, string description)
        {
            var classToEdit = GetClassById(id);
            if (classToEdit != null)
            {
                classToEdit.ClassName = className;
                classToEdit.StudentCount = studentCount;
                classToEdit.Description = description;
            }
        }

        public static bool DeleteClass(int id)
        {
            var classToDelete = GetClassById(id);
            if (classToDelete == null) return false;
            
            return _classes.Remove(classToDelete);
        }

        public static ClassInformationTable GetFilteredTable(
            string? filterClassName = null,
            int? filterMinStudents = null,
            int? filterMaxStudents = null,
            int currentPage = 1,
            int pageSize = 10)
        {
            var query = _classes.AsQueryable();

            if (!string.IsNullOrEmpty(filterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(filterClassName, System.StringComparison.OrdinalIgnoreCase));
            }

            if (filterMinStudents.HasValue)
            {
                query = query.Where(c => c.StudentCount >= filterMinStudents.Value);
            }

            if (filterMaxStudents.HasValue)
            {
                query = query.Where(c => c.StudentCount <= filterMaxStudents.Value);
            }

            var totalItems = query.Count();
            var totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

            var items = query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClassInformationTableItem
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();

            return new ClassInformationTable
            {
                Classes = items,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalItems = totalItems,
                FilterClassName = filterClassName,
                FilterMinStudents = filterMinStudents,
                FilterMaxStudents = filterMaxStudents
            };
        }
    }
}
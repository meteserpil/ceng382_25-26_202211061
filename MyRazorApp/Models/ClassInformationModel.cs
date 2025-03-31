using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyRazorApp.Models;

public class ClassInformationModel
{
    // Static fields for in-memory storage
    private static readonly List<ClassInformationModel> _classList = new();
    private static int _idCounter = 1;

    public ClassInformationModel()
    {
        Id = _idCounter++;
        ClassName = string.Empty;
        Description = string.Empty;
        StudentCount = 1; // Default value
    }

    // Properties
    public int Id { get; private set; }

    [Required(ErrorMessage = "Class name is required")]
    [StringLength(100, ErrorMessage = "Class name cannot exceed 100 characters")]
    public string ClassName { get; set; }

    [Required(ErrorMessage = "Student count is required")]
    [Range(1, 1000, ErrorMessage = "Student count must be between 1 and 1000")]
    public int StudentCount { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    // CRUD Operations
    public static void AddClass(ClassInformationModel newClass)
    {
        lock (_classList) // Thread-safe addition
        {
            _classList.Add(newClass);
        }
    }

    public static bool EditClass(int id, string className, int studentCount, string description)
    {
        var classToEdit = _classList.FirstOrDefault(c => c.Id == id);
        if (classToEdit == null) return false;

        lock (classToEdit) // Thread-safe edit
        {
            classToEdit.ClassName = className;
            classToEdit.StudentCount = studentCount;
            classToEdit.Description = description;
        }
        return true;
    }

    public static bool DeleteClass(int id)
    {
        var classToRemove = _classList.FirstOrDefault(c => c.Id == id);
        if (classToRemove == null) return false;

        lock (_classList) // Thread-safe removal
        {
            return _classList.Remove(classToRemove);
        }
    }

    public static ClassInformationModel? GetClassById(int id)
    {
        return _classList.FirstOrDefault(c => c.Id == id);
    }

    public static List<ClassInformationModel> GetAllClasses()
    {
        lock (_classList) // Thread-safe read
        {
            return new List<ClassInformationModel>(_classList); // Return a copy
        }
    }

    // Helper method to reset the static list (for testing)
    public static void ClearAllClasses()
    {
        lock (_classList)
        {
            _classList.Clear();
            _idCounter = 1;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;

namespace MyRazorApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public ClassInformationModel NewClass { get; set; } = new();

    [BindProperty]
    public int? EditId { get; set; }

    public List<ClassInformationModel> AllClasses => ClassInformationModel.GetAllClasses();

    public void OnGet()
    {
        // Add 100 sample classes if the database is empty
        if (AllClasses.Count == 0)
        {
            int counter = 1;
            while (counter <= 100)
            {
                var sampleClass = new ClassInformationModel
                {
                    ClassName = $"Sample Class {counter}",
                    StudentCount = new Random().Next(10, 50),
                    Description = $"This is a sample class description for class {counter}."
                };
                
                ClassInformationModel.AddClass(sampleClass);
                counter++;
            }
        }
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Create a NEW instance to add (critical fix)
        var classToAdd = new ClassInformationModel
        {
            ClassName = NewClass.ClassName,
            StudentCount = NewClass.StudentCount,
            Description = NewClass.Description
        };

        ClassInformationModel.AddClass(classToAdd);
        
        // Reset the form
        NewClass = new ClassInformationModel();
        
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
            EditId = null;
            NewClass = new ClassInformationModel();
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
        EditId = null;
        NewClass = new ClassInformationModel();
        return RedirectToPage();
    }
}

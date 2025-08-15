using Microsoft.AspNetCore.Mvc;
using StudentSystem.WebApp.Models;
using StudentSystem.WebApp.RESTClients;
using StudentSystem.WebApp.ViewModels;

namespace StudentSystem.WebApp.Controllers;

public class StudentManagementController : Controller
{
    private readonly IStudentManagementAPI _studentManagementAPI;

    public StudentManagementController(IStudentManagementAPI studentManagementAPI)
    {
        _studentManagementAPI = studentManagementAPI;
    }

    public async Task<IActionResult> Index()
    {
        var model = new StudentManagementViewModel
        {
            Students = await _studentManagementAPI.GetStudents()
        };
        return View(model);
    }

    public IActionResult New()
    {
        var model = new StudentManagementNewViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> New(StudentManagementNewViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _studentManagementAPI.RegisterStudent(new Student
                {
                    StudentId = model.StudentId,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth.Value,
                    EmailAddress = model.EmailAddress
                });
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while registering the student.");
            }
        }
        return View(model);
    }
}
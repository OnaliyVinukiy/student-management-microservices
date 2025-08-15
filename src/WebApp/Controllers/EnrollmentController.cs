using Microsoft.AspNetCore.Mvc;
using StudentSystem.WebApp.Models;
using StudentSystem.WebApp.RESTClients;
using StudentSystem.WebApp.ViewModels;

namespace StudentSystem.WebApp.Controllers;

public class EnrollmentController : Controller
{
    private readonly IEnrollmentAPI _enrollmentAPI;
    private readonly IStudentManagementAPI _studentAPI;
    private readonly ICourseManagementAPI _courseAPI;

    public EnrollmentController(IEnrollmentAPI enrollmentAPI, IStudentManagementAPI studentAPI, ICourseManagementAPI courseAPI)
    {
        _enrollmentAPI = enrollmentAPI;
        _studentAPI = studentAPI;
        _courseAPI = courseAPI;
    }

    public async Task<IActionResult> Index()
    {
        // For simplicity, we'll always look at the current academic term.
        // A real app might have a term selector.
        var today = DateOnly.FromDateTime(DateTime.Now);
        var termId = $"{today.Year}-{(today.Month >= 8 ? "FALL" : "SPRING")}";

        var model = new EnrollmentViewModel
        {
            Term = await _enrollmentAPI.GetAcademicTerm(termId) ?? new AcademicTerm { Id = termId, Enrollments = new List<Enrollment>() }
        };

        return View(model);
    }

    public async Task<IActionResult> New()
    {
        var students = await _studentAPI.GetStudents();
        var courses = await _courseAPI.GetCourses();

        var model = new EnrollmentNewViewModel
        {
            Students = new(students, "StudentId", "FullName"),
            Courses = new(courses, "CourseCode", "CourseName")
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> New(EnrollmentNewViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var termId = $"{today.Year}-{(today.Month >= 8 ? "FALL" : "SPRING")}";

                var enrollment = new Enrollment
                {
                    Id = Guid.NewGuid(), // The API expects an ID for the new enrollment
                    Student = new Student { StudentId = model.SelectedStudentId },
                    Course = new Course { CourseCode = model.SelectedCourseCode },
                    EnrollmentDate = DateTime.Now
                };

                await _enrollmentAPI.RegisterEnrollment(termId, enrollment);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }
        }

        // Repopulate dropdowns if model state is invalid
        var students = await _studentAPI.GetStudents();
        var courses = await _courseAPI.GetCourses();
        model.Students = new(students, "StudentId", "FullName");
        model.Courses = new(courses, "CourseCode", "CourseName");
        
        return View(model);
    }
}
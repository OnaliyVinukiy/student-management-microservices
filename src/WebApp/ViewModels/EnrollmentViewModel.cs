using StudentSystem.WebApp.Models;

namespace StudentSystem.WebApp.ViewModels;

public class EnrollmentViewModel
{
    public AcademicTerm Term { get; set; } = new() { Enrollments = new List<Enrollment>() };
}
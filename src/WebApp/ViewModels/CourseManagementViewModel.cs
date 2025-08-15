using StudentSystem.WebApp.Models;

namespace StudentSystem.WebApp.ViewModels;

public class CourseManagementViewModel
{
    public List<Course> Courses { get; set; } = new();
}
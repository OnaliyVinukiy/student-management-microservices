using StudentSystem.WebApp.Models;

namespace StudentSystem.WebApp.RESTClients;

public interface ICourseManagementAPI
{
    Task<List<Course>> GetCourses();
    Task<Course> GetCourseById(string courseCode);
    Task RegisterCourse(Course course);
}
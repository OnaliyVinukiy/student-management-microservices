namespace StudentSystem.EnrollmentManagementAPI.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<CourseDTO>> GetCoursesAsync();
    Task<CourseDTO> GetCourseAsync(string courseCode);
}
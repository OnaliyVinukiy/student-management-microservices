namespace StudentSystem.EnrollmentManagementAPI.CommandHandlers;

public interface IEnrollStudentInCourseCommandHandler
{
    Task<AcademicTerm> HandleCommandAsync(DateOnly termDate, EnrollStudentInCourse command);
}
namespace StudentSystem.EnrollmentManagementAPI.CommandHandlers;

public interface ICompleteCourseWithGradeCommandHandler
{
    Task<AcademicTerm> HandleCommandAsync(DateOnly termDate, CompleteCourseWithGrade command);
}
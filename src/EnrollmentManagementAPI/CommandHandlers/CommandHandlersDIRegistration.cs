namespace StudentSystem.EnrollmentManagementAPI.CommandHandlers;

public static class CommandHandlersDIRegistration
{
    public static void AddCommandHandlers(this IServiceCollection services)
    {
        services.AddTransient<IEnrollStudentInCourseCommandHandler, EnrollStudentInCourseCommandHandler>();
        services.AddTransient<ICompleteCourseWithGradeCommandHandler, CompleteCourseWithGradeCommandHandler>();
    }
}
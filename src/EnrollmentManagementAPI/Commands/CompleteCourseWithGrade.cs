namespace StudentSystem.EnrollmentManagementAPI.Commands;

public class CompleteCourseWithGrade : Command
{
    public readonly Guid EnrollmentId;
    public readonly string Grade;

    public CompleteCourseWithGrade(Guid messageId, Guid enrollmentId, string grade) : base(messageId)
    {
        EnrollmentId = enrollmentId;
        Grade = grade;
    }
}
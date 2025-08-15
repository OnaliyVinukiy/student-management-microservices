namespace StudentSystem.NotificationService.Events;

public class CourseCompleted : Event
{
    public readonly Guid EnrollmentId;
    public readonly string Grade;

    public CourseCompleted(Guid messageId, Guid enrollmentId, string grade) :
        base(messageId)
    {
        EnrollmentId = enrollmentId;
        Grade = grade;
    }
}
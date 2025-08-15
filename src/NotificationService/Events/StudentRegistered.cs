namespace StudentSystem.NotificationService.Events;

public class StudentRegistered : Event
{
    public readonly string StudentId;
    public readonly string FullName;
    public readonly string EmailAddress;

    public StudentRegistered(Guid messageId, string studentId, string fullName, string emailAddress) :
        base(messageId)
    {
        StudentId = studentId;
        FullName = fullName;
        EmailAddress = emailAddress;
    }
}
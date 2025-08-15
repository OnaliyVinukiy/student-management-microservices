namespace StudentSystem.StudentManagementAPI.Events;

public class StudentRegistered : Event
{
    public readonly string StudentId;
    public readonly string FullName;
    public readonly DateTime DateOfBirth;
    public readonly string EmailAddress;

    public StudentRegistered(Guid messageId, string studentId, string fullName, DateTime dateOfBirth, string emailAddress) : base(messageId)
    {
        StudentId = studentId;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        EmailAddress = emailAddress;
    }
}
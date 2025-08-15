namespace StudentSystem.StudentManagementAPI.Commands;

public class RegisterStudent : Command
{
    public readonly string StudentId;
    public readonly string FullName;
    public readonly DateTime DateOfBirth;
    public readonly string EmailAddress;

    public RegisterStudent(Guid messageId, string studentId, string fullName, DateTime dateOfBirth, string emailAddress) : base(messageId)
    {
        StudentId = studentId;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        EmailAddress = emailAddress;
    }
}
namespace StudentSystem.EnrollmentManagementAPI.Domain.Entities;

public class Student : Entity<string>
{
    public string FullName { get; private set; }
    public string EmailAddress { get; private set; }

    public Student(string studentId, string fullName, string emailAddress) : base(studentId)
    {
        FullName = fullName;
        EmailAddress = emailAddress;
    }
}
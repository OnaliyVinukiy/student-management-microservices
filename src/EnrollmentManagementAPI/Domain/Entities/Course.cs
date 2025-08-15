namespace StudentSystem.EnrollmentManagementAPI.Domain.Entities;

public class Course : Entity<string>
{
    public string CourseName { get; private set; }
    public string Department { get; private set; }

    public Course(string courseCode, string courseName, string department) : base(courseCode)
    {
        CourseName = courseName;
        Department = department;
    }
}
namespace StudentSystem.EnrollmentEventHandler.Events;

public class CourseCreated : Event
{
    public readonly string CourseCode;
    public readonly string CourseName;
    public readonly string Department;
    public readonly int CreditHours;

    public CourseCreated(Guid messageId, string courseCode, string courseName, string department, int creditHours) : base(messageId)
    {
        CourseCode = courseCode;
        CourseName = courseName;
        Department = department;
        CreditHours = creditHours;
    }
}
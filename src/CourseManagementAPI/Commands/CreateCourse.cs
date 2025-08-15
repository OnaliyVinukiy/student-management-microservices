namespace StudentSystem.CourseManagement.Commands;

public class CreateCourse : Command
{
    public readonly string CourseCode;
    public readonly string CourseName;
    public readonly string Description;
    public readonly string Department;
    public readonly int CreditHours;

    public CreateCourse(Guid messageId, string courseCode, string courseName, string description, string department, int creditHours) :
        base(messageId)
    {
        CourseCode = courseCode;
        CourseName = courseName;
        Description = description;
        Department = department;
        CreditHours = creditHours;
    }
}
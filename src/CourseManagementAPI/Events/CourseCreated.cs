namespace StudentSystem.CourseManagement.Events;

public class CourseCreated : Event
{
    public readonly string CourseCode;
    public readonly string CourseName;
    public readonly string Description;
    public readonly string Department;
    public readonly int CreditHours;

    public CourseCreated(Guid messageId, string courseCode, string courseName, string description, string department, int creditHours) :
        base(messageId)
    {
        CourseCode = courseCode;
        CourseName = courseName;
        Description = description;
        Department = department;
        CreditHours = creditHours;
    }

    public static CourseCreated FromCommand(CreateCourse command)
    {
        return new CourseCreated(
            Guid.NewGuid(),
            command.CourseCode,
            command.CourseName,
            command.Description,
            command.Department,
            command.CreditHours
        );
    }
}
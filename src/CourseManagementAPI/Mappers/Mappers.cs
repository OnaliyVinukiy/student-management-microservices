namespace StudentSystem.CourseManagementAPI.Mappers;

public static class Mappers
{
    public static Course MapToCourse(this CreateCourse command) => new Course
    {
        CourseCode = command.CourseCode,
        CourseName = command.CourseName,
        Description = command.Description,
        Department = command.Department,
        CreditHours = command.CreditHours
    };
}
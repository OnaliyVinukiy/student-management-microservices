namespace StudentSystem.EnrollmentManagementAPI.Commands;

public class EnrollStudentInCourse : Command
{
    public readonly Guid EnrollmentId;
    public readonly DateTime EnrollmentDate;
    public readonly string StudentId;
    public readonly string CourseCode;

    public EnrollStudentInCourse(Guid messageId, Guid enrollmentId, DateTime enrollmentDate, string studentId, string courseCode) : base(messageId)
    {
        EnrollmentId = enrollmentId;
        EnrollmentDate = enrollmentDate;
        StudentId = studentId;
        CourseCode = courseCode;
    }
}
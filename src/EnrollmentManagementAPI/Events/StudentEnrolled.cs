namespace StudentSystem.EnrollmentManagementAPI.Events;

public class StudentEnrolled : Event
{
    public readonly Guid EnrollmentId;
    public readonly DateTime EnrollmentDate;
    public readonly string StudentId;
    public readonly string CourseCode;

    public StudentEnrolled(Guid messageId, Guid enrollmentId, DateTime enrollmentDate, string studentId, string courseCode) : base(messageId)
    {
        EnrollmentId = enrollmentId;
        EnrollmentDate = enrollmentDate;
        StudentId = studentId;
        CourseCode = courseCode;
    }
}
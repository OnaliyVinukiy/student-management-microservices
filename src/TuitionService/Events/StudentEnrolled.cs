namespace StudentSystem.TuitionService.Events;

public class StudentEnrolled : Event
{
    public readonly Guid EnrollmentId;
    public readonly string StudentId;
    public readonly string CourseCode;
    public readonly decimal Cost;

    public StudentEnrolled(Guid messageId, Guid enrollmentId, string studentId, string courseCode, decimal cost) : base(messageId)
    {
        EnrollmentId = enrollmentId;
        StudentId = studentId;
        CourseCode = courseCode;
        Cost = cost;
    }
}
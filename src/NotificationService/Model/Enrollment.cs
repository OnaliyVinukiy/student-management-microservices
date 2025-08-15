namespace StudentSystem.NotificationService.Model;

public class Enrollment
{
    public string EnrollmentId { get; set; } 
    public string CourseCode { get; set; }
    public string StudentId { get; set; }
    public DateTime StartTime { get; set; }
    public string Description { get; set; }
}
namespace StudentSystem.EnrollmentEventHandler.Model;

public class Enrollment
{
    public Guid EnrollmentId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string CourseCode { get; set; }
    public Course Course { get; set; }
    public string StudentId { get; set; }
    public Student Student { get; set; }
    public string Grade { get; set; } 
}
namespace StudentSystem.WebApp.Models;

public class Enrollment {
    public Guid Id { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }
    public string Status { get; set; }
    public string Grade { get; set; }
}
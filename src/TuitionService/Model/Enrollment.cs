namespace StudentSystem.TuitionService.Model;

public class Enrollment
{
    public string EnrollmentId { get; set; }
    public string CourseCode { get; set; }
    public string StudentId { get; set; }
    public decimal Amount { get; set; } 
    public bool BillSent { get; set; }
}
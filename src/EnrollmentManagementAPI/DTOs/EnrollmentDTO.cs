namespace StudentSystem.EnrollmentManagementAPI.DTOs;

public class EnrollmentDTO
{
    public Guid Id { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public StudentDTO Student { get; set; }
    public CourseDTO Course { get; set; }
    public string Status { get; set; }
    public string Grade { get; set; }
}
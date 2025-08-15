namespace StudentSystem.EnrollmentManagementAPI.DTOs;

public class AcademicTermDTO
{
    public string Id { get; set; }
    public List<EnrollmentDTO> Enrollments { get; set; }
}
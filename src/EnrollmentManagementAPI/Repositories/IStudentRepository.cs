namespace StudentSystem.EnrollmentManagementAPI.Repositories;

public interface IStudentRepository
{
    Task<IEnumerable<StudentDTO>> GetStudentsAsync();
    Task<StudentDTO> GetStudentAsync(string studentId);
}
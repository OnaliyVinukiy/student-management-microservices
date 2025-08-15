namespace StudentSystem.EnrollmentManagementAPI.Repositories;

public class SqlServerRefDataRepository : ICourseRepository, IStudentRepository
{
    private readonly string _connectionString;

    public SqlServerRefDataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<StudentDTO>> GetStudentsAsync()
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<StudentDTO>("SELECT StudentId, FullName, EmailAddress FROM Student");
    }

    public async Task<StudentDTO> GetStudentAsync(string studentId)
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<StudentDTO>(
            "SELECT StudentId, FullName, EmailAddress FROM Student WHERE StudentId = @StudentId",
            new { StudentId = studentId });
    }

    public async Task<IEnumerable<CourseDTO>> GetCoursesAsync()
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<CourseDTO>("SELECT CourseCode, CourseName, Department, CreditHours FROM Course");
    }

    public async Task<CourseDTO> GetCourseAsync(string courseCode)
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<CourseDTO>(
            "SELECT CourseCode, CourseName, Department, CreditHours FROM Course WHERE CourseCode = @CourseCode",
            new { CourseCode = courseCode });
    }
}
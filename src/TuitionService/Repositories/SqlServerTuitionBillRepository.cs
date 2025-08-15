namespace StudentSystem.TuitionService.Repositories;

public class SqlServerTuitionBillRepository : ITuitionBillRepository
{
    private readonly string _connectionString;

    public SqlServerTuitionBillRepository(string connectionString)
    {
        _connectionString = connectionString;
        Policy.Handle<Exception>()
            .WaitAndRetryAsync(10, r => TimeSpan.FromSeconds(10), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 10 sec."); })
            .ExecuteAsync(InitializeDBAsync).Wait();
    }

    public async Task<Student> GetStudentAsync(string studentId)
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<Student>("SELECT * FROM Student WHERE StudentId = @StudentId", new { StudentId = studentId });
    }

    public async Task RegisterEnrollmentAsync(Enrollment enrollment)
    {
        using var conn = new SqlConnection(_connectionString);
        string sql = "INSERT INTO Enrollment(EnrollmentId, CourseCode, StudentId, Amount, BillSent) VALUES(@EnrollmentId, @CourseCode, @StudentId, @Amount, 0);";
        await conn.ExecuteAsync(sql, enrollment);
    }

    public async Task RegisterStudentAsync(Student student)
    {
        using var conn = new SqlConnection(_connectionString);
        string sql = "INSERT INTO Student(StudentId, FullName, EmailAddress) VALUES(@StudentId, @FullName, @EmailAddress);";
        await conn.ExecuteAsync(sql, student);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsToBillAsync()
    {
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<Enrollment>("SELECT * FROM Enrollment WHERE BillSent = 0");
    }

    public async Task RegisterTuitionBillAsync(TuitionBill tuitionBill)
    {
        using var conn = new SqlConnection(_connectionString);
        // Persist tuition bill
        string sql = "INSERT INTO TuitionBill(TuitionBillId, TuitionBillDate, StudentId, Amount, Specification, EnrollmentIds) " +
                     "VALUES(@TuitionBillId, @TuitionBillDate, @StudentId, @Amount, @Specification, @EnrollmentIds);";
        await conn.ExecuteAsync(sql, tuitionBill);

        // Update enrollments to indicate bill sent
        var enrollmentIds = tuitionBill.EnrollmentIds.Split('|').Select(id => new { EnrollmentId = id });
        sql = "UPDATE Enrollment SET BillSent = 1 WHERE EnrollmentId = @EnrollmentId";
        await conn.ExecuteAsync(sql, enrollmentIds);
    }

    private async Task InitializeDBAsync()
    {
        using (var conn = new SqlConnection(_connectionString.Replace("Tuition", "master")))
        {
            await conn.OpenAsync();
            await conn.ExecuteAsync("IF NOT EXISTS(SELECT * FROM master.sys.databases WHERE name='Tuition') CREATE DATABASE Tuition;");
        }

        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            string sql = @"
                IF OBJECT_ID('Student') IS NULL CREATE TABLE Student (StudentId VARCHAR(50) PRIMARY KEY, FullName VARCHAR(250) NOT NULL, EmailAddress VARCHAR(250));
                IF OBJECT_ID('Enrollment') IS NULL CREATE TABLE Enrollment (EnrollmentId VARCHAR(50) PRIMARY KEY, CourseCode VARCHAR(50) NOT NULL, StudentId VARCHAR(50) NOT NULL, Amount DECIMAL(10,2) NOT NULL, BillSent BIT NOT NULL);
                IF OBJECT_ID('TuitionBill') IS NULL CREATE TABLE TuitionBill (TuitionBillId VARCHAR(50) PRIMARY KEY, TuitionBillDate DATETIME2 NOT NULL, StudentId VARCHAR(50) NOT NULL, Amount DECIMAL(10,2) NOT NULL, Specification TEXT, EnrollmentIds VARCHAR(250));
            ";
            await conn.ExecuteAsync(sql);
        }
    }
}
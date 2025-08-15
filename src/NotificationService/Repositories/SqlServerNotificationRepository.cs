using System.Data.SqlClient;
using StudentSystem.NotificationService.Model;
using Polly;
using Serilog;
using Dapper;

namespace StudentSystem.NotificationService.Repositories;

public class SqlServerNotificationRepository : INotificationRepository
{
    private readonly string _connectionString;

    public SqlServerNotificationRepository(string connectionString)
    {
        _connectionString = connectionString;

        // init db
        Log.Information("Initialize Database");

        Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(10, r => TimeSpan.FromSeconds(10), (ex, ts) => { Log.Error("Error connecting to DB. Retrying in 10 sec."); })
            .ExecuteAsync(InitializeDBAsync)
            .Wait();
    }

    public async Task<Student> GetStudentAsync(string studentId)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            return await conn.QueryFirstOrDefaultAsync<Student>("select * from Student where StudentId = @StudentId", new { StudentId = studentId });
        }
    }

    public async Task RegisterEnrollmentAsync(Enrollment enrollment)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            string sql =
                "insert into Enrollment(EnrollmentId, CourseCode, StudentId, StartTime, Description) " +
                "values(@EnrollmentId, @CourseCode, @StudentId, @StartTime, @Description);";
            await conn.ExecuteAsync(sql, enrollment);
        }
    }

    public async Task RegisterStudentAsync(Student student)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            string sql =
                "insert into Student(StudentId, FullName, EmailAddress) " +
                "values(@StudentId, @FullName, @EmailAddress);";
            await conn.ExecuteAsync(sql, student);
        }
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsForTodayAsync(DateTime date)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            return await conn.QueryAsync<Enrollment>(
                "select * from Enrollment where StartTime >= @Today and StartTime < @Tomorrow",
                new { Today = date.Date, Tomorrow = date.AddDays(1).Date });
        }
    }

    public async Task RemoveEnrollmentsAsync(IEnumerable<string> enrollmentIds)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            string sql =
                "delete Enrollment " +
                "where EnrollmentId = @EnrollmentId;";
            await conn.ExecuteAsync(sql, enrollmentIds.Select(id => new { EnrollmentId = id }));
        }
    }

    private async Task InitializeDBAsync()
    {
        // Note: database name "Notification" is fixed by the connection string
        using (var conn = new SqlConnection(_connectionString.Replace("Notification", "master")))
        {
            await conn.OpenAsync();
            await conn.ExecuteAsync("IF NOT EXISTS(SELECT * FROM master.sys.databases WHERE name='Notification') CREATE DATABASE Notification;");
        }

        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            // Create tables with new column names
            string sql = @" 
                IF OBJECT_ID('Student') IS NULL 
                CREATE TABLE Student (
                    StudentId varchar(50) NOT NULL PRIMARY KEY,
                    FullName varchar(200) NOT NULL,
                    EmailAddress varchar(200)
                );
                IF OBJECT_ID('Enrollment') IS NULL 
                CREATE TABLE Enrollment (
                    EnrollmentId varchar(50) NOT NULL PRIMARY KEY,
                    CourseCode varchar(50) NOT NULL,
                    StudentId varchar(50) NOT NULL,
                    StartTime datetime2 NOT NULL,
                    Description varchar(250) NOT NULL
                );";
            await conn.ExecuteAsync(sql);
        }
    }
}
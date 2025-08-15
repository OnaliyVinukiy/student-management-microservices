using StudentSystem.EnrollmentEventHandler.DataAccess;
namespace StudentSystem.EnrollmentEventHandler;

public class EventHandlerWorker : IHostedService, IMessageHandlerCallback
{
    private readonly EnrollmentDBContext _dbContext;
    private readonly IMessageHandler _messageHandler;

    public EventHandlerWorker(IMessageHandler messageHandler, EnrollmentDBContext dbContext)
    {
        _messageHandler = messageHandler;
        _dbContext = dbContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _messageHandler.Start(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _messageHandler.Stop();
        return Task.CompletedTask;
    }

    public async Task<bool> HandleMessageAsync(string messageType, string message)
    {
        try
        {
            JObject messageObject = MessageSerializer.Deserialize(message);
            switch (messageType)
            {
                case "StudentRegistered":
                    await HandleAsync(messageObject.ToObject<StudentRegistered>());
                    break;
                case "CourseCreated":
                    await HandleAsync(messageObject.ToObject<CourseCreated>());
                    break;
                case "StudentEnrolled":
                    await HandleAsync(messageObject.ToObject<StudentEnrolled>());
                    break;
                case "CourseCompleted":
                    await HandleAsync(messageObject.ToObject<CourseCompleted>());
                    break;
                default:
                    Log.Warning("Unknown message type: {MessageType}", messageType);
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while handling message of type {MessageType}.", messageType);
        }

        // Always acknowledge the message, so it's not redelivered.
        return true;
    }

    private async Task HandleAsync(StudentRegistered e)
    {
        Log.Information("Handling StudentRegistered event: {StudentId}, {FullName}", e.StudentId, e.FullName);
        try
        {
            var student = new Student
            {
                StudentId = e.StudentId,
                FullName = e.FullName,
                EmailAddress = e.EmailAddress
            };
            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            Log.Warning("Skipped adding student with Id {StudentId} as it already exists.", e.StudentId);
        }
    }

    private async Task HandleAsync(CourseCreated e)
    {
        Log.Information("Handling CourseCreated event: {CourseCode}, {CourseName}", e.CourseCode, e.CourseName);
        try
        {
            var course = new Course
            {
                CourseCode = e.CourseCode,
                CourseName = e.CourseName,
                Department = e.Department,
                CreditHours = e.CreditHours
            };
            await _dbContext.Courses.AddAsync(course);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            Log.Warning("Skipped adding course with code {CourseCode} as it already exists.", e.CourseCode);
        }
    }

    private async Task HandleAsync(StudentEnrolled e)
    {
        Log.Information("Handling StudentEnrolled event: {StudentId} in {CourseCode}", e.StudentId, e.CourseCode);
        try
        {
            var enrollment = new Enrollment
            {
                EnrollmentId = e.EnrollmentId,
                EnrollmentDate = e.EnrollmentDate,
                StudentId = e.StudentId,
                CourseCode = e.CourseCode,
                Grade = "In Progress" // Default grade upon enrollment
            };
            await _dbContext.Enrollments.AddAsync(enrollment);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            Log.Warning("Skipped adding enrollment with Id {EnrollmentId} as it already exists.", e.EnrollmentId);
        }
    }

    private async Task HandleAsync(CourseCompleted e)
    {
        Log.Information("Handling CourseCompleted event for enrollment: {EnrollmentId}, Grade: {Grade}", e.EnrollmentId, e.Grade);
        try
        {
            var enrollment = await _dbContext.Enrollments.FindAsync(e.EnrollmentId);
            if (enrollment != null)
            {
                enrollment.Grade = e.Grade;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Log.Warning("Could not find enrollment with Id {EnrollmentId} to update grade.", e.EnrollmentId);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while updating grade for enrollment {EnrollmentId}", e.EnrollmentId);
        }
    }
}
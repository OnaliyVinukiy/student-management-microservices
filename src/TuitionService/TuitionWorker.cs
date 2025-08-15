namespace StudentSystem.TuitionService;

public class TuitionWorker : IHostedService, IMessageHandlerCallback
{
    private readonly IMessageHandler _messageHandler;
    private readonly ITuitionBillRepository _repo;
    private readonly IEmailCommunicator _emailCommunicator;

    public TuitionWorker(IMessageHandler messageHandler, ITuitionBillRepository repo, IEmailCommunicator emailCommunicator)
    {
        _messageHandler = messageHandler;
        _repo = repo;
        _emailCommunicator = emailCommunicator;
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
                case "StudentEnrolled":
                    await HandleAsync(messageObject.ToObject<StudentEnrolled>());
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error while handling {messageType} event.");
        }
        return true;
    }

    private async Task HandleAsync(StudentRegistered sr)
    {
        Log.Information("Registering student: {Id}, {Name}", sr.StudentId, sr.FullName);
        var student = new Student { StudentId = sr.StudentId, FullName = sr.FullName, EmailAddress = sr.EmailAddress };
        await _repo.RegisterStudentAsync(student);
    }

    private async Task HandleAsync(StudentEnrolled se)
    {
        Log.Information("Handling enrollment for billing: {EnrollmentId}, Student: {StudentId}, Course: {CourseCode}, Cost: {Cost}",
            se.EnrollmentId, se.StudentId, se.CourseCode, se.Cost);

        // Persist the enrollment locally, ready to be billed
        var enrollment = new Enrollment
        {
            EnrollmentId = se.EnrollmentId.ToString(),
            StudentId = se.StudentId,
            CourseCode = se.CourseCode,
            Amount = se.Cost
        };
        await _repo.RegisterEnrollmentAsync(enrollment);

        // For simplicity, we'll generate and send the bill immediately.
        // A more complex system might use the DayHasPassed event to batch them.
        await GenerateAndSendBillForStudent(se.StudentId);
    }

    private async Task GenerateAndSendBillForStudent(string studentId)
    {
        var enrollmentsToBill = (await _repo.GetEnrollmentsToBillAsync()).Where(e => e.StudentId == studentId);
        if (!enrollmentsToBill.Any())
        {
            Log.Information("No new enrollments to bill for student {StudentId}", studentId);
            return;
        }

        var student = await _repo.GetStudentAsync(studentId);
        if (student == null)
        {
            Log.Warning("Student with Id {StudentId} not found, cannot send bill.", studentId);
            return;
        }

        var tuitionBill = new TuitionBill
        {
            TuitionBillId = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{studentId}",
            TuitionBillDate = DateTime.UtcNow,
            StudentId = studentId,
            EnrollmentIds = string.Join('|', enrollmentsToBill.Select(e => e.EnrollmentId))
        };

        var specification = new StringBuilder();
        decimal totalAmount = 0;
        foreach (var enrollment in enrollmentsToBill)
        {
            specification.AppendLine($"Tuition for course {enrollment.CourseCode}: ${enrollment.Amount}");
            totalAmount += enrollment.Amount;
        }
        tuitionBill.Specification = specification.ToString();
        tuitionBill.Amount = totalAmount;

        await SendTuitionBillEmail(student, tuitionBill);
        await _repo.RegisterTuitionBillAsync(tuitionBill);

        Log.Information("Tuition Bill {Id} sent to {StudentName}", tuitionBill.TuitionBillId, student.FullName);
    }
    
    private async Task SendTuitionBillEmail(Student student, TuitionBill tuitionBill)
    {
        var body = new StringBuilder();
        body.AppendLine($"Dear {student.FullName},");
        body.AppendLine($"<p>Please find your latest tuition bill below for recent course enrollments.</p>");
        body.AppendLine($"<p><b>Bill ID:</b> {tuitionBill.TuitionBillId}</p>");
        body.AppendLine($"<p><b>Date:</b> {tuitionBill.TuitionBillDate:yyyy-MM-dd}</p>");
        body.AppendLine("<hr>");
        body.AppendLine("<b>Specification:</b><br/>");
        body.Append(tuitionBill.Specification.Replace(Environment.NewLine, "<br/>"));
        body.AppendLine("<hr>");
        body.AppendLine($"<p><b>Total Amount Due: ${tuitionBill.Amount}</b></p>");
        body.AppendLine("<p>Thank you,<br/>The Student Management System</p>");

        var mailMessage = new MailMessage
        {
            From = new MailAddress("billing@studentsystem.com"),
            Subject = $"Tuition Bill #{tuitionBill.TuitionBillId}",
            Body = body.ToString(),
            IsBodyHtml = true
        };
        mailMessage.To.Add(student.EmailAddress);
        
        await _emailCommunicator.SendEmailAsync(mailMessage);
    }
}
namespace StudentSystem.NotificationService.Repositories;

public interface INotificationRepository
{
    Task RegisterStudentAsync(Student student);
    Task RegisterEnrollmentAsync(Enrollment enrollment);
    Task<IEnumerable<Enrollment>> GetEnrollmentsForTodayAsync(DateTime date);
    Task<Student> GetStudentAsync(string studentId);
    Task RemoveEnrollmentsAsync(IEnumerable<string> enrollmentIds);
}
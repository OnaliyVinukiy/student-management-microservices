namespace StudentSystem.TuitionService.Repositories;

public interface ITuitionBillRepository
{
    Task RegisterStudentAsync(Student student);
    Task<Student> GetStudentAsync(string studentId);
    Task RegisterEnrollmentAsync(Enrollment enrollment);
    Task<IEnumerable<Enrollment>> GetEnrollmentsToBillAsync();
    Task RegisterTuitionBillAsync(TuitionBill tuitionBill);
}
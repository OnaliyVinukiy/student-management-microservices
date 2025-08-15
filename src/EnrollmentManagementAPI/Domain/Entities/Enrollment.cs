namespace StudentSystem.EnrollmentManagementAPI.Domain.Entities;

public class Enrollment : Entity<Guid>
{
    public Course Course { get; private set; }
    public Student Student { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public string Grade { get; private set; }
    public string Status { get; private set; }

    public Enrollment(Guid id) : base(id) {}

    public void Enroll(Course course, Student student, DateTime enrollmentDate)
    {
        Course = course;
        Student = student;
        EnrollmentDate = enrollmentDate;
        Status = "Enrolled";
        Grade = "In Progress";
    }

    public void Complete(string grade)
    {
        Grade = grade;
        Status = "Completed";
    }
}
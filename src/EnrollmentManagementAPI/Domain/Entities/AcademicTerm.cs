namespace StudentSystem.EnrollmentManagementAPI.Domain.Entities;

public class AcademicTerm : AggregateRoot<AcademicTermId>
{
    public List<Enrollment> Enrollments { get; private set; }

    public AcademicTerm(DateOnly date) : base(AcademicTermId.Create(date)) { }
    public AcademicTerm(DateOnly date, IEnumerable<Event> events) : base(AcademicTermId.Create(date), events) { }

    public static AcademicTerm Create(DateOnly date)
    {
        var term = new AcademicTerm(date);
        var e = new AcademicTermCreated(Guid.NewGuid(), date);
        term.RaiseEvent(e);
        return term;
    }

    public void EnrollStudent(EnrollStudentInCourse command, IStudentRepository studentRepo, ICourseRepository courseRepo)
    {
        // Rule: Student must not already be enrolled in this course this term.
        this.StudentMustNotBeAlreadyEnrolled(command);

        // Handle event
        var e = new StudentEnrolled(Guid.NewGuid(), command.EnrollmentId, command.EnrollmentDate, command.StudentId, command.CourseCode);
        RaiseEvent(e);
    }

    public void CompleteCourse(CompleteCourseWithGrade command)
    {
        // Rule: Enrollment must exist
        var enrollment = Enrollments.FirstOrDefault(en => en.Id == command.EnrollmentId);
        if (enrollment == null)
        {
            throw new EnrollmentNotFoundException($"Enrollment with Id {command.EnrollmentId} not found in this term.");
        }
        
        // Rule: Cannot complete an already completed course
        enrollment.CannotCompleteACompletedEnrollment();

        // Handle event
        var e = new CourseCompleted(Guid.NewGuid(), command.EnrollmentId, command.Grade);
        RaiseEvent(e);
    }

    protected override void When(dynamic @event)
    {
        Handle(@event);
    }

    private void Handle(AcademicTermCreated e)
    {
        Enrollments = new List<Enrollment>();
    }

    private void Handle(StudentEnrolled e)
    {
        var enrollment = new Enrollment(e.EnrollmentId);
        var student = new Student(e.StudentId, "Unknown", "Unknown"); // Temp data, real data is in read model
        var course = new Course(e.CourseCode, "Unknown", "Unknown"); // Temp data
        enrollment.Enroll(course, student, e.EnrollmentDate);
        Enrollments.Add(enrollment);
    }

    private void Handle(CourseCompleted e)
    {
        var enrollment = Enrollments.FirstOrDefault(j => j.Id == e.EnrollmentId);
        enrollment?.Complete(e.Grade);
    }
}
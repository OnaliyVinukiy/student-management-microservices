namespace StudentSystem.EnrollmentEventHandler.DataAccess;

public class EnrollmentDBContext  : DbContext
{
    public EnrollmentDBContext ()
    { }

    public EnrollmentDBContext (DbContextOptions<EnrollmentDBContext > options) : base(options)
    { }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Course>().HasKey(entity => entity.CourseCode);
        builder.Entity<Course>().ToTable("Course");

        builder.Entity<Student>().HasKey(entity => entity.StudentId);
        builder.Entity<Student>().ToTable("Student");

        builder.Entity<Enrollment>().HasKey(entity => entity.EnrollmentId);
        builder.Entity<Enrollment>().ToTable("Enrollment");

        base.OnModelCreating(builder);
    }
}
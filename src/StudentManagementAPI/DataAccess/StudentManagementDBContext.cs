namespace StudentSystem.StudentManagementAPI.DataAccess;

public class StudentManagementDBContext : DbContext
{
    public StudentManagementDBContext(DbContextOptions<StudentManagementDBContext> options) : base(options)
    {

    }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Student>().HasKey(m => m.StudentId);
        builder.Entity<Student>().ToTable("Student");
        base.OnModelCreating(builder);
    }

    public void MigrateDB()
    {
        Policy
            .Handle<Exception>()
            .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
            .Execute(() => Database.Migrate());
    }
}
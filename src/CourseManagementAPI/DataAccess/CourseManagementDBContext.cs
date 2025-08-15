namespace StudentSystem.CourseManagement.DataAccess;

public class CourseManagementDBContext : DbContext
{
    public CourseManagementDBContext(DbContextOptions<CourseManagementDBContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Course>().HasKey(m => m.CourseCode);
        builder.Entity<Course>().ToTable("Course");
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
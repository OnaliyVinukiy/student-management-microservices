namespace StudentSystem.WebApp.Models;
public class AcademicTerm {
    public string Id { get; set; }
    public List<Enrollment> Enrollments { get; set; }
}
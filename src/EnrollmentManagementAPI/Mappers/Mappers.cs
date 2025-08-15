namespace StudentSystem.EnrollmentManagementAPI.Mappers;

public static class Mappers
{
    public static AcademicTermDTO MapToDTO(this AcademicTerm term) => new()
    {
        Id = term.Id,
        Enrollments = term.Enrollments.Select(e => e.MapToDTO()).ToList()
    };

    public static EnrollmentDTO MapToDTO(this Enrollment enrollment) => new()
    {
        Id = enrollment.Id,
        EnrollmentDate = enrollment.EnrollmentDate,
        Status = enrollment.Status,
        Grade = enrollment.Grade,
        Student = enrollment.Student.MapToDTO(),
        Course = enrollment.Course.MapToDTO()
    };

    public static StudentDTO MapToDTO(this Student student) => new()
    {
        StudentId = student.Id,
        FullName = student.FullName,
        EmailAddress = student.EmailAddress
    };

    public static CourseDTO MapToDTO(this Course course) => new()
    {
        CourseCode = course.Id,
        CourseName = course.CourseName,
        Department = course.Department
    };
}
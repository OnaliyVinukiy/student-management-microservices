namespace StudentSystem.WebApp.Mappers;

public static class Mappers
{
    public static RegisterStudent MapToRegisterStudent(this StudentManagementNewViewModel source) => new RegisterStudent
    (
        Guid.NewGuid(),
        Guid.NewGuid().ToString("N"),
        source.Student.Name,
        source.Student.Address,
        source.Student.PostalCode,
        source.Student.City,
        source.Student.TelephoneNumber,
        source.Student.EmailAddress
    );

    public static RegisterCourse MapToRegisterCourse(this CourseManagementNewViewModel source) => new RegisterCourse(
        Guid.NewGuid(),
        source.Course.CourseCode,
        source.Course.Brand,
        source.Course.Type,
        source.SelectedStudentId
    );
}
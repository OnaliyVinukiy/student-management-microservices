namespace StudentSystem.StudentManagementAPI.Mappers;

public static class Mappers
{
    public static StudentRegistered MapToStudentRegistered(this RegisterStudent command) => new(
        Guid.NewGuid(),
        command.StudentId,
        command.FullName,
        command.DateOfBirth,
        command.EmailAddress
    );

    public static Student MapToStudent(this RegisterStudent command) => new()
    {
        StudentId = command.StudentId,
        FullName = command.FullName,
        DateOfBirth = command.DateOfBirth,
        EmailAddress = command.EmailAddress
    };
}
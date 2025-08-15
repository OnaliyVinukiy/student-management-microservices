namespace StudentSystem.StudentManagementAPI.Model;

public class Student
{
    public string StudentId { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}
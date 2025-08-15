namespace StudentSystem.WebApp.RESTClients;
using StudentSystem.WebApp.Models;
public interface IStudentManagementAPI {
    Task<List<Student>> GetStudents();
    Task<Student> GetStudentById(string studentId);
    Task RegisterStudent(Student student);
}
using StudentSystem.WebApp.Models;

namespace StudentSystem.WebApp.RESTClients;

public interface IEnrollmentAPI
{
    Task<AcademicTerm> GetAcademicTerm(string termDate);
    Task RegisterEnrollment(string termDate, Enrollment enrollment);
    Task CompleteEnrollment(string termDate, Guid enrollmentId, string grade);
}
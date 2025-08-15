namespace StudentSystem.EnrollmentManagementAPI.Repositories;

public interface IEventSourceRepository<T> where T : AggregateRoot<AcademicTermId>
{
    Task<T> GetByIdAsync(string id);
    Task SaveAsync(string id, int originalVersion, int newVersion, IEnumerable<Event> newEvents);
}
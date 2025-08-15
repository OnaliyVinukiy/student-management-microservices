namespace StudentSystem.EnrollmentManagementAPI.CommandHandlers;

public class EnrollStudentInCourseCommandHandler : IEnrollStudentInCourseCommandHandler
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IEventSourceRepository<AcademicTerm> _termRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly ICourseRepository _courseRepo;

    public EnrollStudentInCourseCommandHandler(IMessagePublisher messagePublisher, IEventSourceRepository<AcademicTerm> termRepo, IStudentRepository studentRepo, ICourseRepository courseRepo)
    {
        _messagePublisher = messagePublisher;
        _termRepo = termRepo;
        _studentRepo = studentRepo;
        _courseRepo = courseRepo;
    }

    public async Task<AcademicTerm> HandleCommandAsync(DateOnly termDate, EnrollStudentInCourse command)
    {
        var termId = AcademicTermId.Create(termDate);
        var term = await _termRepo.GetByIdAsync(termId);
        if (term == null)
        {
            term = AcademicTerm.Create(termDate);
        }

        term.EnrollStudent(command, _studentRepo, _courseRepo);

        var events = term.GetEvents();
        await _termRepo.SaveAsync(term.Id, term.OriginalVersion, term.Version, events);

        foreach (var e in events)
        {
            await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
        }
        
        return term;
    }
}
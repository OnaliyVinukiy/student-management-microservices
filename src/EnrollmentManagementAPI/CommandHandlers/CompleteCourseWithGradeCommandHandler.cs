namespace StudentSystem.EnrollmentManagementAPI.CommandHandlers;

public class CompleteCourseWithGradeCommandHandler : ICompleteCourseWithGradeCommandHandler
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IEventSourceRepository<AcademicTerm> _termRepo;

    public CompleteCourseWithGradeCommandHandler(IMessagePublisher messagePublisher, IEventSourceRepository<AcademicTerm> termRepo)
    {
        _messagePublisher = messagePublisher;
        _termRepo = termRepo;
    }

    public async Task<AcademicTerm> HandleCommandAsync(DateOnly termDate, CompleteCourseWithGrade command)
    {
        var termId = AcademicTermId.Create(termDate);
        var term = await _termRepo.GetByIdAsync(termId);
        if (term == null)
        {
            throw new EnrollmentNotFoundException($"Academic Term for {termDate} not found.");
        }

        term.CompleteCourse(command);

        var events = term.GetEvents();
        await _termRepo.SaveAsync(term.Id, term.OriginalVersion, term.Version, events);

        foreach (var e in events)
        {
            await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");
        }

        return term;
    }
}
namespace StudentSystem.EnrollmentManagementAPI.Events;

public class AcademicTermCreated : Event
{
    public readonly DateOnly TermDate;

    public AcademicTermCreated(Guid messageId, DateOnly termDate) : base(messageId)
    {
        TermDate = termDate;
    }
}
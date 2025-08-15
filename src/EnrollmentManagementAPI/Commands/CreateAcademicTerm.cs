namespace StudentSystem.EnrollmentManagementAPI.Commands;

public class CreateAcademicTerm : Command
{
    public readonly DateOnly TermDate;

    public CreateAcademicTerm(Guid messageId, DateOnly termDate) : base(messageId)
    {
        TermDate = termDate;
    }
}
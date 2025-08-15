namespace StudentSystem.EnrollmentManagementAPI.Domain.ValueObjects;

public class AcademicTermId : ValueObject
{
    public string Value { get; private set; }

    public static AcademicTermId Create(DateOnly date)
    {
        // An academic term can be identified by its year and a term name (e.g., Fall, Spring)
        string term = date.Month >= 8 ? "FALL" : "SPRING";
        return new AcademicTermId { Value = $"{date.Year}-{term}" };
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator string(AcademicTermId id) => id.Value;
}
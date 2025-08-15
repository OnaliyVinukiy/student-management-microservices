namespace StudentSystem.EnrollmentManagementAPI.Domain.BusinessRules;

public static class EnrollmentRules
{
    public static void CannotCompleteACompletedEnrollment(this Enrollment enrollment)
    {
        if (enrollment.Status == "Completed")
        {
            throw new BusinessRuleViolationException("An already completed enrollment cannot be modified.");
        }
    }
}
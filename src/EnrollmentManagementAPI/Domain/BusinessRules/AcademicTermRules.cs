namespace StudentSystem.EnrollmentManagementAPI.Domain.BusinessRules;

public static class AcademicTermRules
{
    public static void StudentMustNotBeAlreadyEnrolled(this AcademicTerm term, EnrollStudentInCourse command)
    {
        if (term.Enrollments.Any(e => e.Student.Id == command.StudentId && e.Course.Id == command.CourseCode))
        {
            throw new BusinessRuleViolationException($"Student {command.StudentId} is already enrolled in course {command.CourseCode} for this term.");
        }
    }
}
using StudentSystem.EnrollmentManagementAPI.DTOs; 
namespace StudentSystem.EnrollmentManagementAPI.Controllers;

[Route("/api/terms")]
public class AcademicTermController : Controller
{
    private readonly IEventSourceRepository<AcademicTerm> _termRepo;
    private readonly IEnrollStudentInCourseCommandHandler _enrollHandler;
    private readonly ICompleteCourseWithGradeCommandHandler _completeHandler;

    public AcademicTermController(
        IEventSourceRepository<AcademicTerm> termRepo,
        IEnrollStudentInCourseCommandHandler enrollHandler,
        ICompleteCourseWithGradeCommandHandler completeHandler)
    {
        _termRepo = termRepo;
        _enrollHandler = enrollHandler;
        _completeHandler = completeHandler;
    }

    [HttpGet("{termDate}")]
    public async Task<IActionResult> GetTermByDate(string termDate)
    {
        if (!DateOnly.TryParse(termDate, out var date))
            return BadRequest("Invalid date format. Please use YYYY-MM-DD.");

        var termId = AcademicTermId.Create(date);
        var term = await _termRepo.GetByIdAsync(termId);
        return term == null ? NotFound() : Ok(term.MapToDTO());
    }

    [HttpPost("{termDate}/enrollments")]
    public async Task<IActionResult> EnrollStudentAsync(string termDate, [FromBody] EnrollStudentInCourse command)
    {
        try
        {
            if (!DateOnly.TryParse(termDate, out var date))
                return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
            
            var term = await _enrollHandler.HandleCommandAsync(date, command);
            var enrollment = term.Enrollments.FirstOrDefault(e => e.Id == command.EnrollmentId);
            return CreatedAtAction(nameof(GetTermByDate), new { termDate }, enrollment.MapToDTO());
        }
        catch (BusinessRuleViolationException ex)
        {
            return Conflict(new BusinessRuleViolation { ErrorMessage = ex.Message });
        }
        catch (ConcurrencyException)
        {
            return Conflict("Concurrency error. Please try again.");
        }
    }
    
    [HttpPut("{termDate}/enrollments/{enrollmentId}/complete")]
    public async Task<IActionResult> CompleteCourseAsync(string termDate, Guid enrollmentId, [FromBody] CompleteCourseWithGrade command)
    {
        try
        {
            if (!DateOnly.TryParse(termDate, out var date))
                return BadRequest("Invalid date format. Please use YYYY-MM-DD.");

            await _completeHandler.HandleCommandAsync(date, command);
            return Ok();
        }
        catch (BusinessRuleViolationException ex)
        {
            return Conflict(new BusinessRuleViolation { ErrorMessage = ex.Message });
        }
        catch (EnrollmentNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ConcurrencyException)
        {
            return Conflict("Concurrency error. Please try again.");
        }
    }
}
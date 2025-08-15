namespace StudentSystem.StudentManagementAPI.Controllers;

[Route("/api/[controller]")]
public class StudentsController : Controller
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly StudentManagementDBContext _dbContext;

    public StudentsController(StudentManagementDBContext dbContext, IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Students.ToListAsync());
    }

    [HttpGet]
    [Route("{studentId}", Name = "GetByStudentId")]
    public async Task<IActionResult> GetByStudentId(string studentId)
    {
        var student = await _dbContext.Students.FirstOrDefaultAsync(c => c.StudentId == studentId);
        if (student == null)
        {
            return NotFound();
        }
        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterStudent command)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // insert student
                Student student = command.MapToStudent();
                _dbContext.Students.Add(student);
                await _dbContext.SaveChangesAsync();

                // send event
                StudentRegistered e = command.MapToStudentRegistered();
                await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

                // return result
                return CreatedAtRoute("GetByStudentId", new { studentId = student.StudentId }, student);
            }
            return BadRequest();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. " +
                "The Student ID might already exist.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
namespace StudentSystem.CourseManagement.Controllers;

[Route("/api/[controller]")]
public class CoursesController : Controller
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly CourseManagementDBContext _dbContext;

    public CoursesController(CourseManagementDBContext dbContext, IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _dbContext.Courses.ToListAsync());
    }

    [HttpGet]
    [Route("{courseCode}", Name = "GetByCourseCode")]
    public async Task<IActionResult> GetByCourseCode(string courseCode)
    {
        var course = await _dbContext.Courses.FirstOrDefaultAsync(v => v.CourseCode == courseCode.ToUpper());
        if (course == null)
        {
            return NotFound();
        }
        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCourse command)
    {
        try
        {
            if (ModelState.IsValid)
            {
                
                if (string.IsNullOrWhiteSpace(command.CourseCode) || string.IsNullOrWhiteSpace(command.CourseName))
                {
                    return BadRequest("Course Code and Course Name are required.");
                }

                
                Course course = command.MapToCourse();
                
                
                course.CourseCode = course.CourseCode.ToUpper();

                _dbContext.Courses.Add(course);
                await _dbContext.SaveChangesAsync();

   
                var e = CourseCreated.FromCommand(command);
                await _messagePublisher.PublishMessageAsync(e.MessageType, e, "");

               
                return CreatedAtRoute("GetByCourseCode", new { courseCode = course.CourseCode }, course);
            }
            return BadRequest();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Unable to save changes. The Course Code might already exist.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
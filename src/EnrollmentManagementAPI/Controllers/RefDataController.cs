namespace StudentSystem.EnrollmentManagementAPI.Controllers;

[Route("/api/[controller]")]
public class RefDataController : Controller
{
    IStudentRepository _studentRepo;
    ICourseRepository _courseRepo;

    public RefDataController(IStudentRepository studentRepo, ICourseRepository courseRepo)
    {
        _studentRepo = studentRepo;
        _courseRepo = courseRepo;
    }

    [HttpGet]
    [Route("students")]
    public async Task<IActionResult> GetStudents()
    {
        return Ok(await _studentRepo.GetStudentsAsync());
    }

    [HttpGet]
    [Route("students/{studentId}")]
    public async Task<IActionResult> GetStudentByStudentId(string studentId)
    {
        var student = await _studentRepo.GetStudentAsync(studentId);
        if (student == null)
        {
            return NotFound();
        }
        return Ok(student);
    }

    [HttpGet]
    [Route("courses")]
    public async Task<IActionResult> GetCourses()
    {
        return Ok(await _courseRepo.GetCoursesAsync());
    }

    [HttpGet]
    [Route("courses/{courseCode}")]
    public async Task<IActionResult> GetCourseByCourseCode(string courseCode)
    {
        var course = await _courseRepo.GetCourseAsync(courseCode);
        if (course == null)
        {
            return NotFound();
        }
        return Ok(course);
    }
}
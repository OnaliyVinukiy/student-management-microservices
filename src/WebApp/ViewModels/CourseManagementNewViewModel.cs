using System.ComponentModel.DataAnnotations;

namespace StudentSystem.WebApp.ViewModels;

public class CourseManagementNewViewModel
{
    [Required]
    [Display(Name = "Course Code")]
    public string CourseCode { get; set; }

    [Required]
    [Display(Name = "Course Name")]
    public string CourseName { get; set; }

    [Required]
    public string Department { get; set; }

    [Required]
    [Range(1, 10)]
    [Display(Name = "Credit Hours")]
    public int? CreditHours { get; set; }
}
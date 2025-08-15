using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentSystem.WebApp.ViewModels;

public class EnrollmentNewViewModel
{
    [Required]
    [Display(Name = "Select Student")]
    public string SelectedStudentId { get; set; }

    [Required]
    [Display(Name = "Select Course")]
    public string SelectedCourseCode { get; set; }

    public SelectList Students { get; set; }
    public SelectList Courses { get; set; }
}
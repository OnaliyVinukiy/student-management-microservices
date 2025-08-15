using System.ComponentModel.DataAnnotations;

namespace StudentSystem.WebApp.ViewModels;

public class StudentManagementNewViewModel
{
    [Required]
    [Display(Name = "Student ID")]
    public string StudentId { get; set; }
    
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }
    
    [Required]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string EmailAddress { get; set; }
}
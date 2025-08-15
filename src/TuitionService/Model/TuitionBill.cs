namespace StudentSystem.TuitionService.Model;

public class TuitionBill
{
    public string TuitionBillId { get; set; }
    public DateTime TuitionBillDate { get; set; }
    public string StudentId { get; set; }
    public decimal Amount { get; set; }
    public string Specification { get; set; }
    public string EnrollmentIds { get; set; } 
}
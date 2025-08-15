namespace StudentSystem.TuitionBillService.CommunicationChannels;

public interface IEmailCommunicator
{
    Task SendEmailAsync(MailMessage message);
}
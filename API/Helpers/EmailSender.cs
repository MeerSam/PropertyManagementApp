using Microsoft.AspNetCore.Identity.UI.Services;

namespace API.Helpers;
public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    { 
        Console.WriteLine("EMAIL SENT:");
        Console.WriteLine(email);
        Console.WriteLine(subject);
        Console.WriteLine(htmlMessage);

        return Task.CompletedTask;
    }
}

using SendGrid;
using SendGrid.Helpers.Mail;

namespace INFT3500;

public class SendGridEmailSenderSLSLACDK 
{
    private readonly IConfiguration configuration;

    public SendGridEmailSender()
    {
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        string sendGridApiKey = "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";
        if (string.IsNullOrEmpty(sendGridApiKey))
        {
            throw new Exception("The 'SendGridApiKey' is not configured");
        }
        
        var client = new SendGridClient(sendGridApiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("c3331609@uon.edu.au", "INFT3500"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        var response = await client.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Email queued successfully");
        }
        else
        {
            Console.WriteLine("Failed to send email");
            // Adding more information related to the failed email could be helpful in debugging failure,
            // but be careful about logging PII, as it increases the chance of leaking PII
        }
    }
}
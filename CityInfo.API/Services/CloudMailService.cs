namespace CityInfo.API.Services;

public class CloudMailService : IMailService
{
    private string _mailTo = "admin@mycompany.com";
    private string _mailFrom = "no-reply@mycompany.com";

    public void Send(string subject, string message)
    {
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo} with {nameof(CloudMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}
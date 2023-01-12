namespace CityInfo.API.Services;

public class LocalMailService : IMailService
{
    private string _mailTo = "admin@mycompany.com";
    private string _mailFrom = "no-reply@mycompany.com";

    public void Send(string subject, string message)
    {
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo} with {nameof(LocalMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}
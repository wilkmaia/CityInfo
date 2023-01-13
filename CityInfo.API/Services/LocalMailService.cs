namespace CityInfo.API.Services;

public class LocalMailService : IMailService
{
    private readonly string _mailTo;
    private readonly string _mailFrom;

    public LocalMailService(IConfiguration configuration)
    {
        _mailFrom = configuration["Mail:FromAddress"] ??
                    throw new ArgumentNullException("configuration.Mail.FromAddress");
        _mailTo = configuration["Mail:ToAddress"] ??
                    throw new ArgumentNullException("configuration.Mail.ToAddress");
    }

    public void Send(string subject, string message)
    {
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo} with {nameof(LocalMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}
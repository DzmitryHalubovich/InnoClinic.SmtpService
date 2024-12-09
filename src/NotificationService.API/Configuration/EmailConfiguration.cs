namespace NotificationService.API.Configuration;

public class EmailConfiguration
{
    public string From { get; set; }

    public string SmtpServer { get; set; }

    public int Port { get; set; }
}

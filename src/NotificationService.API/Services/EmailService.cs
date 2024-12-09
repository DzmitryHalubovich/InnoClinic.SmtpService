using InnoClinic.SharedModels.MQMessages.Appointments;
using InnoClinic.SharedModels.MQMessages.IdentityServer;
using InnoClinic.SharedModels.MQMessages.Profiles;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using NotificationService.API.Configuration;
using NotificationService.API.DocumentHttpClient;

namespace NotificationService.API.Services;

public class EmailService : IEmailService
{
    private readonly DocumentsServiceHttpClient _documentsHttpClient;
    private readonly EmailConfiguration _emailConfiguration;
    private readonly IConfiguration _configuration;

    public EmailService(EmailConfiguration emailConfiguration, 
        IConfiguration configuration, 
        DocumentsServiceHttpClient documentsHttpClient)
    {
        _configuration = configuration;
        _emailConfiguration = emailConfiguration;
        _documentsHttpClient = documentsHttpClient;
    }


    public async Task SendDoctorProfileCreated(WorkerProfileRegisteredMessage message)
    {
        var emailMessage = CreateDoctorProfileCreatedMessage(message);

        await Send(emailMessage);
    }

    public async Task SendAppointmentApprovedNotification(AppointmentApprovedMessage message)
    {
        var emailMessage = CreateAppointmentApprovedEmailMessage(message);

        await Send(emailMessage);
    }

    public async Task SendAppointmentRemindNotification(AppointmentRemindNotificationMessage message)
    {
        var emailMessage = CreateNotificationEmailMessage(message);

        await Send(emailMessage);
    }

    public async Task SendAppointmentResultCreated(AppointmentResultCreatedMessage message)
    {
        var file = await _documentsHttpClient.DownloadAppointmentResultFileAsync(message.AppointmentResultId);

        var emailMessage = CreateMessageWithCreatedResultFile(message, file);

        await Send(emailMessage);
    }

    public async Task SendAppointmentResultUpdated(AppointmentResultUpdatedMessage message)
    {
        var file = await _documentsHttpClient.DownloadAppointmentResultFileAsync(message.AppointmentResultId);

        var emailMessage = CreateMessageWithUpdatedResultFile(message, file);

        await Send(emailMessage);
    }

    public async Task SendEmailConfirmation(EmailConfirmationMessage message)
    {
        var emailMessage = CreateEmailConfirmMessage(message);

        await Send(emailMessage);
    }

    private MimeMessage CreateEmailConfirmMessage(EmailConfirmationMessage message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("InnoClinic Administration", _emailConfiguration.From));
        emailMessage.To.Add(new MailboxAddress(message.Email, message.Email));
        emailMessage.Subject = "Confirmation email link";
        emailMessage.Body = new TextPart(TextFormat.Plain)
        {
            Text = message.ConfirmationLink
        };

        return emailMessage;
    }

    private MimeMessage CreateDoctorProfileCreatedMessage(WorkerProfileRegisteredMessage message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("InnoClinic Administration", _emailConfiguration.From));
        emailMessage.To.Add(new MailboxAddress(message.Email, message.Email));
        emailMessage.Subject = "Your doctor profile was created";
        emailMessage.Body = new TextPart(TextFormat.Plain)
        {
            Text = $"Hello, your working profile was created. Your credentials: email: \"{message.Email}\", password: \"{message.Password}\""
        };

        return emailMessage;
    }

    private MimeMessage CreateAppointmentApprovedEmailMessage(AppointmentApprovedMessage message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("InnoClinic Administration", _emailConfiguration.From));
        emailMessage.To.Add(new MailboxAddress(message.PatientEmail, message.PatientEmail));
        emailMessage.Subject = "Appointment was approved";
        emailMessage.Body = new TextPart(TextFormat.Plain) 
        { 
            Text = $"Hi there, your appointment is scheduled for {message.AppointmentDate.ToShortDateString()} at {message.AppointmentDate.ToShortTimeString()}" 
        };

        return emailMessage;
    }

    private MimeMessage CreateMessageWithCreatedResultFile(AppointmentResultCreatedMessage message, Stream file)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("InnoClinic Administration", _emailConfiguration.From));
        emailMessage.To.Add(new MailboxAddress(message.PatientEmail, message.PatientEmail));
        emailMessage.Subject = $"Dear {message.PatientFullName}, We are pleased to inform you that the results of your recent appointment are now available. " +
            $"Please find the results attached to this email.";

        var body = new TextPart("plain")
        {
            Text = "Your appointment result"
        };

        var attachment = new MimePart("application", "pdf")
        {
            Content = new MimeContent(file),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = $"{message.PatientFullName}.pdf"
        };

        var multipart = new Multipart("mixed")
        {
            body,
            attachment
        };

        emailMessage.Body = multipart;

        return emailMessage;
    }

    private MimeMessage CreateMessageWithUpdatedResultFile(AppointmentResultUpdatedMessage message, Stream file)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("InnoClinic Administration", _emailConfiguration.From));
        emailMessage.To.Add(new MailboxAddress(message.PatientEmail, message.PatientEmail));
        emailMessage.Subject = "Appointment result";

        var body = new TextPart("plain")
        {
            Text = $"Dear {message.PatientFullName}. We would like to inform you that the results of your recent appointment have been updated. " +
                $"Please find the updated results attached to this email."
        };

        var attachment = new MimePart("application", "pdf")
        {
            Content = new MimeContent(file),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = $"{message.PatientFullName}.pdf"
        };

        var multipart = new Multipart("mixed")
        {
            body,
            attachment
        };

        emailMessage.Body = multipart;

        return emailMessage;
    }

    private MimeMessage CreateNotificationEmailMessage(AppointmentRemindNotificationMessage message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("InnoClinic Administration", _emailConfiguration.From));
        emailMessage.To.Add(new MailboxAddress(message.PatientEmail, message.PatientEmail));
        emailMessage.Subject = "Appointment notification";
        emailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text =$@"
                        <html>
                        <body>
                            <p>Dear {message.PatientFullName},</p>
                            <p>This is a friendly reminder that you have an appointment scheduled for tomorrow.</p>
                            <p><strong>Appointment Details:</strong></p>
                            <ul>
                                <li><strong>Date:</strong> {message.Date}</li>
                                <li><strong>Time:</strong> {message.Time}</li>
                                <li><strong>Service:</strong> {message.ServiceName}</li>
                                <li><strong>Doctor:</strong> {message.DoctorFullName}</li>
                            </ul>
                            <p>If you have any questions or need to reschedule, please contact our office.</p>
                            <p>We look forward to seeing you!</p>
                            <p>Best regards,</p>
                            <p>Your InnoClinic Team</p>
                        </body>
                        </html>"
        };

        return emailMessage;
    }

    private async Task Send(MimeMessage message)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, false);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                 var userName = _configuration["AccountUserName"];
                 var password = _configuration["AccountPassword"];

                await client.AuthenticateAsync(userName, password);

                await client.SendAsync(message);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}

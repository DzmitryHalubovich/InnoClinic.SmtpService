using InnoClinic.SharedModels.MQMessages.IdentityServer;
using MassTransit;
using NotificationService.API.Services;

namespace NotificationService.API.MassTransit;

public class RegisterEmailConfirmationConsumer : IConsumer<EmailConfirmationMessage>
{
    private readonly IEmailService _emailService;

    public RegisterEmailConfirmationConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<EmailConfirmationMessage> context)
    {
        await _emailService.SendEmailConfirmation(new EmailConfirmationMessage()
        {
            Email = context.Message.Email,
            ConfirmationLink = context.Message.ConfirmationLink
        });
    }
}

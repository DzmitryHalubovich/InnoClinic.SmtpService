using InnoClinic.SharedModels.MQMessages.Appointments;
using MassTransit;
using Microsoft.AspNetCore.Identity.UI.Services;
using NotificationService.API.Services;

namespace NotificationService.API.MassTransit;

public class AppointmentNotificationConsumer : IConsumer<AppointmentRemindNotificationMessage>
{
    private readonly IEmailService _emailService;

    public AppointmentNotificationConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<AppointmentRemindNotificationMessage> context)
    {
        await _emailService.SendAppointmentRemindNotification(context.Message);
    }
}

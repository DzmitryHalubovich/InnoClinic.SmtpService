using InnoClinic.SharedModels.MQMessages.Appointments;
using MassTransit;
using NotificationService.API.Services;

namespace NotificationService.API.MassTransit;

public class AppointmentApprovedConsumer : IConsumer<AppointmentApprovedMessage>
{
    private readonly IEmailService _emailService;

    public AppointmentApprovedConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<AppointmentApprovedMessage> context)
    {
        await _emailService.SendAppointmentApprovedNotification(context.Message);
    }
}

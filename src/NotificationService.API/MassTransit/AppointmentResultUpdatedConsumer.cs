using InnoClinic.SharedModels.MQMessages.Appointments;
using MassTransit;
using NotificationService.API.Services;

namespace NotificationService.API.MassTransit;

public class AppointmentResultUpdatedConsumer : IConsumer<AppointmentResultUpdatedMessage>
{
    private readonly IEmailService _emailService;

    public AppointmentResultUpdatedConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<AppointmentResultUpdatedMessage> context)
    {
        await _emailService.SendAppointmentResultUpdated(context.Message);
    }
}

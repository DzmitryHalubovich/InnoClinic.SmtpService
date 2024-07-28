using InnoClinic.SharedModels.MQMessages.Appointments;
using MassTransit;
using NotificationService.API.Services;

namespace NotificationService.API.MassTransit;

public class AppointmentResultCreatedConsumer : IConsumer<AppointmentResultCreatedMessage>
{
    private readonly IEmailService _emailService;

    public AppointmentResultCreatedConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<AppointmentResultCreatedMessage> context)
    {
        await _emailService.SendAppointmentResultCreated(context.Message);
    }
}

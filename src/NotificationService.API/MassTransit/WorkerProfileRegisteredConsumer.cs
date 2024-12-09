using InnoClinic.SharedModels.MQMessages.IdentityServer;
using MassTransit;
using NotificationService.API.Services;

namespace NotificationService.API.MassTransit
{
    public class WorkerProfileRegisteredConsumer : IConsumer<WorkerProfileRegisteredMessage>
    {
        private readonly IEmailService _emailService;

        public WorkerProfileRegisteredConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<WorkerProfileRegisteredMessage> context)
        {
            await _emailService.SendDoctorProfileCreated(context.Message);
        }
    }
}

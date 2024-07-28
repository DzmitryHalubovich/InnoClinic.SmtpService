using NotificationService.API.DocumentHttpClient;
using NotificationService.API.Services;
using MassTransit;
using NotificationService.API.MassTransit;

namespace NotificationService.API.Extentions;

public static class WebApplicationBuilderExtention
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEmailService, EmailService>();

        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();

            var assembly = typeof(Program).Assembly;

            x.AddConsumer<AppointmentApprovedConsumer>();
            x.AddConsumer<AppointmentNotificationConsumer>();
            x.AddConsumer<AppointmentResultCreatedConsumer>();
            x.AddConsumer<AppointmentResultUpdatedConsumer>();
            x.AddConsumer<RegisterEmailConfirmationConsumer>();

            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddActivities(assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("appointment-approved-queue", queueConfigurator =>
                {
                    queueConfigurator.Consumer<AppointmentApprovedConsumer>(context);
                });

                cfg.ReceiveEndpoint("appointment-notificated-queue", queueConfigurator =>
                {
                    queueConfigurator.Consumer<AppointmentNotificationConsumer>(context);
                });

                cfg.ReceiveEndpoint("appointmentResult-created-queue", queueConfigurator =>
                {
                    queueConfigurator.Consumer<AppointmentResultCreatedConsumer>(context);
                });

                cfg.ReceiveEndpoint("appointmentResult-updated-queue", queueConfiguration =>
                {
                    queueConfiguration.Consumer<AppointmentResultUpdatedConsumer>(context);
                });

                cfg.ReceiveEndpoint("email-confirm-queue", queueConfiguration =>
                {
                    queueConfiguration.Consumer<RegisterEmailConfirmationConsumer>(context);
                });
            });

        });

        builder.Services.AddHttpClient<DocumentsServiceHttpClient>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
    }
}
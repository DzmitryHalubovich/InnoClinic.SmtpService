﻿using NotificationService.API.DocumentHttpClient;
using NotificationService.API.Services;
using MassTransit;
using NotificationService.API.MassTransit;
using NotificationService.API.Configuration;

namespace NotificationService.API.Extentions;

public static class WebApplicationBuilderExtention
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var emailConfiguration = builder.Configuration.GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();

        builder.Services.AddSingleton(emailConfiguration);

        builder.Services.AddScoped<IEmailService, EmailService>();

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
            x.AddConsumer<WorkerProfileRegisteredConsumer>();

            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddActivities(assembly);

            var rabbitMqConfiguration = builder.Configuration.GetSection("RabbitMQ")
                .Get<RabbitMQConfiguration>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConfiguration.HostName, "/", h =>
                {
                    h.Username(rabbitMqConfiguration.UserName);
                    h.Password(rabbitMqConfiguration.Password);
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

                cfg.ReceiveEndpoint("worker-user-created-send-credentials", queueConfiguration =>
                {
                    queueConfiguration.Consumer<WorkerProfileRegisteredConsumer>(context);
                });
            });

        });

        builder.Services.AddHttpClient<DocumentsServiceHttpClient>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
    }
}
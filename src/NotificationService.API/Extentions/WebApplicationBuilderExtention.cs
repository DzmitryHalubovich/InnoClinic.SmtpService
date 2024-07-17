using NotificationService.API.Configuration;
using NotificationService.API.DocumentHttpClient;
using NotificationService.API.Services;
using NotificationService.API.RabbitMQ;
using NotificationService.API.RabbitMQ.QueuesBindingParameters;

namespace NotificationService.API.Extentions;

public static class WebApplicationBuilderExtention
{
    private static RabbitListener _listener {  get; set; }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var emailConfig = builder.Configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();

        var appointmentApprovedBindingParameters = builder.Configuration
            .GetSection("RabbitMqConsumerQueuesParameters:AppointmentApprovedEvent")
            .Get<AppointmentApprovedQueueBindingParameters>();
        
        var appointmentNotificationBindingParameters = builder.Configuration
            .GetSection("RabbitMqConsumerQueuesParameters:AppointmentNotificationEvent")
            .Get<AppointmentNotificationQueueBindingParameters>();

        var appointmentResultCreatedBindingParameters = builder.Configuration
            .GetSection("RabbitMqConsumerQueuesParameters:AppointmentResultCreatedEvent")
            .Get<AppointmentResultCreatedQueueBindingParameters>();

        var appointmentResultUpdatedBindingParameters = builder.Configuration
            .GetSection("RabbitMqConsumerQueuesParameters:AppointmentResultUpdatedEvent")
            .Get<AppointmentResultUpdatedQueueBindingParameters>();

        var emailConfirmationQueueBindingParameters = builder.Configuration
            .GetSection("RabbitMqConsumerQueuesParameters:EmailConfirmationEvent")
            .Get<EmailConfirmationQueueBindingParameters>();

        builder.Services.AddSingleton(appointmentApprovedBindingParameters);
        builder.Services.AddSingleton(appointmentNotificationBindingParameters);
        builder.Services.AddSingleton(appointmentResultCreatedBindingParameters);
        builder.Services.AddSingleton(appointmentResultUpdatedBindingParameters);
        builder.Services.AddSingleton(emailConfirmationQueueBindingParameters);

        builder.Services.AddSingleton(emailConfig);
        builder.Services.AddSingleton<RabbitListener>();

        builder.Services.AddTransient<IEmailService, EmailService>();

        builder.Services.AddHttpClient<DocumentsServiceHttpClient>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
    }

    public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
    {
        _listener = app.ApplicationServices.GetService<RabbitListener>();

        var lifeTime = app.ApplicationServices.GetService<Microsoft.AspNetCore.Hosting.IApplicationLifetime>();

        lifeTime.ApplicationStarted.Register(OnStarted);

        lifeTime.ApplicationStopping.Register(OnStopping);

        return app;
    }

    private static void OnStarted()
    {
        _listener.Register();
    }

    private static void OnStopping()
    {
        _listener.Deregister();
    }
}
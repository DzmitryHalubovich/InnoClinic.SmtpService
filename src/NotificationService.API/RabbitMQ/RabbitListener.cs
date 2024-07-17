using InnoClinic.SharedModels.MQMessages.Appointments;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.API.RabbitMQ.QueuesBindingParameters;
using NotificationService.API.Services;
using System.Text.Json;
using InnoClinic.SharedModels.MQMessages.IdentityServer;

namespace NotificationService.API.RabbitMQ;

public class RabbitListener
{
    private readonly AppointmentApprovedQueueBindingParameters _appointmentApprovedBindingParameters;
    private readonly AppointmentNotificationQueueBindingParameters _appointmentNotificationBindingParameters;
    private readonly AppointmentResultCreatedQueueBindingParameters _appointmentResultCreatedBindingParameters;
    private readonly AppointmentResultUpdatedQueueBindingParameters _appointmentResultUpdatedBindingParameters;
    private readonly EmailConfirmationQueueBindingParameters _emailConfirmationQueueBindingParameters;

    private readonly IEmailService _emailSender;
    private readonly IServiceScope _scope;
    private IConnection? _connection;
    private IModel _channel;

    public RabbitListener(IServiceProvider serviceProvider,
        AppointmentApprovedQueueBindingParameters appointmentApprovedBindingParameters,
        AppointmentNotificationQueueBindingParameters appointmentNotificationBindingParameters,
        AppointmentResultCreatedQueueBindingParameters appointmentResultCreatedBindingParameters,
        AppointmentResultUpdatedQueueBindingParameters appointmentResultUpdatedBindingParameters,
        EmailConfirmationQueueBindingParameters emailConfirmationQueueBindingParameters)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        _appointmentApprovedBindingParameters = appointmentApprovedBindingParameters;
        _appointmentNotificationBindingParameters = appointmentNotificationBindingParameters;
        _appointmentResultCreatedBindingParameters = appointmentResultCreatedBindingParameters;
        _appointmentResultUpdatedBindingParameters = appointmentResultUpdatedBindingParameters;

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _scope = serviceProvider.CreateScope();
        _emailSender = _scope.ServiceProvider.GetService<IEmailService>();
        _emailConfirmationQueueBindingParameters = emailConfirmationQueueBindingParameters;

    }

    public void Register()
    {
        RegisterAppointmentApprovedQueue();
        RegisterAppointmentNotificationQueue();
        RegisterAppointmentResultCreatedQueue();
        RegisterAppointmentResultUpdatedQueue();
        RegisterEmailConfirmationQueue();
    }

    public void Deregister()
    {
        _channel?.Close();
        _connection?.Close();
        _scope.Dispose();
    }


    private void RegisterEmailConfirmationQueue()
    {
        SetQueue(_emailConfirmationQueueBindingParameters);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var emailConfirmationInformation = JsonSerializer.Deserialize<EmailConfirmationMessage>(body);

            await _emailSender.SendEmailConfirmation(new EmailConfirmationMessage() 
            { 
                Email = emailConfirmationInformation.Email, 
                ConfirmationLink = emailConfirmationInformation.ConfirmationLink
            });
        };

        _channel.BasicConsume(queue: _emailConfirmationQueueBindingParameters.QueueName, autoAck: true, consumer: consumer);
    }

    private void RegisterAppointmentApprovedQueue()
    {
        SetQueue(_appointmentApprovedBindingParameters);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentApprovedMessage>(body);

            await _emailSender.SendAppointmentApprovedNotification(appointmentInformation);
        };

        _channel.BasicConsume(queue: _appointmentApprovedBindingParameters.QueueName, autoAck: true, consumer: consumer);
    }

    private void RegisterAppointmentNotificationQueue()
    {
        SetQueue(_appointmentNotificationBindingParameters);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentRemindNotificationMessage>(body);

            await _emailSender.SendAppointmentRemindNotification(appointmentInformation);
        };

        _channel.BasicConsume(queue: _appointmentNotificationBindingParameters.QueueName, autoAck: true, consumer: consumer);
    }

    private void RegisterAppointmentResultCreatedQueue()
    {
        SetQueue(_appointmentResultCreatedBindingParameters);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentResultCreatedMessage>(body);

            await _emailSender.SendAppointmentResultCreated(appointmentInformation);
        };

        _channel.BasicConsume(queue: _appointmentResultCreatedBindingParameters.QueueName, autoAck: true, consumer: consumer);
    }

    private void RegisterAppointmentResultUpdatedQueue()
    {
        SetQueue(_appointmentResultUpdatedBindingParameters);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentResultUpdatedMessage>(body);

            await _emailSender.SendAppointmentResultUpdated(appointmentInformation);
        };

        _channel.BasicConsume(queue: _appointmentResultUpdatedBindingParameters.QueueName, autoAck: true, consumer: consumer);
    }

    private void SetQueue(BaseBindingQueueParameters bindingQueueParameters)
    {
        _channel.ExchangeDeclare(exchange: bindingQueueParameters.ExchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: bindingQueueParameters.QueueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.QueueBind(queue: bindingQueueParameters.QueueName,
                           exchange: bindingQueueParameters.ExchangeName,
                           routingKey: bindingQueueParameters.RoutingKey,
                           arguments: null);
    }
}
using InnoClinic.SharedModels.MQMessages.Appointments;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmtpAPI.EmailService;
using System.Text.Json;

namespace SmtpAPI.RabbitMQ;

public class RabbitListener
{
    private readonly IEmailService _emailSender;
    private IConnection? _connection;
    private IModel _channel;

    public RabbitListener(IEmailService emailService)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _emailSender = emailService;
    }

    public void Register()
    {
        RegisterAppointmentApprovedQueue();
        RegisterAppointmentNotificationQueue();
        RegisterAppointmentResultCreatedQueue();
        RegisterAppointmentResultUpdatedQueue();
    }

    public void Deregister()
    {
        _channel.Close();
        _connection.Close();
    }

    private void RegisterAppointmentApprovedQueue()
    {
        var exchangeName = "appointment";
        var routingKey = "appointment.event.approved";
        var queueName = "appointment-approved-queue";

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false);

        _channel.QueueBind(queue: queueName,
                           exchange: exchangeName,
                           routingKey: queueName,
                           arguments: null);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentApprovedMessage>(body);

            await _emailSender.SendAppointmentApprovedNotification(appointmentInformation);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    private void RegisterAppointmentNotificationQueue()
    {
        var exchangeName = "appointment";
        var routingKey = "appointment.event.notificated";
        var queueName = "appointment-notificated-queue";

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false);

        _channel.QueueBind(queue: queueName,
                           exchange: exchangeName,
                           routingKey: queueName,
                           arguments: null);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentRemindNotificationMessage>(body);

            await _emailSender.SendAppointmentRemindNotification(appointmentInformation);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    private void RegisterAppointmentResultCreatedQueue()
    {
        var exchangeName = "appointment";
        var routingKey = "appointmentResult.event.created";
        var queueName = "appointmentResult-created-queue";

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false);

        _channel.QueueBind(queue: queueName,
                           exchange: exchangeName,
                           routingKey: queueName,
                           arguments: null);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentResultCreatedMessage>(body);

            await _emailSender.SendAppointmentResultCreated(appointmentInformation);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    private void RegisterAppointmentResultUpdatedQueue()
    {
        var exchangeName = "appointment";
        var routingKey = "appointmentResult.event.updated";
        var queueName = "appointmentResult-updated-queue";

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        _channel.QueueDeclare(queue: queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false);

        _channel.QueueBind(queue: queueName,
                           exchange: exchangeName,
                           routingKey: queueName,
                           arguments: null);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();

            var appointmentInformation = JsonSerializer.Deserialize<AppointmentResultUpdatedMessage>(body);

            await _emailSender.SendAppointmentResultUpdated(appointmentInformation);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }
}

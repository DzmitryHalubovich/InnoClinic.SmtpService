namespace SmtpAPI.RabbitMQ.QueuesBindingParameters;

public record BaseBindingQueueParameters(string ExchangeName,
    string QueueName, string RoutingKey);
namespace NotificationService.API.RabbitMQ.QueuesBindingParameters;

public record BaseBindingQueueParameters(string ExchangeName,
    string QueueName, string RoutingKey);
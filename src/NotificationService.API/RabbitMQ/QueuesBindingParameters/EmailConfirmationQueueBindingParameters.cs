namespace NotificationService.API.RabbitMQ.QueuesBindingParameters;

public record EmailConfirmationQueueBindingParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);

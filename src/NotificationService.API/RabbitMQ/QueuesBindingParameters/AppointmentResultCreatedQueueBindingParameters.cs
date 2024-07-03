﻿namespace NotificationService.API.RabbitMQ.QueuesBindingParameters;

public record AppointmentResultCreatedQueueBindingParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);

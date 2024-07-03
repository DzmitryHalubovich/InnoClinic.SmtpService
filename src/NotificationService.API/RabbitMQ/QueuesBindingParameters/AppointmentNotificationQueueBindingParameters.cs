﻿namespace NotificationService.API.RabbitMQ.QueuesBindingParameters;

public record AppointmentNotificationQueueBindingParameters(string ExchangeName, string QueueName, string RoutingKey) 
    : BaseBindingQueueParameters(ExchangeName, QueueName, RoutingKey);

using Common;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Consumer.Models;

namespace Consumer
{
    public class NotificationConsumer : ConsumerBase, IHostedService
    {
        protected override string QueueName => "products";
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(
            ConnectionFactory connectionFactory,
            ILogger<ConsumerBase> consumerLogger,
            ILogger<RabbitMqClientBase> rabbitMqClientBaseLogger,
            ILogger<NotificationConsumer> logConsumerLogger,
            IServiceProvider serviceProvider
            ) :
            base(connectionFactory, consumerLogger, rabbitMqClientBaseLogger, serviceProvider)
        {
             _logger = logConsumerLogger;
            
            try
            {
                var consumer = new AsyncEventingBasicConsumer(Channel);
                consumer.Received += OnEventReceived<DebeziumMessage>;  
                Channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while consuming message");
            }
        }

        public virtual Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;
        }
    }
}




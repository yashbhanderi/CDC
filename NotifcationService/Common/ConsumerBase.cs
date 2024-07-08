using System.Text;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common
{
    public abstract class ConsumerBase : RabbitMqClientBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConsumerBase> _logger;
        private readonly IServiceProvider _serviceProvider;
        protected abstract string QueueName { get; }

        public ConsumerBase(
            ConnectionFactory connectionFactory,
            ILogger<ConsumerBase> consumerLogger,
            ILogger<RabbitMqClientBase> rabbitMqClientBaseLogger,
            IServiceProvider serviceProvider) :
            base(connectionFactory, rabbitMqClientBaseLogger)
        {
            _logger = consumerLogger;
            _serviceProvider = serviceProvider;
        }

        protected virtual async Task OnEventReceived<T>(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                
                var body = Encoding.UTF8.GetString(@event.Body.ToArray());
                T message = JsonConvert.DeserializeObject<T>(body);

                await _mediator.Send(message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while retrieving message from queue.");
            }
            finally
            {
                Channel.BasicAck(@event.DeliveryTag, false);
            }
        }
    }
}

using Consumer.Hubs;
using Consumer.Models;
using MediatR;
using Newtonsoft.Json;

namespace Consumer.Commands.Handlers
{
    public class NotificationHandler : IRequestHandler<DebeziumMessage>
    {
        private readonly ILogger<NotificationHandler> _logger;
        private readonly NotificationHub _notificationHub;

        public NotificationHandler(ILogger<NotificationHandler> logger, NotificationHub notificationHub)
        {
            _logger = logger;
            _notificationHub = notificationHub;
        }

        public Task Handle(DebeziumMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("---- Received message: {Message} ----", request);
            _notificationHub.SendNotifications(JsonConvert.SerializeObject(request));
            return Task.FromResult(Unit.Value);
        }
    }
}

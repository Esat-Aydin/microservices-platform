
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ordering.Data;

namespace Ordering.Dispatcher
{

    using Entities;

    public class OutboxMessageDispatcher : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxMessageDispatcher> _logger;

        public OutboxMessageDispatcher(IServiceProvider serviceProvider, ILogger<OutboxMessageDispatcher> logger)
        {
            this._serviceProvider = serviceProvider; ;
            this._logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope scope = this._serviceProvider.CreateScope();
                OrderContext dbContext = scope.ServiceProvider.GetRequiredService<OrderContext>();

                IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                List<OutboxMessage> pendingMessages = await dbContext.OutboxMessages
                                                                     .Where(x => x.ProcessedOn == null)
                                                                     .OrderBy(x => x.OccurredOn)
                                                                     .Take(20)
                                                                     .ToListAsync(stoppingToken);

                foreach (OutboxMessage message in pendingMessages)
                {
                    try
                    {
                        // var dynamicData = JsonConvert.DeserializeObject<dynamic>(message.Content);
                        OrderCreatedEvent? orderCreatedEvent = JsonConvert.DeserializeObject<OrderCreatedEvent>(message.Content);
                        await publishEndpoint.Publish(orderCreatedEvent);

                        message.ProcessedOn = DateTime.UtcNow;
                        this._logger.LogInformation("Published outbox message {Id}", message.Id);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Failed to publish outbox message {Id}", message.Id);
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}

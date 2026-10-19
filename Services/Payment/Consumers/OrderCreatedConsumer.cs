using EventBus.Messages.Events;
using MassTransit;

namespace Payment.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(IPublishEndpoint publishEndpoint, ILogger<OrderCreatedConsumer> logger)
        {
            this._publishEndpoint = publishEndpoint;
            this._logger = logger;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            OrderCreatedEvent message = context.Message;
            this._logger.LogInformation("Processing payment for Order Id: {OrderId}", message.Id);

            //Simulate for Payment processing
            await Task.Delay(1000);
            if (message.TotalPrice > 0)
            {
                //Simulating Success
                PaymentCompletedEvent completedEvent = new PaymentCompletedEvent
                {
                    OrderId = message.Id,
                    CorrelationId = context.CorrelationId.Value 
                };
                await this._publishEndpoint.Publish(completedEvent);
                this._logger.LogInformation("Payment success for Order Id: {OrderId} and {CorrelationId}", message.Id, message.CorrelationId);
            }
            else
            {
                PaymentFailedEvent failedEvent = new PaymentFailedEvent
                {
                    OrderId = message.Id,
                    CorrelationId = context.CorrelationId.Value,
                    Reason = "Total price was zero or negative."
                };
                await this._publishEndpoint.Publish(failedEvent);
                this._logger.LogWarning("Payment failed for Order Id: {OrderId} and {CorrelationId}", message.Id, message.CorrelationId);
            }
        }
    }
}

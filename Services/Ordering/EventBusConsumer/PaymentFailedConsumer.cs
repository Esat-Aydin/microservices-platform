using EventBus.Messages.Events;
using MassTransit;
using Ordering.Repositories;

namespace Ordering.EventBusConsumer
{

    using Entities;

    public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentFailedConsumer> _logger;

        public PaymentFailedConsumer(IOrderRepository orderRepository, ILogger<PaymentFailedConsumer> logger)
        {
            this._orderRepository = orderRepository;
            this._logger = logger;
        }
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Order? order = await this._orderRepository.GetByIdAsync(context.Message.OrderId);
            if (order == null) 
            {
                this._logger.LogWarning("Order not found for Id: {OrderId} and {CorrelationId}", context.Message.OrderId, context.Message.CorrelationId);
                return;
            }
            order.Status = Entities.OrderStatus.Failed;
            await this._orderRepository.UpdateAsync(order);
            this._logger.LogInformation("Payment failed for Order Id: {OrderId}, Reason: {Reason}", context.Message.OrderId, context.Message.Reason);                 
        }
    }
}

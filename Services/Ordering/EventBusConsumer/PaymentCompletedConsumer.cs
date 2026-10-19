using EventBus.Messages.Events;
using MassTransit;
using Ordering.Repositories;

namespace Ordering.EventBusConsumer
{

    using Entities;

    public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentCompletedConsumer> _logger;

        public PaymentCompletedConsumer(IOrderRepository orderRepository, ILogger<PaymentCompletedConsumer> logger)
        {
            this._orderRepository = orderRepository;
            this._logger = logger;
        }
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Order? order = await this._orderRepository.GetByIdAsync(context.Message.OrderId);
            if (order == null) 
            {
                this._logger.LogWarning("Order not found for Id: {OrderId} and {CorrelationId}", context.Message.OrderId, context.Message.CorrelationId);
                return;
            }
            order.Status = Entities.OrderStatus.Paid;
            await this._orderRepository.UpdateAsync(order);
            this._logger.LogInformation("Order Id {OrderId} marked as Paid", context.Message.OrderId);
        }
    }
}

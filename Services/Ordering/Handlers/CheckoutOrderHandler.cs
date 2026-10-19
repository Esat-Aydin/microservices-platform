using MediatR;
using Ordering.Commands;
using Ordering.Mappers;
using Ordering.Repositories;

namespace Ordering.Handlers
{

    using Entities;

    public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckoutOrderHandler> _logger;

        public CheckoutOrderHandler(IOrderRepository orderRepository, ILogger<CheckoutOrderHandler> logger)
        {
            this._orderRepository = orderRepository;
            this._logger = logger;
        }
        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            Order orderEntity = request.ToEntity();
            Order generatedOrder = await this._orderRepository.AddAsync(orderEntity);
            OutboxMessage outboxMessage = OrderMapper.ToOutboxMessage(generatedOrder, request.CorrelationId);
            await this._orderRepository.AddOutboxMessageAsync(outboxMessage);
            this._logger.LogInformation($"Order with Id: {generatedOrder.Id} successfully created with outbox message and CorrelationId: {request.CorrelationId}.");
            return generatedOrder.Id;
        }
    }
}

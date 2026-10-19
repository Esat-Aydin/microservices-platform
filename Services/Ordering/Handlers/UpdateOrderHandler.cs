using MediatR;
using Ordering.Commands;
using Ordering.Entities;
using Ordering.Exceptions;
using Ordering.Mappers;
using Ordering.Repositories;

namespace Ordering.Handlers
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UpdateOrderHandler> _logger;

        public UpdateOrderHandler(IOrderRepository orderRepository, ILogger<UpdateOrderHandler> logger)
        {
            this._orderRepository = orderRepository;
            this._logger = logger;
        }
        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            Order? orderToUpdate = await this._orderRepository.GetByIdAsync(request.Id);
            if (orderToUpdate == null)
            {
                throw new OrderNotFoundException(nameof(Order), request.Id);
            }
            orderToUpdate.MapUpdate(request);
            await this._orderRepository.UpdateAsync(orderToUpdate);
            //Optional change: if status change needs to be known 
            OutboxMessage outBoxMessage = OrderMapper.ToOutboxMessageForUpdate(orderToUpdate, request.CorrelationId);
            await this._orderRepository.AddOutboxMessageAsync(outBoxMessage);
            this._logger.LogInformation($"Order {orderToUpdate.Id} is successfully updated.");
            return Unit.Value;
        }
    }
}

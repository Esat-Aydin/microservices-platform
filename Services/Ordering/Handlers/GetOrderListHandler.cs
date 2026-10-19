using MediatR;
using Ordering.DTOs;
using Ordering.Mappers;
using Ordering.Queries;
using Ordering.Repositories;

namespace Ordering.Handlers
{

    using Entities;

    public class GetOrderListHandler : IRequestHandler<GetOrderList, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderListHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }
        public async Task<List<OrderDto>> Handle(GetOrderList request, CancellationToken cancellationToken)
        {
            IEnumerable<Order> orders = await this._orderRepository.GetOrdersByUserName(request.UserName);
            return orders.Select(o => o.ToDto()).ToList();
        }
    }
}

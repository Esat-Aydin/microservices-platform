using Microsoft.EntityFrameworkCore;
using Ordering.Data;
using Ordering.Entities;

namespace Ordering.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext): base(dbContext) { }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            List<Order> orderList = await this._dbContext.Orders
                                              .AsNoTracking() //For better perfomance
                                              .Where(o=>o.UserName == userName)
                                              .ToListAsync();
            return orderList;
        }

        public async Task AddOutboxMessageAsync(OutboxMessage outboxMessage)
        {
            await this._dbContext.OutboxMessages.AddAsync(outboxMessage);
            await this._dbContext.SaveChangesAsync();
        }
    }
}

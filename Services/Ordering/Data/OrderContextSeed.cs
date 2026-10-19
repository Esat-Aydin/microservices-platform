using Ordering.Entities;

namespace Ordering.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation($"Ordering Database: {typeof(OrderContext).Name} seeded!!!");
            }
        }

        private static IEnumerable<Order> GetOrders()
        {
            return new List<Order>
            {
                new()
                {
                    UserName = "demo-user",
                    FirstName = "Demo",
                    LastName = "User",
                    EmailAddress = "demo.user@example.com",
                    AddressLine = "123 Example Street",
                    Country = "United States",
                    TotalPrice = 750,
                    State = "CA",
                    ZipCode = "90210",

                    CardName = "Visa",
                    CardNumber = "4111111111111111",
                    CreatedBy = "system",
                    Expiration = "12/25",
                    Cvv = "123",
                    PaymentMethod = 1,
                    LastModifiedBy = "system",
                    LastModifiedDate = new DateTime(),
                }
            };
        }
    }
}

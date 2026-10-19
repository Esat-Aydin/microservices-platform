using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Commands;
using Ordering.DTOs;
using Ordering.Mappers;
using Ordering.Queries;

namespace Ordering.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            this._mediator = mediator;
            this._logger = logger;
        }

        [HttpGet("{userName}", Name = "GetOrdersByUserName")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([FromRoute] string userName)
        {
            GetOrderList query = new GetOrderList(userName);
            List<OrderDto> orders = await this._mediator.Send(query);
            this._logger.LogInformation($"$Orders fetched for user: {userName}");
            return this.Ok(orders);
        }

        //testing purpose 
        [HttpPost(Name = "CheckoutOrder")]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CreateOrderDto dto)
        {
            //Extract Correlation id x-correlation-id
            string correlationId = this.HttpContext.Request.Headers["x-correlation-id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            CheckoutOrderCommand command = dto.ToCommand();
            command.CorrelationId = Guid.Parse(correlationId);

            int result = await this._mediator.Send(command);
            this._logger.LogInformation($"Order created with Id: {result}, CorrelationId: {correlationId}");
            return this.Ok(result);
        }

        [HttpPut(Name = "UpdateOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderDto dto)
        {
            //Extract Correlation id x-correlation-id
            string correlationId = this.HttpContext.Request.Headers["x-correlation-id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            UpdateOrderCommand command = dto.ToCommand();
            command.CorrelationId = Guid.Parse(correlationId);
            await this._mediator.Send(command);
            this._logger.LogInformation($"Order updated with Id: {dto.Id}, CorrelationId: {correlationId}");
            return this.NoContent();
        }
        [HttpDelete("{id}", Name = "DeleteOrder")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            //Extract Correlation id x-correlation-id
            string correlationId = this.HttpContext.Request.Headers["x-correlation-id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            DeleteOrderCommand command = new DeleteOrderCommand { Id = id , CorrelationId =Guid.Parse(correlationId)};
            await this._mediator.Send(command);
            this._logger.LogInformation($"Order deleted with Id: {id}, CorrelationId: {correlationId}");
            return this.NoContent();
        }
    }
}

using Basket.Commands;
using Basket.DTOs;
using Basket.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Controllers
{

    using Responses;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BasketController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        //Get: api/v1/basket/{userName}
        [HttpGet("{userName}")]
        public async Task<ActionResult<ShoppingCartDto>> GetBasket(string userName)
        { 
            GetBasketByUserNameQuery query = new GetBasketByUserNameQuery(userName);
            ShoppingCartResponse result = await this._mediator.Send(query);
            return this.Ok(result);
        }
        //Post: api/v1/basket
        [HttpPost]
        public async Task<ActionResult<ShoppingCartDto>> CreateOrUpdateBasket([FromBody] CreateShoppingCartCommand command)
        { 
            ShoppingCartResponse result = await this._mediator.Send(command);
            return this.Ok(result);
        }

        //Delete: api/v1/basket/{userName}
        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteBasket(string userName)
        { 
            DeleteBasketByUserNameCommand cmd = new DeleteBasketByUserNameCommand(userName);
            await this._mediator.Send(cmd);
            return this.Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckoutDto dto)
        {
            await this._mediator.Send(new CheckoutBasketCommand(dto));
            return this.Accepted();
        }
    }
}

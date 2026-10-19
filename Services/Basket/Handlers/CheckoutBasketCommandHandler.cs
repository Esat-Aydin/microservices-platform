using Basket.Commands;
using Basket.Mappers;
using Basket.Queries;
using MassTransit;
using MassTransit.Transports;
using MediatR;

namespace Basket.Handlers
{

    using DTOs;

    using Entities;

    using EventBus.Messages.Events;

    using Responses;

    public class CheckoutBasketCommandHandler : IRequestHandler<CheckoutBasketCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CheckoutBasketCommandHandler> _logger;

        public CheckoutBasketCommandHandler(IMediator mediator, IPublishEndpoint publishEndpoint, ILogger<CheckoutBasketCommandHandler> logger)
        {
            this._mediator = mediator;
            this._publishEndpoint = publishEndpoint;
            this._logger = logger;
        }
        public async Task<Unit> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
        {
            BasketCheckoutDto dto = request.Dto;
            ShoppingCartResponse? basketResponse = await this._mediator.Send(new GetBasketByUserNameQuery(dto.UserName), cancellationToken);
            if(basketResponse is null || !basketResponse.Items.Any())
            {
                throw new InvalidOperationException("Basket not found or empty");
            }
            ShoppingCart basket = basketResponse.ToEntity();

            //Map
            BasketCheckoutEvent evt = dto.ToBasketCheckoutEvent(basket);
            this._logger.LogInformation("Publishing BasketCheckoutEvent for {User}", basket.UserName);
            await this._publishEndpoint.Publish(evt, cancellationToken);
            //delete the basket
            await this._mediator.Send(new DeleteBasketByUserNameCommand(dto.UserName), cancellationToken);
            return Unit.Value;
        }
    }
}

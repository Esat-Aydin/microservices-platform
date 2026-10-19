using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Mappers;

namespace Ordering.EventBusConsumer
{

    using Commands;

    public class BasketOrderingConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BasketOrderingConsumer> _logger;

        public BasketOrderingConsumer(IMediator mediator, ILogger<BasketOrderingConsumer> logger)
        {
            this._mediator = mediator;
            this._logger = logger;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            using IDisposable? scope = this._logger.BeginScope("Consuming Basket Checkout Event for {CorrelationId}", context.Message.CorrelationId);
            CheckoutOrderCommand command = context.Message.ToCheckoutOrderCommand();
            int result = await this._mediator.Send(command);
            this._logger.LogInformation("Basket Checkout Event completed Successfully.");
        }
    }
}

using Basket.Mappers;
using Basket.Queries;
using Basket.Repositories;
using Basket.Responses;
using MediatR;

namespace Basket.Handlers
{

    using Entities;

    public class GetBasketByUserNameHandler : IRequestHandler<GetBasketByUserNameQuery, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;

        public GetBasketByUserNameHandler(IBasketRepository basketRepository)
        {
            this._basketRepository = basketRepository;
        }
        public async Task<ShoppingCartResponse> Handle(GetBasketByUserNameQuery request, CancellationToken cancellationToken)
        {
            ShoppingCart? shoppingCart = await this._basketRepository.GetBasket(request.UserName);
            if (shoppingCart == null)
            {
                return new ShoppingCartResponse(request.UserName)
                {
                    Items = new List<ShoppingCartItemResponse>()
                };
            }
            return shoppingCart.ToResponse();
        }
    }
}

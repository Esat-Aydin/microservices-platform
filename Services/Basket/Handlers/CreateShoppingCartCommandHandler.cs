using Basket.Commands;
using Basket.GrpcService;
using Basket.Mappers;
using Basket.Repositories;
using Basket.Responses;
using MediatR;

namespace Basket.Handlers
{

    using Discount.Grpc.Protos;

    using DTOs;

    using Entities;

    public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;

        public CreateShoppingCartCommandHandler(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
        {
            this._basketRepository = basketRepository;
            this._discountGrpcService = discountGrpcService;
        }
        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            //Apply discounts using grpc call
            foreach (CreateShoppingCartItemDto item in request.Items)
            {
                CouponModel coupon = await this._discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            // Convert Command to domain entity
            ShoppingCart shoppingCartEntity = request.ToEntity();
            //Save to Redis
            ShoppingCart updatedCart = await this._basketRepository.UpsertBasket(shoppingCartEntity);
            //Convert back to Response
            return updatedCart.ToResponse();
        }
    }
}

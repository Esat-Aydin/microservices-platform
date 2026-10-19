using Discount.Commands;
using Discount.Grpc.Protos;
using Discount.Mappers;
using Discount.Queries;
using Grpc.Core;
using MediatR;

namespace Discount.Services
{

    using DTOs;

    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IMediator _mediator;

        public DiscountService(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            GetDiscountQuery query = new GetDiscountQuery(request.ProductName);
            CouponDto dto = await this._mediator.Send(query);
            return dto.ToModel();
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            CreateDiscountCommand cmd = request.Coupon.ToCreateCommand();
            CouponDto dto = await this._mediator.Send(cmd);
            return dto.ToModel();
        }
        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            UpdateDiscountCommand cmd = request.Coupon.ToUpdateCommand();
            CouponDto dto = await this._mediator.Send(cmd);
            return dto.ToModel();
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            DeleteDiscountCommand cmd = new DeleteDiscountCommand(request.ProductName);
            bool deleted = await this._mediator.Send(cmd);
            return new DeleteDiscountResponse
            {
                Success = deleted
            };
        }
    }
}

using Discount.Commands;
using Discount.DTOs;
using Discount.Extensions;
using Discount.Mappers;
using Discount.Repositories;
using Grpc.Core;
using MediatR;

namespace Discount.Handlers
{

    using Entities;

    public class UpdateDiscountHandler : IRequestHandler<UpdateDiscountCommand, CouponDto>
    {
        private readonly IDiscountRepository _discountRepository;

        public UpdateDiscountHandler(IDiscountRepository discountRepository)
        {
            this._discountRepository = discountRepository;
        }
        public async Task<CouponDto> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            // Input Validations
            Dictionary<string, string> validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(request.ProductName))
                validationErrors["ProductName"] = "Product Name must not be empty.";
            if (string.IsNullOrWhiteSpace(request.Description))
                validationErrors["Description"] = "Product Description must not be empty.";
            if (request.Amount <= 0)
                validationErrors["Amount"] = "Amount must be greater than zero.";

            if (validationErrors.Any())
                throw GrpcErrorHelper.CreateValidationException(validationErrors);

            //Convert Command to Entity
            Coupon coupon = request.ToEntity();

            bool updated = await this._discountRepository.UpdateDiscount(coupon);

            if (!updated)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount update failed for Product = {request.ProductName}"));
            }

            //Convert Entity to DTO
            return coupon.ToDto();
        }
    }
}

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

    public class CreateDiscountHandler : IRequestHandler<CreateDiscountCommand, CouponDto>
    {
        private readonly IDiscountRepository _discountRepository;

        public CreateDiscountHandler(IDiscountRepository discountRepository)
        {
            this._discountRepository = discountRepository;
        }
        public async Task<CouponDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
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

            //Convert to Entity
            Coupon coupon = request.ToEntity();

            //Save to Db
            bool created = await this._discountRepository.CreateDiscount(coupon);
            if (!created)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Could not create discount for product: {request.ProductName}"));
            }

            //Return DTO
            return coupon.ToDto();
        }
    }
}

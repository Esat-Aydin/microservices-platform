using Discount.Commands;
using Discount.Extensions;
using Discount.Repositories;
using MediatR;

namespace Discount.Handlers
{
    public class DeleteDiscountHandler : IRequestHandler<DeleteDiscountCommand, bool>
    {
        private readonly IDiscountRepository _discountRepository;

        public DeleteDiscountHandler(IDiscountRepository discountRepository)
        {
            this._discountRepository = discountRepository;
        }
        public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ProductName))
            {
                Dictionary<string, string> validationErrors = new Dictionary<string, string>
                {
                    { "ProductName", "Product name must not be empty."}
                };
                throw GrpcErrorHelper.CreateValidationException(validationErrors);
            }

            bool deleted = await this._discountRepository.DeleteDiscount(request.ProductName);
            return deleted;
        }
    }
}

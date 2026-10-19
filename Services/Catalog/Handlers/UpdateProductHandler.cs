using Catalog.Commands;
using Catalog.Mappers;
using Catalog.Repositories;
using MediatR;

namespace Catalog.Handlers
{

    using Entities;

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductHandler(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            Product? existing = await this._productRepository.GetProduct(request.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Product with Id {request.Id} not found.");
            }
            //Step 1: Fetch Brand and Type
            ProductBrand? brand = await this._productRepository.GetBrandByIdAsync(request.Id);
            ProductType? type = await this._productRepository.GetTypeByIdAsync(request.Id);
            if (brand == null || type == null)
            {
                throw new ApplicationException("Invalid Brand or Type specified");
            }
            //Step 2: Mapper role
            Product updatedProduct = request.ToUpdateEntity(existing, brand, type);

            // Step 3: Save the record
            return await this._productRepository.UpdateProduct(updatedProduct);
        }
    }
}

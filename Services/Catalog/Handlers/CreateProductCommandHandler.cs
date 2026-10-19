using Catalog.Commands;
using Catalog.Mappers;
using Catalog.Repositories;
using Catalog.Responses;
using MediatR;

namespace Catalog.Handlers
{

    using Entities;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            //Fetch Brand and Type from repository
            ProductBrand? brand = await this._productRepository.GetBrandByIdAsync(request.BrandId);
            ProductType? type = await this._productRepository.GetTypeByIdAsync(request.TypeId);

            if (brand == null || type == null)
            {
                throw new ApplicationException("Invalid Brand or Type specified");
            }
            //Match to entity
            Product productEntity = request.ToEntity(brand, type);
            Product newProduct = await this._productRepository.CreateProduct(productEntity);
            return newProduct.ToResponse();
        }
    }
}

using Catalog.Queries;
using Catalog.Repositories;
using Catalog.Responses;
using Catalog.Specifications;
using MediatR;
using Catalog.Mappers;

namespace Catalog.Handlers
{

    using Entities;

    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, Pagination<ProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsHandler(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }
        public async Task<Pagination<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            Pagination<Product> productList = await this._productRepository.GetProducts(request.CatalogSpecParams);
            Pagination<ProductResponse> productResponseList = productList.ToResponse();
            return productResponseList;
        }
    }
}

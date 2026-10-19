using Catalog.Commands;
using Catalog.DTOs;
using Catalog.Mappers;
using Catalog.Queries;
using Catalog.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{

    using Responses;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IList<ProductDto>>> GetAllProducts([FromQuery] CatalogSpecParams catalogSpecParams)
        { 
            GetAllProductsQuery query = new GetAllProductsQuery(catalogSpecParams);
            Pagination<ProductResponse> result = await this._mediator.Send(query);
            return this.Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(string id)
        {
            GetProductByIdQuery query = new GetProductByIdQuery(id);
            ProductResponse result = await this._mediator.Send(query);
            return this.Ok(result);
        }
        [HttpGet("productName/{productName}")]
        public async Task<ActionResult<IList<ProductDto>>> GetProductByProductName(string productName)
        {
            GetProductByNameQuery query = new GetProductByNameQuery(productName);
            IList<ProductResponse>? result = await this._mediator.Send(query);
            if (result == null || !result.Any())
            {
                return this.NotFound();
            }

            List<ProductDto> dtoList = result.Select(p=>p.ToDto()).ToList();
            return this.Ok(dtoList);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductCommand productCommand)
        {
            ProductResponse result = await this._mediator.Send(productCommand);
            return this.Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        { 
            DeleteProductByIdCommand command = new DeleteProductByIdCommand(id);
            bool result = await this._mediator.Send(command);
            if (!result) 
            {
                return this.NotFound(); //Product wasn't found or maybe deletion failed
            }

            return this.NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, UpdateProductDto updateProductDto)
        {
            UpdateProductCommand command = updateProductDto.ToCommand(id);
            bool result = await this._mediator.Send(command);
            if (!result)
            {
                return this.NotFound();
            }
            return this.NoContent();
        }
        [HttpGet("GetAllBrands")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            GetAllBrandsQuery query = new GetAllBrandsQuery();
            IList<BrandResponse> result = await this._mediator.Send(query);
            return this.Ok(result);
        }
        [HttpGet("GetAllTypes")]
        public async Task<ActionResult<IEnumerable<TypeDto>>> GetTypes()
        {
            GetAllTypesQuery query = new GetAllTypesQuery();
            IList<TypesResponse> result = await this._mediator.Send(query);
            return this.Ok(result);
        }

        [HttpGet("brand/{brand}", Name = "GetProductsByBrandName")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrand(string brand)
        {
            //First get the products
            GetProductByBrandQuery query = new GetProductByBrandQuery(brand);
            IList<ProductResponse> result = await this._mediator.Send(query);
            return this.Ok(result);
        }
    }
}

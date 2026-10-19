using Catalog.Entities;
using Catalog.Specifications;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Catalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<ProductBrand> _brands;
        private readonly IMongoCollection<ProductType> _types;
        public ProductRepository(IConfiguration config) 
        {
            MongoClient client = new MongoClient(config["DatabaseSettings:ConnectionString"]);
            IMongoDatabase db = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
            this._products = db.GetCollection<Product>(config["DatabaseSettings:ProductCollectionName"]);
            this._brands = db.GetCollection<ProductBrand>(config["DatabaseSettings:BrandCollectionName"]);
            this._types = db.GetCollection<ProductType>(config["DatabaseSettings:TypeCollectionName"]);
        }
        public async Task<Product> CreateProduct(Product product)
        {
            await this._products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            DeleteResult deletedProduct = await this._products.DeleteOneAsync(p => p.Id == id);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await this._products.Find(p => true).ToListAsync();
        }

        public async Task<ProductBrand> GetBrandByIdAsync(string brandId)
        {
            return await this._brands.Find(b => b.Id == brandId).FirstOrDefaultAsync();
        }

        public async Task<Product> GetProduct(string productId)
        {
            return await this._products.Find(p=>p.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
        {
            FilterDefinitionBuilder<Product> builder = Builders<Product>.Filter;
            FilterDefinition<Product> filter = builder.Empty;
            if(!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                filter &= builder.Where(p => p.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                filter &= builder.Eq(p => p.Brand.Id, catalogSpecParams.BrandId);
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                filter &= builder.Eq(p => p.Type.Id, catalogSpecParams.TypeId);
            }

            long totalItems = await this._products.CountDocumentsAsync(filter);
            IReadOnlyCollection<Product> data = await this.ApplyDataFilters(catalogSpecParams, filter);

            return new Pagination<Product>(
                catalogSpecParams.PageIndex,
                catalogSpecParams.PageSize,
                (int)totalItems,
                data
            );
        }

      

        public async Task<IEnumerable<Product>> GetProductsByBrand(string name)
        {
            return await this._products
                             .Find(p => p.Brand.Name.ToLower() == name.ToLower())
                             .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression($".*{name}.*", "i"));
            return await this._products.Find(filter).ToListAsync();
        }

        public async Task<ProductType> GetTypeByIdAsync(string typeId)
        {
            return await this._types.Find(t=>t.Id == typeId).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            ReplaceOneResult updatedProduct = await this._products
                                                        .ReplaceOneAsync(p => p.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        private async Task<IReadOnlyCollection<Product>> ApplyDataFilters(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            SortDefinition<Product> sortDefn = Builders<Product>.Sort.Ascending("Name");
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                sortDefn = catalogSpecParams.Sort switch
                {
                    "priceAsc" => Builders<Product>.Sort.Ascending(p => p.Price),
                    "priceDesc" => Builders<Product>.Sort.Descending(p => p.Price),
                    _ => Builders<Product>.Sort.Ascending(p => p.Name)
                };
            }
            return await this._products
                             .Find(filter)
                             .Sort(sortDefn)
                             .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                             .Limit(catalogSpecParams.PageSize)
                             .ToListAsync();
        }
    }
}

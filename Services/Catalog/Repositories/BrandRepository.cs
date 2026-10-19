using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IMongoCollection<ProductBrand> _brands;
        public BrandRepository(IConfiguration config)
        {
            MongoClient client = new MongoClient(config["DatabaseSettings:ConnectionString"]);
            IMongoDatabase db = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
            this._brands = db.GetCollection<ProductBrand>(config["DatabaseSettings:BrandCollectionName"]);
        }
        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            return await this._brands.Find(_ => true).ToListAsync();
        }

        public async Task<ProductBrand> GetByIdAsync(string id)
        {
            return await this._brands.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}

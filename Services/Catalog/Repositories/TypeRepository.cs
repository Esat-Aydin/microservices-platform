using Catalog.Entities;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class TypeRepository : ITypeRepository
    {
        private readonly IMongoCollection<ProductType> _types;
        public TypeRepository(IConfiguration config)
        {
            MongoClient client = new MongoClient(config["DatabaseSettings:ConnectionString"]);
            IMongoDatabase db = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
            this._types = db.GetCollection<ProductType>(config["DatabaseSettings:TypeCollectionName"]);
        }
        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            return await this._types.Find(_ => true).ToListAsync();
        }

        public async Task<ProductType> GetByIdAsync(string id)
        {
            return await this._types.Find(t=>t.Id == id).FirstOrDefaultAsync();
        }
    }
}

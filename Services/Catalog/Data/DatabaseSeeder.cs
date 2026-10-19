using Catalog.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IConfiguration configuration)
        {
            string? connectionString = configuration["DatabaseSettings:ConnectionString"];
            string? databaseName = configuration["DatabaseSettings:DatabaseName"];

            int maxRetries = 5;
            int delaySeconds = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    MongoClient client = new MongoClient(connectionString);
                    IMongoDatabase db = client.GetDatabase(databaseName);
                    IMongoCollection<Product> products = db.GetCollection<Product>(configuration["DatabaseSettings:ProductCollectionName"]);
                    IMongoCollection<ProductBrand> brands = db.GetCollection<ProductBrand>(configuration["DatabaseSettings:BrandCollectionName"]);
                    IMongoCollection<ProductType> types = db.GetCollection<ProductType>(configuration["DatabaseSettings:TypeCollectionName"]);

                    string SeedBasePath = Path.Combine("Data", "SeedData");

                    // Seed Brands
                    List<ProductBrand> brandList = new();
                    if ((await brands.CountDocumentsAsync(_ => true)) == 0)
                    {
                        string brandData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "brands.json"));
                        brandList = JsonSerializer.Deserialize<List<ProductBrand>>(brandData) ?? new List<ProductBrand>();
                        await brands.InsertManyAsync(brandList);
                    }
                    else
                    {
                        brandList = await brands.Find(_ => true).ToListAsync();
                    }

                    // Seed Types
                    List<ProductType> typeList = new();
                    if ((await types.CountDocumentsAsync(_ => true)) == 0)
                    {
                        string typeData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "types.json"));
                        typeList = JsonSerializer.Deserialize<List<ProductType>>(typeData) ?? new List<ProductType>();
                        await types.InsertManyAsync(typeList);
                    }
                    else
                    {
                        typeList = await types.Find(_ => true).ToListAsync();
                    }

                    // Seed Products
                    if ((await products.CountDocumentsAsync(_ => true)) == 0)
                    {
                        string productData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "products.json"));
                        List<Product> productList = JsonSerializer.Deserialize<List<Product>>(productData) ?? new List<Product>();

                        foreach (Product product in productList)
                        {
                            // Reset Id to let Mongo generate one
                            product.Id = null;

                            // Default CreatedDate if not set
                            if (product.CreatedDate == default)
                                product.CreatedDate = DateTime.UtcNow;
                        }

                        await products.InsertManyAsync(productList);
                    }

                    Console.WriteLine("[DatabaseSeeder] Seeding completed successfully.");
                    break; // Exit loop if successful
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DatabaseSeeder] Attempt {attempt} failed: {ex.Message}");

                    if (attempt == maxRetries)
                    {
                        Console.WriteLine("[DatabaseSeeder] Max retries reached. Seeding aborted.");
                        throw; // rethrow after max retries
                    }

                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds * attempt)); // Exponential backoff
                }
            }
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Catalog.Domain.Helpers;

namespace Catalog.API.Data
{
    public class ApplicationDbContextSeed
    {
        public async Task SeedAsync(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger)
        {
            try
            {
                if (!context.Plates.Any())
                {
                    var filePath = Path.Combine(env.ContentRootPath, "Setup", "plates.json");
                    var json = File.ReadAllText(filePath);

                    var plates = JsonSerializer.Deserialize<List<Plate>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    });

                    if (plates != null)
                    {
                        foreach (var plate in plates)
                        {
                            logger.LogError("============= PLATE PRICE CHECK =============");
                            logger.LogError(
                                "BEFORE => Plate {Registration}: Purchase £{PurchasePrice:N2}, Sale £{SalePrice:N2}",
                                plate.Registration,
                                plate.PurchasePrice,
                                plate.SalePrice
                            );

                            var originalSalePrice = plate.SalePrice;
                            plate.SalePrice = PriceCalculator.EnsureMinimumSalePrice(
                                plate.PurchasePrice,
                                plate.SalePrice);

                            logger.LogError(
                                "AFTER => Plate {Registration}: Purchase £{PurchasePrice:N2}, Original Sale £{OriginalSale:N2}, Final Sale £{FinalSale:N2}",
                                plate.Registration,
                                plate.PurchasePrice,
                                originalSalePrice,
                                plate.SalePrice
                            );
                            logger.LogError("==========================================");
                        }

                        await context.Plates.AddRangeAsync(plates);
                        await context.SaveChangesAsync();

                        logger.LogInformation("Successfully seeded {Count} plates with calculated sale prices", plates.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding plates");
                throw;
            }
        }
    }
}


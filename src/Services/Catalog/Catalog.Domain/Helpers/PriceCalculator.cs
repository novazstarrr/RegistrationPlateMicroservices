namespace Catalog.Domain.Helpers
{
    public static class PriceCalculator
    {
        private const decimal MINIMUM_MARKUP_PERCENTAGE = 0.2m; // 20%

        public static decimal EnsureMinimumSalePrice(decimal purchasePrice, decimal salePrice)
        {
            var minimumSalePrice = purchasePrice * (1 + MINIMUM_MARKUP_PERCENTAGE);

            // Log with more visibility
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"CALCULATION FOR PLATE:");
            Console.WriteLine($"Purchase Price: £{purchasePrice:N2}");
            Console.WriteLine($"Current Sale Price: £{salePrice:N2}");
            Console.WriteLine($"Minimum Required: £{minimumSalePrice:N2}");
            Console.WriteLine($"Final Sale Price: £{Math.Max(salePrice, minimumSalePrice):N2}");
            Console.WriteLine("----------------------------------------");

            return Math.Max(salePrice, minimumSalePrice);
        }
    }
}
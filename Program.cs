using MemoryCache;
using Microsoft.Extensions.Caching.Memory;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/products", async (IMemoryCache memoryCache) =>
{
    string memoryCacheKey = "cache:products";

    if (!memoryCache.TryGetValue(memoryCacheKey, out List<Product>? products))
    {
        Console.WriteLine("Consultando productos desde la base de datos...");

        await Task.Delay(2000);

        products = [.. Enumerable
                        .Range(1, 10)
                        .Select(i => new Product
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = $"Product-{i}-{Guid.NewGuid().ToString()[..8]}"
                        })];

        Console.WriteLine("Termin� la consulta a la base de datos...");

        memoryCache.Set(memoryCacheKey, products, TimeSpan.FromHours(2));
    }

    return Results.Ok(new { products?.Count, products });
});

await app.RunAsync();

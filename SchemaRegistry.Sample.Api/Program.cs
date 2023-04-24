using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SchemaRegistry;
using SchemaRegistry.JsonStore;
using SchemaRegistry.Microsoft.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddSchemaRegistry(config =>
{
    config.WithDataStore(new JsonDataStore("./schema/"))
        .WithJson();
});
builder.Services.AddHostedService<BootstrapService>();

var app = builder.Build();

app.MapGet("/api/v{version:apiVersion}/products", 
    ActionResult (IProductService productService, ApiVersion apiVersion) =>
{
    IEnumerable<Product> products = productService.GetAll();
    switch (apiVersion.MajorVersion)
    {
        case 2:
            return new ObjectResult(products.Select(p => new { p.Id, p.Name, p.Price }));
        default:
            return products;
    }
});

app.MapGet("/api/v{version:apiVersion}/products/{id:int}", 
    IResult (IProductService productService, int id, ApiVersion apiVersion) =>
{
    Product? product = productService.GetById(id);
    if (product == null)
    {
        return new NotFoundResult();
    }

    return apiVersion.MajorVersion switch
    {
        2 => new ObjectResult(new { product.Id, product.Name, product.Price }),
        _ => product
    };
});

app.MapPost("/api/v{version:apiVersion}/products", 
    async Task<IResult> (HttpRequest request, IProductService productService, IRegistry registry) =>
    {
        ValidationResult validationResult = await registry.ValidateAsync(request.Body, "/api/products", null, null);
        if (!validationResult.IsValid) return new BadRequestObjectResult(validationResult.Message);
        using StreamReader reader = new StreamReader(request.Body, Encoding.UTF8);
        string requestBody = await reader.ReadToEndAsync();
        Product? product = JsonConvert.DeserializeObject<Product>(requestBody);
        if(product == null) return new BadRequestObjectResult("Invalid product.");
        productService.Add(product);
        return new CreatedResult($"/api/v1/products/{product.Id}", product);
});

app.Run();
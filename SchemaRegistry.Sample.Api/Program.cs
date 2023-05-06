using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
    IResult (IProductService productService, ApiVersion apiVersion) =>
    {
        IEnumerable<Product> products = productService.GetAll();
        return apiVersion.MajorVersion switch
        {
            2 => Results.Ok(new ObjectResult(products.Select(p => new { p.Id, p.Name, p.Price }))),
            _ => Results.Ok(products)
        };
    });

app.MapGet("/api/v{version:apiVersion}/products/{id:int}", 
    IResult (IProductService productService, int id, ApiVersion apiVersion) =>
{
    Product? product = productService.GetById(id);
    if (product == null) return Results.NotFound();

    return apiVersion.MajorVersion switch
    {
        2 => Results.Ok(new ObjectResult(new { product.Id, product.Name, product.Price })),
        _ => Results.Ok(product)
    };
});

app.MapPost("/api/v{version:apiVersion}/products", 
    async Task<IResult> (HttpRequest request, 
        IProductService productService, 
        IRegistry registry, 
        ApiVersion apiVersion) =>
    {
        ValidationResult validationResult = await registry.ValidateAsync(
            request.Body, 
            "/api/products", 
            version: apiVersion.MajorVersion.ToString());
        if (!validationResult.IsValid) return Results.BadRequest(validationResult.Message);
        using StreamReader reader = new StreamReader(request.Body, Encoding.UTF8);
        string requestBody = await reader.ReadToEndAsync();
        Product? product = JsonConvert.DeserializeObject<Product>(requestBody);
        if(product == null) return Results.BadRequest("Invalid product.");
        productService.Add(product);
        return Results.Created($"/api/v1/products/{product.Id}", product);
});

app.Run();
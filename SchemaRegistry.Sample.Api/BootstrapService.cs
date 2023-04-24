using SchemaRegistry;

public class BootstrapService : IHostedService
{
    private readonly IRegistry _schemaRegistry;

    public BootstrapService(IRegistry schemaRegistry)
    {
        _schemaRegistry = schemaRegistry;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ISchema productSchema = new ValidationSchema
        {
            Subject = "/api/products",
            Version = "1.0.0",
            Schema = @"{
                ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                ""type"": ""object"",
                ""properties"": {
                    ""id"": {
                        ""type"": ""integer""
                    },
                    ""name"": {
                        ""type"": ""string""
                    },
                    ""price"": {
                        ""type"": ""number"",
                        ""minimum"": 0,
                        ""exclusiveMinimum"": true
                    }
                },
                ""required"": [
                    ""id"",
                    ""name"",
                    ""price""
                ]
            }"
        };
        
        await _schemaRegistry.RegisterAsync(productSchema);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
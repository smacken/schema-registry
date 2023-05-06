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
        await _schemaRegistry.RegisterAsync(Schemas.ProductSchema);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
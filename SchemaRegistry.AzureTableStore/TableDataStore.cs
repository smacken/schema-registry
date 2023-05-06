using System.Runtime.CompilerServices;
using Azure;
using Azure.Data.Tables;

namespace SchemaRegistry.AzureTableStore;

public class ValidationSchemaEntity : ITableEntity, ISchema
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string Subject { get; }
    public string Schema { get; }
    public string Label { get; }
    public string Version { get; }

    public ValidationSchemaEntity(ISchema schema)
    {
        this.Subject = schema.Subject;
        this.Schema = schema.Schema;
        this.Label = schema.Label ?? string.Empty;
        this.Version = schema.Version ?? string.Empty;
        
        this.PartitionKey = this.Subject;
        this.RowKey = $"{this.Subject}:{this.Label}:{this.Version}";
        this.ETag = new ETag(Guid.NewGuid().ToString());
    }
}

public class TableDataStore: SchemaRegistry.IDataStore
{
    private readonly TableServiceClient _serviceClient;
    private readonly TableClient _tableClient;

    public TableDataStore(string storageUri, string accountName, string accountKey)
    {
        _serviceClient = new TableServiceClient(
            new Uri(storageUri),
            new TableSharedKeyCredential(accountName, accountKey));
        string tableName = "schema";
        _serviceClient.CreateTableIfNotExists(tableName);
        _tableClient = new TableClient(
            new Uri(storageUri),
            tableName,
            new TableSharedKeyCredential(accountName, accountKey));

        _tableClient.Create();
    }
    
    public async Task UpsertAsync(ISchema schema)
    {
        var entity = new ValidationSchemaEntity(schema);
        var results = _tableClient.Query<ValidationSchemaEntity>(x => x.PartitionKey == schema.Subject);
        var existing = results.FirstOrDefault(x => x.Label == schema.Label && x.Version == schema.Version);
        if (existing != null)
        {
            await _tableClient.UpdateEntityAsync(entity, existing.ETag, TableUpdateMode.Replace);
        }
        else
        {
            await _tableClient.AddEntityAsync(entity);
        }
    }

    public Task<ISchema> GetAsync(string subject, string? label = null, string? version = null)
    {
        var filter = $"PartitionKey eq '{subject}'";
        if (label != null) filter += $" and Label eq '{label}'";
        if (version != null) filter += $" and Version eq '{version}'";

        var filterQuery = FormattableStringFactory.Create(filter);
        var subjectFilter = TableClient.CreateQueryFilter(filterQuery);
        var results = _tableClient.Query<ValidationSchemaEntity>(subjectFilter);
        return Task.FromResult<ISchema>(new ValidationSchema
        {
            Subject = subject,
            Schema = string.Empty,
            Label = label ?? string.Empty,
            Version = version ?? string.Empty
        });
    }
}
namespace SchemaRegistry.Parquet;

public static class ConfigExtensions
{
    public static SchemaRegistryConfiguration WithParquet(this SchemaRegistryConfiguration config)
    {
        config.AddDetector(new ParquetStreamDetector());
        config.AddValidator(SchemaType.Parquet, new ParquetSchemaValidator());
        return config;
    }
}
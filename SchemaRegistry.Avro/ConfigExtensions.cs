namespace SchemaRegistry.Avro;

public static class ConfigExtensions
{
    public static SchemaRegistryConfiguration WithAvro(this SchemaRegistryConfiguration config)
    {
        config.AddDetector(new AvroStreamDetector());
        config.AddValidator(SchemaType.Avro, new AvroSchemaValidator());
        return config;
    }
}
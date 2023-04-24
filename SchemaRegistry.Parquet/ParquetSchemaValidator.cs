namespace SchemaRegistry.Parquet
{
    public class ParquetSchemaValidator : ISchemaValidator
    {
        public Task<ValidationResult> ValidateAsync(Stream schema, string knownSchema)
        {
            throw new NotImplementedException();
        }
    }
}
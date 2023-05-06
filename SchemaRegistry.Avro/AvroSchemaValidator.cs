namespace SchemaRegistry.Avro
{
    using Microsoft.Hadoop.Avro;
    public class AvroSchemaValidator : ISchemaValidator
    {
        /// <summary>
        /// Validate an avro stream against a schema to verify that the input matches the schema.
        /// Should evaluate the schema to tell if it is a valid avo schema.
        /// </summary>
        /// <param name="input">the avro input stream containing an avro payload.</param>
        /// <param name="schema">the avro schema string to evaluate against.</param>
        /// <returns>true if the input avro matches the schema, false if otherwise.</returns>
        public Task<ValidationResult> ValidateAsync(Stream input, string schema)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (input.Position != 0)
            {
                input.Position = 0;
            }

            ValidationResult result;
            var avroSettings = new AvroSerializerSettings
            {
                Resolver = new AvroPublicMemberContractResolver(true),
            };

            try
            {
                var serializer = AvroSerializer.CreateGeneric(schema);
                var value = serializer.Deserialize(input);
                result = new ValidationResult(true);
            }
            catch (Exception)
            {
                result = new ValidationResult(false, "Invalid Avro Schema.");
            }

            return Task.FromResult(result);
        }
    }
}
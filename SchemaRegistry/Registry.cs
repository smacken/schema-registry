namespace SchemaRegistry
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Schema validation registry.
    /// </summary>
    public sealed class Registry : IRegistry
    {
        private readonly IDataStore _dataStore;
        private readonly StreamDetector _schemaStreamDetector;
        private readonly IReadOnlyDictionary<SchemaType, ISchemaValidator> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry"/> class.
        /// </summary>
        /// <param name="config">Schema registry configuration.</param>
        public Registry(SchemaRegistryConfiguration config)
        {
            _dataStore = config.DataStore;
            _schemaStreamDetector = new StreamDetector(config);
            _validators = config.Validators;
        }

        /// <inheritdoc/>
        public Task RegisterAsync(ISchema schema)
        {
            if (schema?.Subject == null || schema.Schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            return _dataStore.UpsertAsync(schema);
        }

        /// <inheritdoc/>
        public async Task<ValidationResult> ValidateAsync(
            Stream inputStream,
            string subject,
            string? label = null,
            string? version = null)
        {
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            SchemaType schemaType = _schemaStreamDetector.DetectTypeFromStream(inputStream);
            if (schemaType == SchemaType.Unknown)
            {
                return new ValidationResult(false, "Unknown schema type");
            }

            ISchema? knownSchema = await _dataStore.GetAsync(subject, label, version);
            if (string.IsNullOrEmpty(knownSchema.Schema))
            {
                return new ValidationResult(false, "Schema not found");
            }

            _validators.TryGetValue(schemaType, out ISchemaValidator? validator);
            if (validator == null)
            {
                return new ValidationResult(false, "No validator found for schema type");
            }

            ValidationResult? result = await validator.ValidateAsync(inputStream, knownSchema.Schema);
            return result;
        }

        public Task<ValidationResult> ValidateAsync(Stream inputStream, string subject, string? version) =>
            ValidateAsync(inputStream, subject, null, version);

        public Task<ValidationResult> ValidateAsync(Stream inputStream, string subject) =>
            ValidateAsync(inputStream, subject, null, null);
    }
}
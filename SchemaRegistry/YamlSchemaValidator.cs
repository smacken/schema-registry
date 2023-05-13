

namespace SchemaRegistry
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using NJsonSchema;
    using NJsonSchema.Validation;
    using NJsonSchema.Yaml;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using JsonSchema = NJsonSchema.JsonSchema;

    /// <summary>
    /// Validate against a yaml schema
    /// </summary>
    public sealed class YamlSchemaValidator : ISchemaValidator
    {
        /// <inheritdoc/>
        public async Task<ValidationResult> ValidateAsync(Stream schema, string knownSchema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (knownSchema == null)
            {
                throw new ArgumentNullException(nameof(knownSchema));
            }

            if (schema.Position != 0)
            {
                schema.Position = 0;
            }

            JsonSchema? jsonSchema = await JsonSchemaYaml.FromYamlAsync(knownSchema);
            string? input = await new StreamReader(schema).ReadToEndAsync();
            JsonSchemaValidatorSettings schemaValidatorSettings = new ()
            {
                PropertyStringComparer = StringComparer.OrdinalIgnoreCase,
            };

            // format the input for yaml validation
            input = input.Replace(":", ": ");
            input = input.Replace("  ", " ");

            string errorMessage;
            try
            {
                string json = ConvertYamlToJson(new StringReader(input));
                JsonSchema? jSchema = await JsonSchema.FromJsonAsync(json);
                ICollection<ValidationError>? errors = jSchema.Validate(json, schemaValidatorSettings);
                errorMessage = string.Join(Environment.NewLine, errors.Select(x => x.ToString()));
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return new ValidationResult
            {
                IsValid = string.IsNullOrEmpty(errorMessage),
                Message = errorMessage,
            };
        }

        private Type ToBaseType(JsonObjectType jsonType)
        {
            return jsonType switch
            {
                JsonObjectType.None => typeof(object),
                JsonObjectType.Object => typeof(object),
                JsonObjectType.Array => typeof(Array),
                JsonObjectType.Integer => typeof(int),
                JsonObjectType.Number => typeof(double),
                JsonObjectType.String => typeof(string),
                JsonObjectType.Boolean => typeof(bool),
                JsonObjectType.Null => typeof(object),
                _ => throw new ArgumentOutOfRangeException(nameof(jsonType), jsonType, null)
            };
        }

        private string ConvertYamlToJson(TextReader reader)
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new ScalarTypeConverter())
                .Build();
            object? yamlObject = deserializer.Deserialize(reader);
            if (yamlObject == null)
            {
                throw new Exception("Failed to deserialize yaml");
            }

            ISerializer serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();
            return serializer.Serialize(yamlObject);
        }
    }
}
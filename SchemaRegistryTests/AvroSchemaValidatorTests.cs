using FluentAssertions;
using SchemaRegistry;
using SchemaRegistry.Avro;

namespace SchemaRegistryTests
{
    public class AvroSchemaValidatorTests
    {
        //unit test Registry.ValidateAsync for avro schema validation against avro schema for a valid avro stream
        [Fact]
        public async Task Validate_AvroSchema_Valid()
        {
            MemoryStream? stream = AvroTestHelper.CreateAvroStream();
            var config = new SchemaRegistryConfiguration
            {
                DataStore = new MemoryDataStore()
            }
                .WithAvro();

            Registry? registry = new(config);
            registry.RegisterAsync(new ValidationSchema { Subject = "avro", Schema = AvroTestHelper.AvroSchema }).Wait();
            ValidationResult? result = await registry.ValidateAsync(stream, "avro");
            result.IsValid.Should().BeTrue();
        }
    }
}
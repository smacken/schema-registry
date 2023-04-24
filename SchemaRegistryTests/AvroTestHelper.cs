using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;

namespace SchemaRegistryTests
{
    public class AvroTestHelper
    {
        public static string AvroSchema => @"
            {
              ""namespace"": ""example"",
              ""type"": ""record"",
              ""name"": ""Product"",
              ""fields"": [
                {""name"": ""Id"", ""type"": ""int""},
                {""name"": ""Name"", ""type"": ""string""},
                {""name"": ""Description"", ""type"": ""string""},
                {""name"": ""Price"", ""type"": ""double""}
              ]
            }";
        public static MemoryStream CreateAvroStream()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                Description = "This is product 1.",
                Price = 9.99
            };

            AvroSerializerSettings? settings = new AvroSerializerSettings();
            settings.Resolver = new AvroPublicMemberContractResolver();
            settings.UseCache = true;

            MemoryStream? stream = new MemoryStream();
            using var writer = AvroContainer.CreateWriter<Product>(stream, true, settings, Codec.Null);
            using var writer2 = new SequentialWriter<Product>(writer, 24);
            writer2.Write(product);
            writer2.Flush();

            return stream;
        }
    }
}
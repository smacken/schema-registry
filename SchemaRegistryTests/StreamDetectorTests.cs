using System.Text;
using FluentAssertions;
using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;
using SchemaRegistry;
using SchemaRegistry.Avro;

namespace SchemaRegistryTests
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
    public class StreamDetectorTests
    {
        [Fact]
        public void DetectTypeFromStream_Json()
        {
            //create json string for a person
            string? json = @"{
                ""firstName"": ""John"",
                ""lastName"": ""Smith"",
                ""age"": 25,
                ""address"": {
                    ""streetAddress"": ""21 2nd Street"",
                    ""city"": ""New York"",
                    ""state"": ""NY"",
                    ""postalCode"": ""10021""
                },
                ""phoneNumber"": [
                    {
                        ""type"": ""home"",
                        ""number"": ""212 555-1234""
                    },
                    {
                        ""type"": ""fax"",
                        ""number"": ""646 555-4567""
                    }
                ]
            }";

            //create json object from json
            MemoryStream? stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var config = new SchemaRegistryConfiguration().WithJson();

            StreamDetector? detector = new SchemaRegistry.StreamDetector(config);
            SchemaType result = detector.DetectTypeFromStream(stream);
            result.Should().Be(SchemaRegistry.SchemaType.Json);
        }

        //unit test StreamDetector.DetectTypeFromStream() for xml
        [Fact]
        public void DetectTypeFromStream_Xml()
        {
            //create xml string for product and add root element
            string? xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <root>
                  <Product>
                    <Id>1</Id>
                    <Name>Product 1</Name>
                    <Description>Product 1 Description</Description>
                    <Price>99.99</Price>
                  </Product>
                </root>";

            //create xml object from xml
            MemoryStream? stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

            var config = new SchemaRegistryConfiguration().WithXml();
            StreamDetector? detector = new SchemaRegistry.StreamDetector(config);
            SchemaType result = detector.DetectTypeFromStream(stream);
            result.Should().Be(SchemaRegistry.SchemaType.Xml);
        }

        //unit test StreamDetector.DetectTypeFromStream() for invalid xml
        [Fact]
        public void DetectTypeFromStream_InvalidXml()
        {
            //create xml string
            string? xml = @"dfsdagdfsghfdg";
            //create xml object from xml
            MemoryStream? stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var config = new SchemaRegistryConfiguration().WithXml();

            StreamDetector? detector = new SchemaRegistry.StreamDetector(config);
            SchemaType result = detector.DetectTypeFromStream(stream);
            result.Should().Be(SchemaRegistry.SchemaType.Unknown);
        }

        //unit test StreamDetector.DetectTypeFromStream() for avro
        [Fact]
        public void DetectTypeFromStream_Avro()
        {
            var schema = @"
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

            // Create a new Product object.
            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                Description = "This is product 1.",
                Price = 9.99
            };

            //make Product supported by the resolver for the AcroContainer
            AvroSerializerSettings? settings = new AvroSerializerSettings();
            settings.Resolver = new AvroPublicMemberContractResolver();
            settings.UseCache = true;

            //create an inmemory stream for product as an avro file with the schema as the avro schema
            MemoryStream? stream = new MemoryStream();
            using var writer = AvroContainer.CreateWriter<Product>(stream, true, settings, Codec.Null);
            using var writer2 = new SequentialWriter<Product>(writer, 24);
            writer2.Write(product);
            writer2.Flush();

            var config = new SchemaRegistryConfiguration().WithAvro();

            StreamDetector? detector = new SchemaRegistry.StreamDetector(config);
            SchemaType result = detector.DetectTypeFromStream(stream);
            result.Should().Be(SchemaRegistry.SchemaType.Avro);
        }
    }
}
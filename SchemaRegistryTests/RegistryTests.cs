using System.Text;
using FluentAssertions;
using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class RegistryTests
    {
        //unit test Registry.RegisterAsync for xml schema
        [Fact]
        public async Task Register_XmlSchema()
        {
            //create a sample xml schema with root element of the schema as <schema>
            string schema = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <xs:schema id=""root"" xmlns:xs=""http://www.w3.org/2001/XMLSchema""
                    xmlns:lib=""http://www.library.com""
                    targetNamespace=""http://www.library.com""
                    elementFormDefault=""qualified"">
                  <xs:element name=""root"">
                    <xs:complexType>
                      <xs:sequence>
                        <xs:element name=""child1"" type=""xs:string"" />
                        <xs:element name=""child2"" type=""xs:string"" />
                      </xs:sequence>
                    </xs:complexType>
                  </xs:element>
                </xs:schema>";

            //create memorystream with sample xml using xsd as schema
            string? xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <root>
                  <child1>test</child1>
                  <child2>test</child2>
                </root>";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var config = new SchemaRegistryConfiguration
            {
                DataStore = new MemoryDataStore()
            };

            Registry? registry = new Registry(config);
            await registry.RegisterAsync(new ValidationSchema { Subject = "xml", Schema = schema });
            ValidationResult? result = await registry.ValidateAsync(stream, "xml");
            result.IsValid.Should().BeFalse();
        }

        //unit test Registry.RegisterAsync for json schema
        [Fact]
        public async Task Register_JsonSchema()
        {
            string? jsonSchema = @"{
                ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                ""$id"": ""http://example.com/product.schema.json"",
                ""title"": ""Product"",
                ""description"": ""A product from Acme's catalog"",
                ""type"": ""object"",
                ""properties"": {
                  ""productId"": {
                    ""description"": ""The unique identifier for a product"",
                    ""type"": ""integer""
                  },
                  ""productName"": {
                    ""description"": ""Name of the product"",
                    ""type"": ""string""
                  },
                  ""price"": {
                    ""type"": ""number"",
                    ""exclusiveMinimum"": 0
                  },
                  ""tags"": {
                    ""type"": ""array"",
                    ""items"": {
                      ""type"": ""string""
                    },
                    ""minItems"": 1,
                    ""uniqueItems"": true
                  },
                  ""dimensions"": {
                    ""type"": ""object"",
                    ""properties"": {
                      ""length"": {
                        ""type"": ""number""
                      },
                      ""width"": {
                        ""type"": ""number""
                      },
                      ""height"": {
                        ""type"": ""number""
                      }
                    },
                    ""required"": [""length"", ""width"", ""height""]
                  },
                },
                ""required"": [""productId"", ""productName"", ""price""]
              }";

            //create json string from jsonSchema and create memory stream from json string
            string? json = @"{
                ""productId"": 1,
                ""productName"": ""A green door"",
                ""price"": 12.50,
                ""tags"": [""home"", ""green""],
            }";
            MemoryStream? stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            Registry? registry = new Registry(new SchemaRegistryConfiguration { DataStore = new MemoryDataStore() }.WithJson());
            await registry.RegisterAsync(new ValidationSchema
            {
                Subject = "json",
                Schema = jsonSchema
            });
            ValidationResult? result = await registry.ValidateAsync(stream, "json");
            result.IsValid.Should().BeTrue();
        }
        
        //unit test Registry.RegisterAsync for a duplicate schema
        [Fact]
        public async Task Register_DuplicateSchema()
        {
            string? jsonSchema = @"{
                ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                ""$id"": ""http://example.com/product.schema.json"",
                ""title"": ""Product"",
                ""description"": ""A product from Acme's catalog"",
                ""type"": ""object"",
                ""properties"": {
                  ""productId"": {
                    ""description"": ""The unique identifier for a product"",
                    ""type"": ""integer""
                  },
                  ""productName"": {
                    ""description"": ""Name of the product"",
                    ""type"": ""string""
                  },
                  ""price"": {
                    ""type"": ""number"",
                    ""exclusiveMinimum"": 0
                  },
                  ""tags"": {
                    ""type"": ""array"",
                    ""items"": {
                      ""type"": ""string""
                    },
                    ""minItems"": 1,
                    ""uniqueItems"": true
                  },
                  ""dimensions"": {
                    ""type"": ""object"",
                    ""properties"": {
                      ""length"": {
                        ""type"": ""number""
                      },
                      ""width"": {
                        ""type"": ""number""
                      },
                      ""height"": {
                        ""type"": ""number""
                      }
                    },
                    ""required"": [""length"", ""width"", ""height""]
                  },
                },
                ""required"": [""productId"", ""productName"", ""price""]
              }";

            //create json string from jsonSchema and create memory stream from json string
            string? json = @"{
                ""productId"": 1,
                ""productName"": ""A green door"",
                ""price"": 12.50,
                ""tags"": [""home"", ""green""],
            }";
            MemoryStream? stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            Registry? registry = new Registry(new SchemaRegistryConfiguration { DataStore = new MemoryDataStore() }.WithJson());
            await registry.RegisterAsync(new ValidationSchema
            {
                Subject = "json",
                Schema = jsonSchema
            });
            await registry.RegisterAsync(new ValidationSchema
            {
                Subject = "json",
                Schema = jsonSchema
            });
            ValidationResult? result = await registry.ValidateAsync(stream, "json");
            result.IsValid.Should().BeTrue();
        }
        
        //unit test Registry.RegisterAsync for a null schema should throw exception
        [Fact]
        public async Task Register_NullSchema()
        {
            Registry? registry = new Registry(new SchemaRegistryConfiguration { DataStore = new MemoryDataStore() }.WithJson());
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await registry.RegisterAsync(new ValidationSchema
            {
                Subject = "json",
                Schema = null
            }));
        }
    }
}
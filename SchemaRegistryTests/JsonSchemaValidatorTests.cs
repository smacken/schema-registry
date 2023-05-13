using System.Text;
using FluentAssertions;
using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class JsonSchemaValidatorTests
    {
        //unit test JsonSchemaValidator.ValidateAsync() for valid json against a schema
        [Fact]
        public void ValidateAsync_Valid()
        {
            string? json = @"{
            ""name"": ""John"",
            ""age"": 30,
            ""cars"": [
                ""Ford"",
                ""BMW"",
                ""Fiat""
            ]
        }";
            string? schema = @"{
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""$id"": ""http://example.com/product.schema.json"",
            ""title"": ""Product"",
            ""description"": ""A product from Acme's catalog"",
            ""type"": ""object"",
            ""properties"": {
                ""name"": {
                    ""description"": ""Name of the product"",
                    ""type"": ""string""
                },
                ""age"": {
                    ""description"": ""Age of the product"",
                    ""type"": ""integer""
                },
                ""cars"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""type"": ""string""
                    }
                }
            },
            ""required"": [""name"", ""age""]
        }";
            JsonSchemaValidator? validator = new JsonSchemaValidator();
            ValidationResult? result = validator.ValidateAsync(new MemoryStream(Encoding.UTF8.GetBytes(json)), schema)
                .Result;
            result.IsValid.Should().BeTrue();
        }

        //unit test JsonSchemaValidator.ValidateAsync() for invalid json against a schema
        [Fact]
        public void ValidateAsync_Invalid()
        {
            string? json = @"{
            ""name"": ""John"",
            ""age"": 30,
            ""cars"": [
                ""Ford"",
                ""BMW"",
                ""Fiat""
            ]
        }";
            string? schema = @"{
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""$id"": ""http://example.com/product.schema.json"",
            ""title"": ""Product"",
            ""description"": ""A product from Acme's catalog"",
            ""type"": ""object"",
            ""properties"": {
                ""name"": {
                    ""description"": ""Name of the product"",
                    ""type"": ""string""
                },
                ""age"": {
                    ""description"": ""Age of the product"",
                    ""type"": ""integer""
                },
                ""cars"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""type"": ""string""
                    }
                }
            },
            ""required"": [""name"", ""age"", ""color""]
        }";
            JsonSchemaValidator? validator = new JsonSchemaValidator();
            ValidationResult? result = validator.ValidateAsync(new MemoryStream(Encoding.UTF8.GetBytes(json)), schema)
                .Result;
            result.IsValid.Should().BeFalse();
        }

        //unit test JsonSchemaValidator.ValidateAsync() for small size json against a schema
        [Fact]
        public void ValidateAsync_SmallSize()
        {
            string? json = @"{
            ""name"": ""John"",
            ""age"": 30,
            ""cars"": [
                ""Ford"",
                ""BMW"",
                ""Fiat""
            ]
        }";
            string? schema = @"{
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""$id"": ""http://example.com/product.schema.json"",
            ""title"": ""Product"",
            ""description"": ""A product from Acme's catalog"",
            ""type"": ""object"",
            ""properties"": {
                ""name"": {
                    ""description"": ""Name of the product"",
                    ""type"": ""string""
                },
                ""age"": {
                    ""description"": ""Age of the product"",
                    ""type"": ""integer""
                },
                ""cars"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""type"": ""string""
                    }
                }
            },
            ""required"": [""name"", ""age""]
        }";
            JsonSchemaValidator? validator = new JsonSchemaValidator();
            ValidationResult? result = validator.ValidateAsync(new MemoryStream(Encoding.UTF8.GetBytes(json)), schema)
                .Result;
            result.IsValid.Should().BeTrue();
        }

        //unit test JsonSchemaValidator.ValidateAsync() for large size json against a schema
        [Fact]
        public void ValidateAsync_LargeSize()
        {
            string? json = @"{
                ""name"": ""John"",
                ""age"": 30,
                ""cars"": [
                    ""Ford"",
                    ""BMW"",
                    ""Fiat""
                ]
            }";
            string? schema = @"{
                ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                ""$id"": ""http://example.com/product.schema.json"",
                ""title"": ""Product"",
                ""description"": ""A product from Acme's catalog"",
                ""type"": ""object"",
                ""properties"": {
                    ""name"": {
                        ""description"": ""Name of the product"",
                        ""type"": ""string""
                    },
                    ""age"": {
                        ""description"": ""Age of the product"",
                        ""type"": ""integer""
                    },
                    ""cars"": {
                        ""type"": ""array"",
                        ""items"": {
                            ""type"": ""string""
                        }
                    }
                },
                ""required"": [""name"", ""age""]
            }";
            JsonSchemaValidator? validator = new JsonSchemaValidator();
            ValidationResult? result = validator.ValidateAsync(new MemoryStream(Encoding.UTF8.GetBytes(json)), schema)
                .Result;
            result.IsValid.Should().BeTrue();
        }
    }
}
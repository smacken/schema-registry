using System.Text;
using FluentAssertions;
using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class YamlSchemaValidatorTests
    {
        //unit test YamlSchemaValidator for valid yaml against a schema
        [Fact]
        public void ValidateAsync_Valid()
        {
            string? yaml = @"name: John
age: 30
cars:
    - Ford
    - BMW
    - Fiat
";
            string? schema = @"
$schema: http://json-schema.org/draft-07/schema#
$id: http://example.com/product.schema.json
title: Product
description: A product from Acme's catalog
type: object
properties:
    name:
        description: Name of the product
        type: string
    age:
        description: Age of the product
        type: integer
    cars:
        type: array
        items:
            type: string
required: [name, age]
";
            YamlSchemaValidator? validator = new ();
            ValidationResult? result = validator.ValidateAsync(new MemoryStream(Encoding.UTF8.GetBytes(yaml)), schema)
                .Result;
            result.IsValid.Should().BeTrue(result.Message);
        }
        
        //unit test YamlSchemaValidator for an invalid yaml against a schema
        [Fact]
        public void ValidateAsync_Invalid()
        {
            string? yaml = @"%YAML 1.0
            ---
            name: John
            age: 30
            cars:
                - Ford
                - BMW
                - Fiat";
            string? schema = @"---
            $schema: http://json-schema.org/draft-07/schema#
            $id: http://example.com/product.schema.json
            title: Product
            description: A product from Acme's catalog
            type: object
            properties:
                name:
                    description: Name of the product
                    type: string
                age:
                    description: Age of the product
                    type: integer
                cars:
                    type: array
                    items:
                        type: string
            required: [name, age]";
            YamlSchemaValidator? validator = new YamlSchemaValidator();
            ValidationResult? result = validator.ValidateAsync(new MemoryStream(Encoding.UTF8.GetBytes(yaml)), schema)
                .Result;
            result.IsValid.Should().BeFalse();
        }
    }
}
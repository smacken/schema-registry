using System.Text;
using System.Xml.Linq;
using FluentAssertions;
using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class XmlSchemaValidationTests
    {
        //unit test Registry.ValidateAsync for xml schema validation against xml schema for a valid xml stream
        [Fact]
        public async Task Validate_XmlSchema_Valid()
        {
            //create and xml xsd schema string for product with name description price and id
            string xsd = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <xs:schema id=""Product"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
                  <xs:element name=""Product"">
                    <xs:complexType>
                      <xs:sequence>
                        <xs:element name=""name"" type=""xs:string"" />
                        <xs:element name=""description"" type=""xs:string"" />
                        <xs:element name=""price"" type=""xs:decimal"" />
                        <xs:element name=""id"" type=""xs:integer"" />
                      </xs:sequence>
                    </xs:complexType>
                  </xs:element>
                </xs:schema>";
            //create xml schema string for Product with name description, price and id
            string? xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Product>
                  <name>test</name>
                  <description>test</description>
                  <price>1.00</price>
                  <id>1</id>
                </Product>";

            MemoryStream? stream2 = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            Registry? registry = new Registry(new SchemaRegistryConfiguration { DataStore = new MemoryDataStore() }.WithXml());
            registry.RegisterAsync(new ValidationSchema { Subject = "xml", Schema = xsd }).Wait();
            ValidationResult? result = await registry.ValidateAsync(stream2, "xml");
            result.IsValid.Should().BeTrue();
        }
    }
}
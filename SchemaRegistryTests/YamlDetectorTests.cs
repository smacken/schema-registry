using System.Text;
using FluentAssertions;
using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class YamlDetectorTests
    {
        [Fact]
        public void CanDetect_Valid()
        {
            string? yaml = @"---
                name: John
                age: 30  
            ";
            YamlStreamDetector? detector = new ();
            bool? result = detector.CanDetect(new MemoryStream(Encoding.UTF8.GetBytes(yaml)));
            result.Should().BeTrue();
        }
        
        [Fact]
        public void CanDetect_Invalid()
        {
            string? json = @"{
                ""name"": ""John"",
                ""age"": 30
            }";
            YamlStreamDetector? detector = new ();
            bool? result = detector.CanDetect(new MemoryStream(Encoding.UTF8.GetBytes(json)));
            result.Should().BeTrue();
        }
        
        [Fact]
        public void CanDetect_Empty()
        {
            YamlStreamDetector? detector = new ();
            bool? result = detector.CanDetect(new MemoryStream());
            result.Should().BeFalse();
        }
    }
}
using Moq;
using SchemaRegistry;
using SchemaRegistry.Parquet;

namespace SchemaRegistryTests
{
    public class SchemaRegistryConfigurationTests
    {
        [Fact]
        public void WithParquet_AddsParquetStreamDetector()
        {
            var config = new SchemaRegistryConfiguration();
            config.WithParquet();
            Assert.Contains(config.Detectors, d => d is ParquetStreamDetector);
        }
        
        [Fact]
        public void WithParquet_AddsParquetSchemaValidator()
        {
            var config = new SchemaRegistryConfiguration();
            config.WithParquet();
            Assert.Contains(config.Validators, v => v.Value is ParquetSchemaValidator);
        }
        
        [Fact]
        public void WithParquet_AddsParquetSchemaType()
        {
            var config = new SchemaRegistryConfiguration();
            config.WithParquet();
            Assert.Contains(config.Validators, v => v.Key == SchemaType.Parquet);
        }
        
        
        //unit test AddDetector
        [Fact]
        public void AddDetector_AddsDetector()
        {
            var config = new SchemaRegistryConfiguration();
            var detector = new Mock<IStreamDetectorStrategy>();
            config.AddDetector(detector.Object);
            Assert.Contains(config.Detectors, d => d == detector.Object);
        }
        
        
        //unit test AddValidator
        [Fact]
        public void AddValidator_AddsValidator()
        {
            var config = new SchemaRegistryConfiguration();
            var validator = new Mock<ISchemaValidator>();
            config.AddValidator(SchemaType.Json, validator.Object);
            Assert.Contains(config.Validators, v => v.Value == validator.Object);
        }
        
        //unit test WithDataStore
        [Fact]
        public void WithDataStore_AddsDataStore()
        {
            var config = new SchemaRegistryConfiguration();
            var dataStore = new Mock<IDataStore>();
            config.WithDataStore(dataStore.Object);
            Assert.Equal(config.DataStore, dataStore.Object);
        }
        
        //unit test WithJson
        [Fact]
        public void WithJson_AddsJsonStreamDetector()
        {
            var config = new SchemaRegistryConfiguration();
            config.WithJson();
            Assert.Contains(config.Detectors, d => d is JsonStreamDetector);
        }
        
        //unit test WithXml
        [Fact]
        public void WithXml_AddsXmlStreamDetector()
        {
            var config = new SchemaRegistryConfiguration();
            config.WithXml();
            Assert.Contains(config.Detectors, d => d is XmlStreamDetector);
        }
        
    }
}
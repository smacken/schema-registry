using System.Text;
using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class JsonStreamDetectorTests
    {
        //unit test JsonStreamDetector.CanDetect
        [Fact]
        public void CanDetect_WhenStreamIsJson_ReturnsTrue()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"name\":\"John Doe\"}"));
            var detector = new JsonStreamDetector();

            // Act
            var result = detector.CanDetect(stream);

            // Assert
            Assert.True(result);
        }
        
        //unit test JsonStreamDetector.Detect
        [Fact]
        public void Detect_WhenStreamIsJson_ReturnsJson()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"name\":\"John Doe\"}"));
            var detector = new JsonStreamDetector();

            // Act
            var result = detector.Detect(stream);

            // Assert
            Assert.Equal(SchemaType.Json, result);
        }
    }
}
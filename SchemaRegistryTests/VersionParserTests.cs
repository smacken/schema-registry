using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class VersionParserTests
    {
        //unit test VersionParser.VersionIsNumber
        [Fact]
        public void VersionIsNumber_WhenVersionIsNumber_ReturnsTrue()
        {
            //arrange
            string version = "1";
            
            //act
            bool result = VersionParser.VersionIsNumber(version);
            
            //assert
            Assert.True(result);
        }
        
        //unit test VersionParser.VersionIsNumber with semver version
        [Fact]
        public void VersionIsNumber_WhenVersionIsSemver_ReturnsFalse()
        {
            //arrange
            string version = "1.0.0";
            
            //act
            bool result = VersionParser.VersionIsNumber(version);
            
            //assert
            Assert.False(result);
        }
        
        
        //unit test VersionParser.VersionIsSemver
        [Fact]
        public void VersionIsSemver_WhenVersionIsSemver_ReturnsTrue()
        {
            //arrange
            string version = "1.0.0";
            
            //act
            bool result = VersionParser.VersionIsSemver(version);
            
            //assert
            Assert.True(result);
        }
        //unit test VersionParser.StringIsSemver
        [Fact]
        public void StringIsSemver_WhenVersionIsSemver_ReturnsTrue()
        {
            //arrange
            string version = "1.0.0";
            
            //act
            bool result = VersionParser.StringIsSemver(version);
            
            //assert
            Assert.True(result);
        }
        
        //unit test VersionParser.Parse
        [Fact]
        public void Parse_WhenVersionIsValid_ReturnsSemanticVersion()
        {
            //arrange
            string version = "1.0.0";
            
            //act
            SemanticVersion result = VersionParser.Parse(version);
            
            //assert
            Assert.Equal(1, result.Major);
            Assert.Equal(0, result.Minor);
            Assert.Equal(0, result.Patch);
        }
        
        //unit test VersionParser.GetLatestVersion
        [Fact]
        public void GetLatestVersion_WhenVersionsAreValid_ReturnsLatestVersion()
        {
            //arrange
            string[] versions = new string[] {"1.0.0", "1.0.1", "1.0.2"};
            
            //act
            string result = VersionParser.GetLatestVersion(versions);
            
            //assert
            Assert.Equal("1.0.2", result);
        }
        
        //unit test semver without patch
        [Fact]
        public void GetLatestVersion_WhenVersionsAreValidWithoutPatch_ReturnsLatestVersion()
        {
            //arrange
            string[] versions = new string[] {"1.0", "1.0.1", "1.0.2"};
            
            //act
            string result = VersionParser.GetLatestVersion(versions);
            
            //assert
            Assert.Equal("1.0.2", result);
        }
        
    }
}
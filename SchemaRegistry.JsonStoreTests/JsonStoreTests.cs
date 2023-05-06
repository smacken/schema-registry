using SchemaRegistry.JsonStore;

namespace SchemaRegistry.JsonStoreTests;

public class JsonStoreTests
{
    string filePath = "../../../schema.json";
    
    public JsonStoreTests()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    
    //unit test JsonDataStore.UpsertAsync with local file
    [Fact]
    public async Task UpsertAsync_WhenLocalFile_ReturnsTask()
    {
        //arrange
        ISchema schema = new ValidationSchema
        {
            Subject = "api/products",
            Schema = "{\"type\":\"record\",\"name\":\"Product\",\"namespace\":\"api\",\"fields\":[{\"name\":\"Id\",\"type\":\"int\"},{\"name\":\"Name\",\"type\":\"string\"},{\"name\":\"Price\",\"type\":\"double\"}]}",
            Label = "dev",
            Version = "1.0.0"
        };
        JsonDataStore jsonDataStore = new(filePath);

        //act
        await jsonDataStore.UpsertAsync(schema);

        //assert
        Assert.True(File.Exists(filePath));
    }
    
    //unit test JsonDataStore.UpsertAsync with local file and GetAsync can retrieve the schema
    [Fact]
    public async Task UpsertAsync_WhenLocalFile_ReturnsTaskAndCanRetrieveSchema()
    {
        //arrange
        ISchema schema = new ValidationSchema
        {
            Subject = "api/products",
            Schema = "{\"type\":\"record\",\"name\":\"Product\",\"namespace\":\"api\",\"fields\":[{\"name\":\"Id\",\"type\":\"int\"},{\"name\":\"Name\",\"type\":\"string\"},{\"name\":\"Price\",\"type\":\"double\"}]}",
            Label = "dev",
            Version = "1.0.0"
        };
        JsonDataStore jsonDataStore = new(filePath);

        //act
        await jsonDataStore.UpsertAsync(schema);
        ISchema actual = await jsonDataStore.GetAsync(schema.Subject);

        //assert
        Assert.Equal(schema.Subject, actual.Subject);
        Assert.Equal(schema.Schema, actual.Schema);
        Assert.Equal(schema.Label, actual.Label);
        Assert.Equal(schema.Version, actual.Version);
    }
    
    //unit test JsonDataStore.UpsertAsync with multiple versions to local file and GetAsync can retrieve the schema for max version
    [Fact]
    public async Task UpsertAsync_WhenLocalFile_ReturnsTaskAndCanRetrieveSchemaForMaxVersion()
    {
        //arrange
        ISchema schema1 = new ValidationSchema
        {
            Subject = "api/products",
            Schema = "{\"type\":\"record\",\"name\":\"Product\",\"namespace\":\"api\",\"fields\":[{\"name\":\"Id\",\"type\":\"int\"},{\"name\":\"Name\",\"type\":\"string\"},{\"name\":\"Price\",\"type\":\"double\"}]}",
            Label = "dev",
            Version = "1.0.0"
        };
        ISchema schema2 = new ValidationSchema
        {
            Subject = "api/products",
            Schema = "{\"type\":\"record\",\"name\":\"Product\",\"namespace\":\"api\",\"fields\":[{\"name\":\"Id\",\"type\":\"int\"},{\"name\":\"Name\",\"type\":\"string\"},{\"name\":\"Price\",\"type\":\"double\"}]}",
            Label = "dev",
            Version = "1.0.1"
        };
        JsonDataStore jsonDataStore = new(filePath);

        //act
        await jsonDataStore.UpsertAsync(schema1);
        await jsonDataStore.UpsertAsync(schema2);
        ISchema actual = await jsonDataStore.GetAsync(schema1.Subject);

        //assert
        Assert.Equal(schema2.Subject, actual.Subject);
        Assert.Equal(schema2.Schema, actual.Schema);
        Assert.Equal(schema2.Label, actual.Label);
        Assert.Equal(schema2.Version, actual.Version);
    }
}
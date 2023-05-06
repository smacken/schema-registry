using System.Net;
using Moq;
using SchemaRegistry.RedisStore;
using StackExchange.Redis;

namespace SchemaRegistry.RedisStoreTests;

public class RedisHelperTests
{
    //unit test RedisHelper.GetValueForKeyAsync with just the key parameter
    [Fact]
    public async Task GetValueForKeyAsync_WhenKeyParameter_ReturnsValue()
    {
        var mockMultiplexer = new Mock<IConnectionMultiplexer>();
        mockMultiplexer.Setup(_ => _.IsConnected).Returns(false);
        var mockServer = new Mock<IServer>();
        var mockEndPoint = new Mock<EndPoint>();
        mockServer.Setup(_ => _.EndPoint).Returns(mockEndPoint.Object);
        mockMultiplexer.Setup(_ => _.GetEndPoints(It.IsAny<bool>())).Returns(new[] {mockEndPoint.Object});
        mockMultiplexer.Setup(_ => _.GetServer(It.IsAny<EndPoint>(), It.IsAny<object>())).Returns(mockServer.Object);
        var mockDatabase = new Mock<IDatabase>();
        mockMultiplexer
            .Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDatabase.Object);
        mockServer.Setup(_ => _.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(new RedisKey[] {"api/products"});
        
        //arrange
        string key = "api/products";
        string expected = "{\"type\":\"record\",\"name\":\"Product\",\"namespace\":\"api\",\"fields\":[{\"name\":\"Id\",\"type\":\"int\"},{\"name\":\"Name\",\"type\":\"string\"},{\"name\":\"Price\",\"type\":\"double\"}]}";
        RedisHelper redisHelper = new("localhost", mockMultiplexer.Object);
        
        mockDatabase.Setup(_ => _.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(expected);

        //act
        string actual = await redisHelper.GetValueForKeyAsync(key);

        //assert
        Assert.Equal(expected, actual);
    }
}
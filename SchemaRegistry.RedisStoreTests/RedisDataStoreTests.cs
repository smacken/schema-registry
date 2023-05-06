using System.Net;
using Moq;
using SchemaRegistry.RedisStore;
using StackExchange.Redis;

namespace SchemaRegistry.RedisStoreTests
{
    public class RedisDataStoreTests
    {
        //unit test RedisDataStore.UpsertAsync with mock Redis IConnectionMultiplexer
        [Fact]
        public async Task UpsertAsync_WhenRedisConnectionMultiplexer_ReturnsTask()
        {
            //arrange
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
            
            mockDatabase.Setup(_ => _.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()));
            
            ISchema schema = new ValidationSchema
            {
                Subject = "api/products",
                Schema = "{\"type\":\"record\",\"name\":\"Product\",\"namespace\":\"api\",\"fields\":[{\"name\":\"Id\",\"type\":\"int\"},{\"name\":\"Name\",\"type\":\"string\"},{\"name\":\"Price\",\"type\":\"double\"}]}",
                Label = "dev",
                Version = "1.0.0"
            };
            RedisDataStore redisDataStore = new (mockMultiplexer.Object);
            
            //act
            await redisDataStore.UpsertAsync(schema);
            
            mockDatabase.Verify(_ => _.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}
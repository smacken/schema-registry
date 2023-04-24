using StackExchange.Redis;

namespace SchemaRegistry.RedisStore
{
    internal sealed class RedisHelper
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisHelper(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
        }

        public async Task<string> GetValueForKeyAsync(string key, string? label = null, string? version = null)
        {
            if (version == null)
            {
                IEnumerable<RedisKey>? keys = _redis.GetServer(_redis.GetEndPoints()[0]).Keys(pattern: key + ":*");
                int maxVersion = keys.Select(k => int.Parse(k.ToString().Split(':')[1])).Max();
                Task<RedisValue>? result = _redis.GetDatabase().StringGetAsync(key + ":" + maxVersion) ??
                                           _redis.GetDatabase().StringGetAsync(key);
                return result.Result.ToString();
            }

            RedisValue keyValue = await _redis.GetDatabase().StringGetAsync(key + ":" + version);
            return keyValue.ToString();
        }
    }
}
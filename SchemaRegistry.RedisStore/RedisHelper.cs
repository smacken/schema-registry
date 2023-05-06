using StackExchange.Redis;

namespace SchemaRegistry.RedisStore
{
    public sealed class RedisHelper
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisHelper(string connectionString, IConnectionMultiplexer? redis = null)
        {
            _redis = redis ?? ConnectionMultiplexer.Connect(connectionString);
        }

        public async Task<string> GetValueForKeyAsync(string key, string? label = null, string? version = null)
        {
            if (version == null)
            {
                string keyPattern = label == null ? key + ":*" : key + ":" + label + ":*";
                string maxKeyPattern = label == null ? key + ":*:*" : key + ":" + label + ":*";
                var endpoint = _redis.GetEndPoints()[0];
                IEnumerable<RedisKey>? keys = _redis.GetServer(endpoint).Keys(pattern: keyPattern);
                var versions = keys.Select(k => k.ToString().Split(':').Last()).ToArray();
                string maxVersion = VersionParser.GetLatestVersion(versions);
                Task<RedisValue>? result = _redis.GetDatabase().StringGetAsync(maxVersion) ??
                                           _redis.GetDatabase().StringGetAsync(key);
                return result.Result.ToString();
            }

            string keysPattern = key + ":" + (label ?? "*") + (":" + version);
            IEnumerable<RedisKey>? keysRange = _redis.GetServer(_redis.GetEndPoints()[0]).Keys(pattern: keysPattern);
            if (keysPattern.IndexOf(':') > 2)
            {
                var versions = keysRange.Select(k => k.ToString().Split(':').Last()).ToArray();
                string maxVersion = VersionParser.GetLatestVersion(versions);
            }

            RedisValue keyValue = await _redis.GetDatabase().StringGetAsync(keysPattern);
            return keyValue.ToString();
        }
        
        public async Task<string> GetValueForKeyWithSortAsync(string key, string? label = null, string? version = null)
        {
            var db = _redis.GetDatabase();
            var keyPattern = $"{key}:{label ?? "*"}:{version ?? "*"}";
            RedisResult? keysResult = await db.ExecuteAsync("SCAN", "0", "MATCH", keyPattern);
            if (keysResult.IsNull) return string.Empty;
            var keys = ((RedisResult[])keysResult)[1];
            return keys.ToString() ?? string.Empty;
        }

    }
}
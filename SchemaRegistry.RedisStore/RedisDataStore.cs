namespace SchemaRegistry.RedisStore
{
    using System.Text;
    using StackExchange.Redis;

    /// <summary>
    /// Schema registry data store using Redis.
    /// </summary>
    public class RedisDataStore : IDataStore
    {
        private readonly IDatabase _database;
        private readonly RedisHelper _redisHelper;

        /// <summary>
        /// Create an instance of the RedisDataStore.
        /// </summary>
        public RedisDataStore(IConnectionMultiplexer? redisConnection = null)
        {
            IConnectionMultiplexer redis = redisConnection ?? ConnectionMultiplexer.Connect("localhost");
            _database = redis.GetDatabase();
            _redisHelper = new RedisHelper("localhost", redis);
        }

        /// <inheritdoc />
        /// <remarks>'api/products:dev:1.0.0'</remarks>
        public Task UpsertAsync(ISchema schema)
        {
            StringBuilder? sb = new();
            sb.Append(schema.Subject);
            if (!string.IsNullOrEmpty(schema.Label))
            {
                sb.Append(':');
                sb.Append(schema.Label);
            }

            if (!string.IsNullOrEmpty(schema.Version))
            {
                sb.Append(':');
                sb.Append(schema.Version);
            }

            return _database.StringSetAsync(sb.ToString(), schema.Schema, flags: CommandFlags.FireAndForget, expiry: null, when: When.Always);
        }

        /// <inheritdoc />
        public async Task<ISchema> GetAsync(string subject, string? label, string? version)
        {
            string? storeValue = await _redisHelper.GetValueForKeyAsync(subject, label, version);
            return new ValidationSchema
            {
                Subject = subject,
                Schema = storeValue,
                Label = label ?? string.Empty,
                Version = version ?? string.Empty
            };
        }
    }
}
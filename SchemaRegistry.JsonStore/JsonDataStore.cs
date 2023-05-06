namespace SchemaRegistry.JsonStore
{
    using JsonFlatFileDataStore;
    using System.Text;

    public sealed class JsonDataStore : SchemaRegistry.IDataStore
    {
        private readonly DataStore _store;
        private readonly IDocumentCollection<JsonStore> _collection;

        public class JsonStore
        {
            public string Key { get; set; }
            public ValidationSchema Schema { get; init; }

            public JsonStore(ValidationSchema schema)
            {
                var keyText = new StringBuilder();
                keyText.Append(schema.Subject);
                if (schema.Label != null)
                {
                    keyText.Append(':');
                    keyText.Append(schema.Label);
                }
                if (schema.Version != null)
                {
                    keyText.Append(':');
                    keyText.Append(schema.Version);
                }

                Key = keyText.ToString();
                Schema = schema;
            }
        }

        public JsonDataStore(string filePath)
        {
            _store = new DataStore(filePath);
            _collection = _store.GetCollection<JsonStore>("schema");
        }

        /// <inheritdoc />
        public Task UpsertAsync(ISchema schema)
        {
            return _collection.InsertOneAsync(new JsonStore((schema as ValidationSchema)!));
        }

        /// <inheritdoc />
        public Task<ISchema> GetAsync(string subject, string? label = null, string? version = null)
        {
            IEnumerable<string> keys = GetFilterKeys(subject, label, version);

            var items = _collection.AsQueryable()
                .Where(x => keys.Contains(x.Key))
                .Select(x => x.Schema)
                .ToList();
            var itemVersions = items.Select(x => x.Version).ToArray();
            var latestVersion = VersionParser.GetLatestVersion(itemVersions);
            var latestItem = items.FirstOrDefault(x => x.Version == latestVersion);
            if (latestItem != null) return Task.FromResult<ISchema>(latestItem);

            return Task.FromResult(new EmptySchema() as ISchema);
        }

        private IEnumerable<string> GetFilterKeys(string subject, string? label, string? version)
        {
            var keys = _collection.AsQueryable()
                .Where(x => x.Key.StartsWith(subject))
                .Select(x => x.Key);

            if (label != null && version != null)
            {
                keys = keys.Where(x => x.Contains(label) && x.Contains(version));
            }
            else if (label != null)
            {
                keys = keys.Where(x => x.Contains(label));
            }
            else if (version != null)
            {
                keys = keys.Where(x => x.Contains(version));
            }

            return keys;
        }
    }
}
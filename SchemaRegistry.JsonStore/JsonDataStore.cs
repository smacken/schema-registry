using System.Text;

namespace SchemaRegistry.JsonStore
{
    using JsonFlatFileDataStore;

    public class JsonDataStore : SchemaRegistry.IDataStore
    {
        private readonly DataStore _store;
        private readonly IDocumentCollection<JsonStore> _collection;

        private class JsonStore
        {
            public string Key { get; set; }
            public ISchema Schema { get; init; }

            public JsonStore(ISchema schema)
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
            return _collection.InsertOneAsync(new JsonStore(schema));
        }

        /// <inheritdoc />
        public Task<ISchema> GetAsync(string subject, string? label = null, string? version = null)
        {
            var keys = _collection.AsQueryable()
                .Where(x => x.Key.StartsWith(subject))
                .Select(x => x.Key);
            foreach (var key in keys)
            {
                var value = _store.GetItem<JsonStore>(key);
                if (label != null && version != null)
                {
                    if (value.Schema.Label == label && value.Schema.Version == version)
                    {
                        return Task.FromResult(value.Schema);
                    }
                }
                else if (label != null)
                {
                    if (value.Schema.Label == label)
                    {
                        return Task.FromResult(value.Schema);
                    }
                }
                else if (version != null)
                {
                    if (value.Schema.Version == version)
                    {
                        return Task.FromResult(value.Schema);
                    }
                }
                else
                {
                    return Task.FromResult(value.Schema);
                }
            }

            return Task.FromResult(new EmptySchema() as ISchema);
        }
    }
}
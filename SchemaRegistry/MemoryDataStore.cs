// <copyright file="MemoryDataStore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Concurrent;

namespace SchemaRegistry
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// In memory version of a data store.
    /// </summary>
    public sealed class MemoryDataStore : IDataStore
    {
        private readonly static Task<ISchema> EmptyTask = Task.FromResult(null as ISchema);
        private readonly ConcurrentDictionary<string, ISchema> schemaStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryDataStore"/> class.
        /// </summary>
        public MemoryDataStore()
        {
            schemaStore = new ConcurrentDictionary<string, ISchema>();
        }

        public Task UpsertAsync(ISchema schema)
        {
            string key = GetKey(schema);
            schemaStore.AddOrUpdate(key, schema,
                (_, existingSchema) => existingSchema != schema ? schema : existingSchema);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<ISchema> GetAsync(string subject, string? label = null, string? version = null)
        {
            string key = subject.ToLower().Trim();
            if (!string.IsNullOrEmpty(label))
            {
                key += ":" + label.ToLower().Trim();
            }

            if (!string.IsNullOrEmpty(version))
            {
                key += ":" + version.ToLower().Trim();
            }

            return schemaStore.TryGetValue(key, out ISchema? schema)
                ? Task.FromResult(schema)
                : EmptyTask;
        }

        private static string GetKey(string subject, string? label = null, string? version = null)
        {
            string key = subject;
            if (!string.IsNullOrEmpty(label))
            {
                key = string.Concat(key, ":", label);
            }

            if (!string.IsNullOrEmpty(version))
            {
                key = string.Concat(key, ":", version);
            }

            return key.ToLowerInvariant().Trim();
        }

        private static string GetKey(ISchema schema)
        {
            return GetKey(schema.Subject, schema.Label, schema.Version);
        }
    }
}
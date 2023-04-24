// <copyright file="IDataStore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System.Threading.Tasks;

    /// <summary>
    /// A data store for schemas.
    /// </summary>
    public interface ISchema
    {
        /// <summary>
        /// Gets the subject of the schema.
        /// </summary>
        string Subject { get; }

        /// <summary>
        /// Gets the schema itself.
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// Gets if the schema is a version of another schema, the label of the parent schema.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the version of the schema.
        /// </summary>
        string Version { get; }
    }

    /// <summary>
    /// A schema registry data store.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Add or update a schema in the data store.
        /// </summary>
        /// <param name="schema">The ISchema to be added.</param>
        /// <returns>Promise of a returned value.</returns>
        Task UpsertAsync(ISchema schema);

        /// <summary>
        /// Get a schema from the data store.
        /// </summary>
        /// <param name="subject">The schema subject.</param>
        /// <param name="label">The label/tag of a schema.</param>
        /// <param name="version">If the schema has a version.</param>
        /// <returns>A matching schema if available.</returns>
        Task<ISchema> GetAsync(string subject, string? label = null, string? version = null);
    }

    public class EmptySchema : ISchema
    {
        /// <inheritdoc/>
        public string Subject { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Schema { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Label { get; } = string.Empty;

        /// <inheritdoc/>
        public string Version { get; } = string.Empty;
    }

    /// <summary>
    /// A schema for validation.
    /// </summary>
    public class ValidationSchema : ISchema
    {
        /// <inheritdoc/>
        public string Subject { get; set; }

        /// <inheritdoc/>
        public string Schema { get; set; }

        /// <inheritdoc/>
        public string Label { get; set; }

        /// <inheritdoc/>
        public string Version { get; set; }
    }
}
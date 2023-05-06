// <copyright file="IRegistry.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Schema validation registry.
    /// </summary>
    public interface IRegistry
    {
        /// <summary>
        /// Register a schema to be validated against
        /// </summary>
        /// <param name="schema">The schema in question.</param>
        /// <returns>a promise that the schema would be registered.</returns>
        Task RegisterAsync(ISchema schema);

        /// <summary>
        /// Validate a schema against the registry.
        /// </summary>
        /// <param name="inputStream">the input being evaluated against.</param>
        /// <param name="subject">the schema subject.</param>
        /// <param name="label">an additional label metadata.</param>
        /// <param name="version">the version of the schema over time.</param>
        /// <returns>a validation result from the schema validation.</returns>
        Task<ValidationResult> ValidateAsync(Stream inputStream, string subject, string? label, string? version);
        
        Task<ValidationResult> ValidateAsync(Stream inputStream, string subject, string? version);
        
        Task<ValidationResult> ValidateAsync(Stream inputStream, string subject);
    }
}
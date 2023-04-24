// <copyright file="ISchemaValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// I can validate schemas.
    /// </summary>
    public interface ISchemaValidator
    {
        /// <summary>
        /// Validate a schema against a known schema.
        /// </summary>
        /// <param name="schema">the input to be evaluated against.</param>
        /// <param name="knownSchema">the known stored schema to evaluate with.</param>
        /// <returns>promise of a validation result.</returns>
        Task<ValidationResult> ValidateAsync(Stream schema, string knownSchema);
    }
}
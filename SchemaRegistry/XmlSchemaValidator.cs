// <copyright file="XmlSchemaValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Xml schema validator.
    /// </summary>
    public sealed class XmlSchemaValidator : ISchemaValidator
    {
        /// <summary>
        /// This function validates a schema, using a known schema as a reference.
        /// The known schema is a string, and the schema to validate is a stream.
        /// The function returns a ValidationResult object.
        /// </summary>
        /// <param name="schema"> the stream to validate.</param>
        /// <param name="knownSchema">the schema to validate against.</param>
        /// <returns>validation result.</returns>
        public async Task<ValidationResult> ValidateAsync(Stream schema, string knownSchema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (knownSchema == null)
            {
                throw new ArgumentNullException(nameof(knownSchema));
            }

            if (schema.Position != 0)
            {
                schema.Position = 0;
            }

            try
            {
                XmlReader? namespaceReader = XmlReader.Create(new StringReader(knownSchema));
                namespaceReader.ReadToFollowing("schema");
                string? targetNamespace = namespaceReader.GetAttribute("targetNamespace");

                XmlSchemaSet? schemas = new();
                schemas.Add(targetNamespace ?? string.Empty, XmlReader.Create(new StringReader(knownSchema)));
                XmlReaderSettings? settings = new()
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas,
                    DtdProcessing = DtdProcessing.Ignore
                };

                XmlReader? reader = XmlReader.Create(schema, settings);
                while (reader.Read())
                {
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ValidationResult(false, e.Message);
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validate that the schema given is a valid xml schema.
        /// </summary>
        /// <param name="schema">the xml schema to evaluate.</param>
        /// <returns>true if the schema is valid xml schema, false if otherwise.</returns>
        public bool IsValidSchema(string schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            try
            {
                XmlReader? namespaceReader = XmlReader.Create(new StringReader(schema));
                namespaceReader.ReadToFollowing("schema");
                string? targetNamespace = namespaceReader.GetAttribute("targetNamespace");

                XmlSchemaSet? schemas = new();
                schemas.Add(targetNamespace ?? string.Empty, XmlReader.Create(new StringReader(schema)));
                XmlReaderSettings? settings = new()
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas,
                    DtdProcessing = DtdProcessing.Ignore
                };

                XmlReader? reader = XmlReader.Create(new StringReader(schema), settings);
                while (reader.Read())
                {
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}
// <copyright file="SchemaRegistryConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System.Collections.Generic;

    /// <summary>
    /// Schema registration configuration.
    /// </summary>
    public sealed class SchemaRegistryConfiguration
    {
        private List<IStreamDetectorStrategy> detectors = new();
        private Dictionary<SchemaType, ISchemaValidator> validators = new();

        public IDataStore DataStore { get; set; }

        public IReadOnlyList<IStreamDetectorStrategy> Detectors => detectors.AsReadOnly();

        public IReadOnlyDictionary<SchemaType, ISchemaValidator> Validators => validators;

        public SchemaRegistryConfiguration AddDetector(IStreamDetectorStrategy detector)
        {
            detectors.Add(detector);
            return this;
        }

        public SchemaRegistryConfiguration AddValidator(SchemaType type, ISchemaValidator validator)
        {
            if (validators.TryGetValue(type, out var existingValidator))
            {
                validators[type] = validator;
                return this;
            }

            validators.Add(type, validator);
            return this;
        }

        /// <summary>
        /// Configure a data store for the schema registry.
        /// </summary>
        /// <param name="dataStore">The type of data store.</param>
        /// <returns>the configuration.</returns>
        public SchemaRegistryConfiguration WithDataStore(IDataStore dataStore)
        {
            DataStore = dataStore;
            return this;
        }

        public SchemaRegistryConfiguration WithJson()
        {
            AddDetector(new JsonStreamDetector());
            AddValidator(SchemaType.Json, new JsonSchemaValidator());
            return this;
        }

        public SchemaRegistryConfiguration WithXml()
        {
            AddDetector(new XmlStreamDetector());
            AddValidator(SchemaType.Xml, new XmlSchemaValidator());
            return this;
        }
    }
}
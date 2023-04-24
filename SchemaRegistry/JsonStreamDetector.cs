// <copyright file="JsonStreamDetector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System.IO;
    using System.Text.Json;

    /// <summary>
    /// Detects if a stream is a JSON stream.
    /// </summary>
    public sealed class JsonStreamDetector : IStreamDetectorStrategy
    {
        /// <inheritdoc/>
        public bool CanDetect(Stream stream)
        {
            JsonDocumentOptions options = new JsonDocumentOptions { AllowTrailingCommas = true };

            try
            {
                using JsonDocument? jsonDoc = JsonDocument.Parse(stream, options);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public SchemaType Detect(Stream stream)
        {
            return SchemaType.Json;
        }
    }
}
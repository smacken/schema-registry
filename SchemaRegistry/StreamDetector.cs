// <copyright file="StreamDetector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IStreamDetectorStrategy
    {
        bool CanDetect(Stream stream);

        SchemaType Detect(Stream stream);
    }

    public sealed class StreamDetector
    {
        private readonly List<IStreamDetectorStrategy> _strategies;

        public StreamDetector(SchemaRegistryConfiguration config)
        {
            _strategies = new List<IStreamDetectorStrategy>(config.Detectors);
        }

        public SchemaType DetectTypeFromStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            foreach (IStreamDetectorStrategy? strategy in _strategies)
            {
                if (strategy.CanDetect(stream))
                {
                    return strategy.Detect(stream);
                }
            }

            return SchemaType.Unknown;
        }
    }
}
// <copyright file="XmlStreamDetector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    using System.IO;
    using System.Xml;

    public class XmlStreamDetector : IStreamDetectorStrategy
    {
        public bool CanDetect(Stream stream)
        {
            try
            {
                if (stream.Position != 0)
                {
                    stream.Position = 0;
                }

                XmlReaderSettings? settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                    IgnoreProcessingInstructions = true
                };
                using XmlReader? reader = XmlReader.Create(stream, settings);
                reader.Read();
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        public SchemaType Detect(Stream stream)
        {
            return SchemaType.Xml;
        }
    }
}
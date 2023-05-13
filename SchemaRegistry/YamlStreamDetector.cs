using System.IO;
using NJsonSchema;
using NJsonSchema.Yaml;

namespace SchemaRegistry
{
    public sealed class YamlStreamDetector : IStreamDetectorStrategy
    {
        /// <inheritdoc/>
        public bool CanDetect(Stream stream)
        {
            if (stream == null || stream.Length == 0)
            {
                return false;
            }

            stream.Position = 0;
            try
            {
                string yaml = new StreamReader(stream).ReadToEnd();
                JsonSchemaYaml.FromYamlAsync(yaml);
            }
            catch (YamlDotNet.Core.YamlException)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public SchemaType Detect(Stream stream)
        {
            return SchemaType.Yaml;
        }
    }
}
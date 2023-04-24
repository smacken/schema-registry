using Microsoft.Hadoop.Avro.Container;

namespace SchemaRegistry.Avro
{
    public class AvroStreamDetector : IStreamDetectorStrategy
    {
        public bool CanDetect(Stream stream)
        {
            if (stream.Position != 0) stream.Position = 0;
            try
            {
                using var avroReader = AvroContainer.CreateGenericReader(stream);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public SchemaType Detect(Stream stream)
        {
            return SchemaType.Avro;
        }
    }
}
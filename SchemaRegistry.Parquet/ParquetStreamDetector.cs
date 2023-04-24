using Parquet;

namespace SchemaRegistry.Parquet
{
    public class ParquetStreamDetector : IStreamDetectorStrategy
    {
        public bool CanDetect(Stream stream)
        {
            try
            {
                using var parquetReader = Task.Run(() => ParquetReader.ReadTableFromStreamAsync(stream));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public SchemaType Detect(Stream stream)
        {
            return SchemaType.Parquet;
        }
    }
}
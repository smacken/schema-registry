namespace SchemaRegistry
{
    using System;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    public class ScalarTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var scalar = parser.Consume<Scalar>();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                    return decimal.Parse(scalar.Value);
                case TypeCode.Double:
                    return double.Parse(scalar.Value);
                case TypeCode.Single:
                    return float.Parse(scalar.Value);
                default:
                    throw new NotSupportedException($"Conversion to type '{type.Name}' is not supported by ScalarTypeConverter.");
            }
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            emitter.Emit(new Scalar(((IFormattable)value).ToString("G", System.Globalization.CultureInfo.InvariantCulture)));
        }
    }

}
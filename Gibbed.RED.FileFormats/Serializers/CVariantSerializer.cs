namespace Gibbed.RED.FileFormats.Serializers
{
    public class CVariantSerializer: IPropertySerializer
    {
        public void Serialize(IFileStream stream, object value)
        {
            throw new System.NotImplementedException();
        }

        public object Deserialize(IFileStream stream)
        {
            string typeName = null;
            stream.SerializeName(ref typeName);
            uint valueSize = 0;
            stream.SerializeValue(ref valueSize);
            var serializer = PropertySerializer.GetSerializer(typeName);
            if (serializer != null)
            {
                var value = serializer.Deserialize(stream);
                return value;
            }
            stream.Position += valueSize - 4;
            return null;
        }
    }
}
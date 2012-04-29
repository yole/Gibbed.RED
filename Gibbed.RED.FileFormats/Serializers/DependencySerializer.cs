using System.Diagnostics;

namespace Gibbed.RED.FileFormats.Serializers
{
    public class DependencySerializer: IPropertySerializer
    {
        public void Serialize(IFileStream stream, object value)
        {
            throw new System.NotImplementedException();
        }

        public object Deserialize(IFileStream stream)
        {
            string result = null;
            stream.SerializeDependency(ref result);
            return result;
        }
    }
}
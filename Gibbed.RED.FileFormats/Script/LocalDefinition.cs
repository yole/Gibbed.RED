namespace Gibbed.RED.FileFormats.Script
{
    public class LocalDefinition
    {
        public TypeDefinition Type;
        public string Name;

        public override string ToString()
        {
            return "local " + Type.Name + " " + Name;
        }
    }
}
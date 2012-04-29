namespace Gibbed.RED.FileFormats.Game
{
    [ResourceHandler("CFootstepData")]
    public class CFootstepData: CObject
    {
        private byte[] _data;

        public override void Serialize(IFileStream stream)
        {
            var bytesRemaining = (uint) (stream.Length - stream.Position);
            stream.SerializeValue(ref _data, bytesRemaining);
        }
    }
}
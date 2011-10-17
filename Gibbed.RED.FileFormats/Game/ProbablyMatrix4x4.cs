using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.RED.FileFormats.Game
{
    public class ProbablyMatrix4x4 : IFileObject
    {
        public ProbablyMatrix A;
        public ProbablyMatrix B;
        public ProbablyMatrix C;
        public ProbablyMatrix D;

        public void Serialize(IFileStream stream)
        {
            stream.SerializeObject(ref this.A);
            stream.SerializeObject(ref this.B);
            stream.SerializeObject(ref this.C);
            stream.SerializeObject(ref this.D);
        }

        public class ProbablyMatrix : IFileObject
        {
            public float X;
            public float Y;
            public float Z;
            public float W;

            public void Serialize(IFileStream stream)
            {
                stream.SerializeValue(ref this.X);
                stream.SerializeValue(ref this.Y);
                stream.SerializeValue(ref this.Z);
                stream.SerializeValue(ref this.W);
            }
        }
    }
}

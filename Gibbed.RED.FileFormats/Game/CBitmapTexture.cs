/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using Gibbed.RED.FileFormats.Serializers;

namespace Gibbed.RED.FileFormats.Game
{
    [ResourceHandler("CBitmapTexture")]
    public class CBitmapTexture : CResource
    {
        [PropertyName("width")]
        [PropertyDescription("Width of the texture")]
        [PropertySerializer(typeof(UintSerializer))]
        public uint Width { get; set; }

        [PropertyName("height")]
        [PropertyDescription("Height of the texture")]
        [PropertySerializer(typeof(UintSerializer))]
        public uint Height { get; set; }

        [PropertyName("format")]
        [PropertyDescription("Source texture format type")]
        [PropertySerializer(typeof(EnumSerializer<ETextureRawFormat>))]
        public ETextureRawFormat Format { get; set; }

        [PropertyName("compression")]
        [PropertyDescription("Compression method to use")]
        [PropertySerializer(typeof(EnumSerializer<ETextureCompression>))]
        public ETextureCompression Compression { get; set; }

        [PropertyName("textureGroup")]
        [PropertySerializer(typeof(CNameSerializer))]
        public string TextureGroup { get; set; }

        [PropertyName("preserveArtistData")]
        [PropertySerializer(typeof(BoolSerializer))]
        public bool PreserveArtistData { get; set; }

        [PropertyName("importFile")]
        [PropertySerializer(typeof(StringSerializer))]
        public string ImportFile { get; set; }

        public uint Unknown0
        {
            get { return this._Unknown0; }
            set { this._Unknown0 = value; }
        }
        public List<Mipmap> Mipmaps { get; set; }

        private uint _Unknown0;

        public CBitmapTexture()
        {
            this.Width = 0;
            this.Height = 0;
            this.PreserveArtistData = false;
            this.ImportFile = "";
            this.Format = ETextureRawFormat.TRF_TrueColor;
            this.Compression = ETextureCompression.TCM_None;

            this.Unknown0 = 0;
            this.Mipmaps = new List<Mipmap>();
        }

        public override void Serialize(IFileStream stream)
        {
            base.Serialize(stream);

            stream.SerializeValue(ref this._Unknown0);
            if (this.Unknown0 != 0)
            {
                throw new FormatException();
            }

            if (stream.Mode == SerializeMode.Reading)
            {
                this.Mipmaps.Clear();

                uint mipmapCount = 0;
                stream.SerializeValue(ref mipmapCount);

                for (uint i = 0; i < mipmapCount; i++)
                {
                    var mip = new Mipmap();
                    stream.SerializeValue(ref mip.Width);
                    stream.SerializeValue(ref mip.Height);
                    stream.SerializeValue(ref mip.Unknown2);

                    uint size = 0;
                    stream.SerializeValue(ref size);
                    stream.SerializeValue(ref mip.Data, size);

                    this.Mipmaps.Add(mip);
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            uint unknown1 = 0;
            stream.SerializeValue(ref unknown1);
            if (unknown1 != 0)
            {
                throw new FormatException();
            }

            byte unknown2 = 0;
            stream.SerializeValue(ref unknown2);
            if (unknown2 != 0)
            {
                throw new FormatException();
            }
        }

        public class Mipmap
        {
            public uint Width;
            public uint Height;
            public uint Unknown2;
            public byte[] Data;
        }
    }
}

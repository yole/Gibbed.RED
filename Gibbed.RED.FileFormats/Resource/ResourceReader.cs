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
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats.Resource
{
    internal class ResourceReader : IFileStream, IDisposable
    {
        private MemoryStream Stream;
        private Info Info;
        private bool _Disposed = false;

        public ResourceReader(
            Info info,
            byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.Info = info;
            this.Stream = new MemoryStream((byte[])data.Clone());
        }

        ~ResourceReader()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._Disposed == false)
            {
                if (disposing == true)
                {
                    this.Stream.Dispose();
                }

                this._Disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IFileStream
        SerializeMode IFileStream.Mode
        {
            get { return SerializeMode.Reading; }
        }

        public long Position
        {
            get
            {
                return this.Stream.Position;
            }
            set
            {
                this.Stream.Position = value;
            }
        }

        public long Length
        {
            get
            {
                return this.Stream.Length;
            }
        }

        void IFileStream.SerializeValue(ref bool value)
        {
            value = this.Stream.ReadValueB8();
        }

        void IFileStream.SerializeValue(ref sbyte value)
        {
            value = this.Stream.ReadValueS8();
        }

        void IFileStream.SerializeValue(ref byte value)
        {
            value = this.Stream.ReadValueU8();
        }

        void IFileStream.SerializeValue(ref short value)
        {
            value = this.Stream.ReadValueS16();
        }

        void IFileStream.SerializeValue(ref ushort value)
        {
            value = this.Stream.ReadValueU16();
        }

        void IFileStream.SerializeValue(ref int value)
        {
            value = this.Stream.ReadValueS32();
        }

        void IFileStream.SerializeValue(ref uint value)
        {
            value = this.Stream.ReadValueU32();
        }

        void IFileStream.SerializeValue(ref float value)
        {
            value = this.Stream.ReadValueF32();
        }

        void IFileStream.SerializeValue(ref string value)
        {
            value = this.Stream.ReadEncodedString();
        }

        void IFileStream.SerializeValue(ref Guid value)
        {
            value = this.Stream.ReadValueGuid();
        }

        void IFileStream.SerializeValue(ref byte[] value, int length)
        {
            value = new byte[length];
            if (this.Stream.Read(value, 0, value.Length) != value.Length)
            {
                throw new FormatException();
            }
        }

        void IFileStream.SerializeValue(ref byte[] value, uint length)
        {
            ((IFileStream)this).SerializeValue(ref value, (int)length);
        }

        void IFileStream.SerializeBuffer(ref byte[] value)
        {
            var length = this.Stream.ReadValueEncodedS32();
            var buffer = new byte[length];
            this.Stream.Read(buffer, 0, buffer.Length);
            value = buffer;
        }

        void IFileStream.SerializeName(ref string value)
        {
            var index = this.Stream.ReadValueS16();

            if (index == 0)
            {
                value = null;
                return;
            }
            else if (index > this.Info.Names.Length)
            {
                throw new FormatException();
            }

            value = this.Info.Names[index - 1];
        }

        void IFileStream.SerializeTagList(ref List<string> value)
        {
            var count = this.Stream.ReadValueEncodedS32();

            var list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string item = null;
                ((IFileStream)this).SerializeName(ref item);
                list.Add(item);
            }
            value = list;
        }

        void IFileStream.SerializeObject<TType>(ref TType value)
        {
            var instance = new TType();
            instance.Serialize(this);
            value = instance;
        }

        void IFileStream.SerializePointer(ref IFileObject value)
        {
            var index = this.Stream.ReadValueS32();

            if (index > 0)
            {
                index--;

                if (index >= this.Info.Objects.Length)
                {
                    throw new FormatException();
                }

                var obj = this.Info.Objects[index];
                value = obj.Data;
            }
            else if (index == 0)
            {
                value = null;
            }
            else /*if (value < 0)*/
            {
                index = -index;
                index--;

                if (index >= this.Info.Links.Length)
                {
                    throw new FormatException();
                }

                var link = new Link()
                {
                    Info = this.Info.Links[index],
                };

                value = link;
            }
        }

        void IFileStream.SerializePointer(ref List<IFileObject> value, bool encoded)
        {
            var count = encoded == false ?
                this.Stream.ReadValueS32() :
                this.Stream.ReadValueEncodedS32();
            var list = new List<IFileObject>();
            for (int i = 0; i < count; i++)
            {
                IFileObject item = null;
                ((IFileStream)this).SerializePointer(ref item);
                list.Add(item);
            }
            value = list;
        }

        #endregion
    }
}

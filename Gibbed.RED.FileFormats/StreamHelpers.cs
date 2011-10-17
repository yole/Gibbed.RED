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
using System.IO;
using System.Text;
using Gibbed.Helpers;

namespace Gibbed.RED.FileFormats
{
    internal static class StreamHelpers
    {
        private static Encoding WindowsEncoding = Encoding.GetEncoding(1252);

        public static int ReadValueEncodedS32(this Stream stream)
        {
            var op = stream.ReadValueU8();

            uint value = (byte)(op & 0x3F);

            if ((op & 0x40) != 0)
            {
                int shift = 6;
                byte extra;
                do
                {
                    if (shift > 27)
                    {
                        throw new InvalidOperationException();
                    }

                    extra = stream.ReadValueU8();
                    value |= (uint)(extra & 0x7F) << shift;
                    shift += 7;
                }
                while ((extra & 0x80) != 0);
            }

            if ((op & 0x80) != 0)
            {
                return -(int)value;
            }

            return (int)value;
        }

        public static void WriteValueEncodedS32(this Stream stream, int value)
        {
            byte op = 0;

            if (value <= 0)
            {
                op |= 0x80;
                value = -value;
            }

            op |= (byte)(value & 0x3F);
            value >>= 6;

            if (value > 0)
            {
                op |= 0x40;
            }

            stream.WriteValueU8(op);

            if (value > 0)
            {
                do
                {
                    byte extra = (byte)(value & 0x7F);
                    value >>= 7;
                    if (value > 0)
                    {
                        extra |= 0x80;
                    }
                    stream.WriteValueU8(extra);
                }
                while (value > 0);
            }
        }

        public static string ReadEncodedStringW(this Stream stream)
        {
            var length = stream.ReadValueEncodedS32();

            if (length < 0 || length >= 0x10000)
            {
                throw new InvalidOperationException();
            }

            return stream.ReadString(length, true, WindowsEncoding);
        }

        public static string ReadEncodedString(this Stream stream)
        {
            var length = stream.ReadValueEncodedS32();

            if (length < 0)
            {
                length = -length;
                if (length >= 0x10000)
                {
                    throw new InvalidOperationException();
                }

                return stream.ReadString(length, true, WindowsEncoding);
            }
            else
            {
                if (length >= 0x10000)
                {
                    throw new InvalidOperationException();
                }

                return stream.ReadString(length * 2, true, Encoding.Unicode);
            }
        }

        public static void WriteEncodedString(this Stream stream, string value)
        {
            bool isUnicode = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] > 0xFF && value[i] != '…')
                {
                    isUnicode = true;
                    break;
                }
            }

            if (isUnicode == false)
            {
                value = value.Replace("…", "...");

                stream.WriteValueEncodedS32(-value.Length);
                stream.WriteString(value, WindowsEncoding);
            }
            else
            {
                stream.WriteValueEncodedS32(value.Length);
                stream.WriteString(value, Encoding.Unicode);
            }
        }

        public static byte[] ReadEncodedStringBuffer(this Stream stream)
        {
            var length = stream.ReadValueEncodedS32();

            if (length < 0)
            {
                throw new NotImplementedException();

                length = -length;
                if (length >= 0x10000)
                {
                    throw new InvalidOperationException();
                }

                // ASCII
                var buffer = new byte[length * 2];
                var ascii = new byte[length];
                if (stream.Read(ascii, 0, ascii.Length) != ascii.Length)
                {
                    throw new InvalidOperationException();
                }

                for (int i = 0; i < length; i++)
                {
                    buffer[i * 2] = ascii[i];
                }

                return buffer;
            }
            else
            {
                if (length >= 0x10000)
                {
                    throw new InvalidOperationException();
                }

                var buffer = new byte[length * 2];
                if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    throw new InvalidOperationException();
                }

                return buffer;
            }
        }

        public static void WriteEncodedStringBuffer(this Stream stream, byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                stream.WriteValueEncodedS32(0);
                return;
            }

            if ((value.Length % 2) == 1)
            {
                throw new InvalidOperationException();
            }

            stream.WriteValueEncodedS32(value.Length / 2);
            stream.Write(value, 0, value.Length);
        }
    }
}

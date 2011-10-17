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

namespace Gibbed.RED.Unpack
{
    internal class LZF
    {
        public static int Decompress(byte[] input, byte[] output)
        {
            int i = 0;
            int o = 0;

            int inputLength = input.Length;
            int outputLength = output.Length;

            while (i < inputLength)
            {
                uint control = input[i++];

                if (control < (1 << 5))
                {
                    int length = (int)(control + 1);

                    if (o + length > outputLength)
                    {
                        throw new InvalidOperationException();
                    }

                    Array.Copy(input, i, output, o, length);
                    i += length;
                    o += length;
                }
                else
                {
                    int length = (int)(control >> 5);
                    int offset = (int)((control & 0x1F) << 8);

                    if (length == 7)
                    {
                        length += input[i++];
                    }
                    length += 2;

                    offset |= input[i++];

                    if (o + length > outputLength)
                    {
                        throw new InvalidOperationException();
                    }

                    offset = o - 1 - offset;
                    if (offset < 0)
                    {
                        throw new InvalidOperationException();
                    }

                    int block = Math.Min(length, o - offset);
                    Array.Copy(output, offset, output, o, block);
                    o += block;
                    offset += block;
                    length -= block;

                    while (length > 0)
                    {
                        output[o++] = output[offset++];
                        length--;
                    }
                }
            }

            return o;
        }
    }
}

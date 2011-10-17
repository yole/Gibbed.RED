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
using Gibbed.RED.FileFormats;
using NDesk.Options;

namespace Gibbed.RED.Unpack
{
    internal class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool showHelp = false;
            bool overwriteFiles = false;
            string cdkey = null;
            string extensionFilter = null;

            OptionSet options = new OptionSet()
            {
                {
                    "o|overwrite",
                    "overwrite existing files",
                    v => overwriteFiles = v != null
                },
                {
                    "cdkey=",
                    "cdkey for use with DLC archives\n(in format #####-#####-#####-#####)",
                    v => cdkey = v
                },
                {
                    "e|extension=",
                    "only extract files of this extension",
                    v => extensionFilter = v
                },
                {
                    "h|help",
                    "show this message and exit", 
                    v => showHelp = v != null
                },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extras.Count < 1 || extras.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_dzip [output_dir]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string inputPath = extras[0];
            string outputPath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(inputPath, null);

            var uncompressed = new byte[0x10000];
            bool filtering = string.IsNullOrEmpty(extensionFilter) == false;

            using (var input = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var pkg = new PackageFile();
                pkg.DeserializeWithCDKey(input, cdkey);

                long current = 0;
                long total = pkg.Entries.Count;

                foreach (var entry in pkg.Entries)
                {
                    current++;
                    var entryPath = Path.Combine(outputPath, entry.Name);

                    if (overwriteFiles == false &&
                        File.Exists(entryPath) == true)
                    {
                        continue;
                    }

                    if (filtering == true &&
                        Path.GetExtension(entryPath) != extensionFilter)
                    {
                        continue;
                    }

                    Console.WriteLine("[{0}/{1}] {2}",
                        current, total, entry.Name);

                    Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                    input.Seek(entry.Offset, SeekOrigin.Begin);

                    int blocks = (int)((entry.UncompressedSize + 0xFFFF) >> 16); // .Align(0x10000) / 0x10000;

                    var offsets = new long[blocks + 1];
                    for (int i = 0; i < offsets.Length; i++)
                    {
                        offsets[i] = entry.Offset + input.ReadValueU32();
                    }
                    offsets[blocks] = entry.Offset + entry.CompressedSize;

                    using (var output = File.Create(entryPath))
                    {
                        long left = entry.UncompressedSize;

                        for (int i = 0; i < blocks; i++)
                        {
                            var compressed = new byte[offsets[i + 1] - offsets[i + 0]];
                            input.Seek(offsets[i], SeekOrigin.Begin);
                            input.Read(compressed, 0, compressed.Length);

                            int read = LZF.Decompress(compressed, uncompressed);

                            if (i + 1 < blocks && read != uncompressed.Length)
                            {
                                throw new InvalidOperationException();
                            }

                            output.Write(uncompressed, 0, (int)Math.Min(left, read));
                            left -= read;
                        }
                    }

                    File.SetLastWriteTime(entryPath, entry.TimeStamp);
                }
            }
        }
    }
}

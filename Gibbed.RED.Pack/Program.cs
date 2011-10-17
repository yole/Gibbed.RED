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
using System.Linq;
using Gibbed.RED.FileFormats;
using Package = Gibbed.RED.FileFormats.Package;
using NDesk.Options;

namespace Gibbed.RED.Pack
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
            bool verbose = false;

            OptionSet options = new OptionSet()
            {
                {
                    "v|verbose",
                    "be verbose",
                    v => verbose = v != null
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
                Console.WriteLine("Usage: {0} [OPTIONS]+ [output_dzip] input_dir+", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var inputPaths = new List<string>();
            string outputPath;

            if (extras.Count == 1)
            {
                inputPaths.Add(extras[0]);
                outputPath = Path.ChangeExtension(extras[0], ".dzip");
            }
            else
            {
                outputPath = extras[0];
                inputPaths.AddRange(extras.Skip(1));
            }

            var paths = new SortedDictionary<string, string>();

            if (verbose == true)
            {
                Console.WriteLine("Finding files...");
            }

            foreach (var relPath in inputPaths)
            {
                string inputPath = Path.GetFullPath(relPath);

                if (inputPath.EndsWith(Path.DirectorySeparatorChar.ToString()) == true)
                {
                    inputPath = inputPath.Substring(0, inputPath.Length - 1);
                }

                foreach (string path in Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories))
                {
                    bool hasName;
                    string fullPath = Path.GetFullPath(path);
                    string partPath = fullPath.Substring(inputPath.Length + 1).ToLowerInvariant();

                    if (paths.ContainsKey(partPath) == true)
                    {
                        Console.WriteLine("Ignoring {0} duplicate.", partPath);
                        continue;
                    }

                    paths[partPath] = fullPath;
                }
            }

            using (var output = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var pkg = new PackageFile();

                output.Seek(PackageFile.HeaderSize, SeekOrigin.Begin);

                if (verbose == true)
                {
                    Console.WriteLine("Writing files to package...");
                }

                foreach (var kvp in paths)
                {
                    if (verbose == true)
                    {
                        Console.WriteLine(kvp.Key);
                    }

                    using (var input = File.OpenRead(kvp.Value))
                    {
                        var entry = new Package.Entry();
                        entry.Name = kvp.Key;
                        entry.TimeStamp = File.GetLastWriteTime(kvp.Value);
                        entry.UncompressedSize = input.Length;
                        entry.Offset = output.Position;

                        int blockCount = (int)((entry.UncompressedSize + 0xFFFF) >> 16); // .Align(0x10000) / 0x10000;
                        var compressedBlocks = new byte[blockCount][];
                        var compressedSizes = new int[blockCount];
                        var uncompressedBlock = new byte[0x10000];

                        long left = input.Length;
                        for (int i = 0; i < blockCount; i++)
                        {
                            int uncompressedSize = input.Read(
                                uncompressedBlock, 0,
                                uncompressedBlock.Length);
                            
                            var lzf = new LZF();
                            compressedBlocks[i] = new byte[0x20000];
                            int compressedSize = lzf.Compress(
                                uncompressedBlock,
                                uncompressedSize,
                                compressedBlocks[i],
                                compressedBlocks[i].Length);
                            if (compressedSize == 0)
                            {
                                throw new InvalidOperationException();
                            }
                            compressedSizes[i] = compressedSize;
                        }

                        int offset = blockCount * 4;
                        for (int i = 0; i < blockCount; i++)
                        {
                            output.WriteValueS32(offset);
                            offset += compressedSizes[i];
                        }

                        for (int i = 0; i < blockCount; i++)
                        {
                            output.Write(compressedBlocks[i], 0, compressedSizes[i]);
                        }

                        entry.CompressedSize = offset;
                        pkg.Entries.Add(entry);
                    }
                }

                if (verbose == true)
                {
                    Console.WriteLine("Writing package header...");
                }

                pkg.Serialize(output, output.Length);
            }
        }
    }
}

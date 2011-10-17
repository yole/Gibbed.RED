using System;
using System.Runtime.InteropServices;

namespace Gibbed.Squish
{
	public sealed class Native
	{
		public enum SquishFlags
		{
			DXT1						= (1 << 0),		// Use DXT1 compression.
			DXT3						= (1 << 1),		// Use DXT3 compression.
			DXT5						= (1 << 2), 	// Use DXT5 compression.
		
			ColourClusterFit			= (1 << 3),		// Use a slow but high quality colour compressor (the default).
			ColourRangeFit				= (1 << 4),		// Use a fast but low quality colour compressor.

			ColourMetricPerceptual		= (1 << 5),		// Use a perceptual metric for colour error (the default).
			ColourMetricUniform		    = (1 << 6),		// Use a uniform metric for colour error.
	
			WeightColourByAlpha		    = (1 << 7),		// Weight the colour by alpha during cluster fit (disabled by default).

		    ColourIterativeClusterFit	= (1 << 8),		// Use a very slow but very high quality colour compressor.
		}

		private	static bool	Is64Bit()
		{
			return Marshal.SizeOf(IntPtr.Zero) == 8 ; 
		}

		private sealed class Native32
		{
			[DllImport("squish_32.dll", EntryPoint = "SquishCompressImage")]
            internal static extern void CompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba, int width, int height, [MarshalAs(UnmanagedType.LPArray)] byte[] blocks, int flags);

            [DllImport("squish_32.dll", EntryPoint = "SquishDecompressImage")]
            internal static extern void DecompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba, int width, int height, [MarshalAs(UnmanagedType.LPArray)] byte[] blocks, int flags);
		}
   
		private sealed class Native64
		{
            [DllImport("squish_64.dll", EntryPoint = "SquishCompressImage")]
            internal static extern void CompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba, int width, int height, [MarshalAs(UnmanagedType.LPArray)] byte[] blocks, int flags);

            [DllImport("squish_64.dll", EntryPoint = "SquishDecompressImage")]
            internal static extern void DecompressImage([MarshalAs(UnmanagedType.LPArray)] byte[] rgba, int width, int height, [MarshalAs(UnmanagedType.LPArray)] byte[] blocks, int flags);
		}
	
		private static void CallDecompressImage(byte[] rgba, int width, int height, byte[] blocks, int flags)
		{
            if (Is64Bit())
            {
                Native64.DecompressImage(rgba, width, height, blocks, flags);
            }
            else
            {
                Native32.DecompressImage(rgba, width, height, blocks, flags);
			}
		}

		public static byte[] DecompressImage(byte[] blocks, int width, int height, int flags)
		{
			byte[]pixelOutput = new byte[width * height * 4];
			CallDecompressImage(pixelOutput, width, height, blocks, flags);
			return pixelOutput;
		}
	}
}

using System;
using System.IO.Compression;
using System.Text;

namespace Nano.VFS
{
    /// <summary>
    /// Virtual File System entry wrapper for the ZIP file format.
    /// Uses the System.IO.Compression NuGet package.
    /// </summary>
    public class ZipFileEntry : VirtualFileEntry
    {
        public ZipArchiveEntry ArchiveEntry { get; set; }

        public override byte[] Read()
        {
            var output = new byte[ArchiveEntry.Length];

            using (var outputStream = ArchiveEntry.Open())
            {
                outputStream.Read(output, 0, output.Length);
            }

            return output;
        }

        public override void Read(byte[] output, int offset, int length)
        {
            if (length - offset < ArchiveEntry.Length)
                throw new ArgumentException($"The buffer is too small to read the entry! {length - offset} given, {ArchiveEntry.Length} needed.");

            using (var outputStream = ArchiveEntry.Open())
            {
                outputStream.Read(output, 0, output.Length);
            }
        }

        public override string ReadString()
        {
            return Encoding.UTF8.GetString(Read());
        }
    }
}

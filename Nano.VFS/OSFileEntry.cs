using System.IO;

namespace Nano.VFS
{
    /// <summary>
    /// Virtual File System entry wrapper for operating system files
    /// </summary>
    public class OSFileEntry : VirtualFileEntry
    {
        /// <summary>
        /// The actual file path of the entry
        /// </summary>
        public string FilePath { get; set; }

        public override byte[] Read()
        {
            return File.ReadAllBytes(FilePath);
        }

        public override void Read(byte[] output, int offset, int length)
        {
            using (var fs = new FileStream(FilePath, FileMode.Open))
            {
                fs.Read(output, offset, length);
            }
        }

        public override string ReadString()
        {
            return File.ReadAllText(FilePath);
        }
    }
}

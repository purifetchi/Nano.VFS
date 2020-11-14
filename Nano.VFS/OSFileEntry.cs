using System.IO;
using System.Text;

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

        public override void Write(byte[] buffer, WriteMode fileMode)
        {
            switch (fileMode)
            {
                case WriteMode.Overwrite:
                    File.WriteAllBytes(FilePath, buffer);
                    break;

                case WriteMode.Append:
                    // To append bytes, we need to open a file stream, since System.IO.File doesn't
                    // have an AppendAllBytes function.
                    using (var fs = new FileStream(FilePath, FileMode.Append))
                    {
                        fs.Write(buffer, 0, buffer.Length);
                    }
                    break;
            }
        }

        public override void Write(string value, WriteMode fileMode)
        {
            Write(Encoding.UTF8.GetBytes(value), fileMode);
        }
    }
}

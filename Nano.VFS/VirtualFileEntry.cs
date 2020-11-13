using System.IO;

namespace Nano.VFS
{
    public abstract class VirtualFileEntry
    {
        /// <summary>
        /// The entry type
        /// </summary>
        public virtual VirtualEntryType Type { get; set; }

        /// <summary>
        /// Reference to the VFS it's a part of
        /// </summary>
        internal VirtualFileSystem VFS { get; set; }

        /// <summary>
        /// The filename + extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Which entry does this entry belong to
        /// </summary>
        public uint BelongsTo { get; set; }

        /// <summary>
        /// Reads the entry into a new byte array
        /// </summary>
        /// <returns>The written entry as a byte array</returns>
        public abstract byte[] Read();

        /// <summary>
        /// Reads the current entry into a buffer
        /// </summary>
        /// <param name="output">The buffer to write to</param>
        /// <param name="offset">Offset at which the entry should be written</param>
        /// <param name="length">The max length of the buffer</param>
        public abstract void Read(byte[] output, int offset, int length);

        /// <summary>
        /// Reads the current entry as a string
        /// </summary>
        /// <returns>The content of the entry as a string</returns>
        public abstract string ReadString();

        /// <summary>
        /// Gets the filename of the current Virtual File Entry
        /// </summary>
        /// <returns>The filename + extension if applicable</returns>
        public virtual string GetName() => Name;

        /// <summary>
        /// Traverses the VFS backwards creating the full path of the current entry
        /// </summary>
        /// <returns>The full path of the current VFS entry</returns>
        public virtual string GetPath()
        {
            // Current level we're at and the current path
            var level = BelongsTo;
            var path = Name;

            // Iterate the folder structure backwards, as long as the BelongsTo field exists
            // Which means the directory will be made like this: current - 2 + current - 1 + current
            while (VFS.entries.ContainsKey(level))
            {
                path = Path.Combine(VFS.entries[level].Name, path);
                level = VFS.entries[level].BelongsTo;
            }

            // Combine this with the root
            return Path.Combine("\\", path);
        }
    }
}

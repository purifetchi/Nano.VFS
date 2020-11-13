namespace Nano.VFS
{
    /// <summary>
    /// User-facing wrapper for VFS files.
    /// </summary>
    public struct VFSEntry
    {
        internal uint entryHash;
        internal VirtualFileSystem vfs;

        /// <summary>
        /// The type of the entry
        /// </summary>
        public VirtualEntryType Type { get; internal set; }

        /// <summary>
        /// The filename of the current entry
        /// </summary>
        public string Name => vfs.entries[entryHash].GetName();

        /// <summary>
        /// The full path of the current entry
        /// </summary>
        public string Path => vfs.entries[entryHash].GetPath();

        /// <summary>
        /// Reads the given entry as a byte array
        /// </summary>
        /// <param name="relativePath">The path of the entry</param>
        /// <returns>The read array</returns>
        public byte[] Read()
        {
            return vfs.entries[entryHash].Read();
        }

        /// <summary>
        /// Reads the given entry into a buffer
        /// </summary>
        /// <param name="relativePath">The path of the entry</param>
        /// <param name="buffer">The buffer to write to</param>
        /// <param name="offset">The offset at which the entry should be written</param>
        /// <param name="length">The length of the buffer</param>
        public void Read(byte[] buffer, int offset, int length)
        {
            vfs.entries[entryHash].Read(buffer, offset, length);
        }

        /// <summary>
        /// Reads the given entry as a string
        /// </summary>
        /// <param name="relativePath">The path of the entry</param>
        /// <returns>The read string</returns>
        public string ReadString()
        {
            return vfs.entries[entryHash].ReadString();
        }
    }
}

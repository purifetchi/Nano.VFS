using System;
using System.Collections.Generic;

namespace Nano.VFS
{
    /// <summary>
    /// In-memory directory representation
    /// </summary>
    public class DirectoryFileEntry : VirtualFileEntry
    {
        /// <summary>
        /// List of the entries this directory links to.
        /// </summary>
        public List<uint> Links { get; set; }

        // We don't support reading a directory

        public override byte[] Read() => throw new NotSupportedException();

        public override void Read(byte[] output, int offset, int length) => throw new NotSupportedException();

        public override string ReadString() => throw new NotSupportedException();

        /// <summary>
        /// Gets the enumerator for all the files in this directory
        /// </summary>
        /// <returns>VFSEntry Enumerator</returns>
        public virtual IEnumerable<VFSEntry> GetFileEnumerator()
        {
            foreach (var file in Links)
            {
                yield return new VFSEntry
                {
                    Type = VFS.entries[file].Type,
                    entryHash = file,
                    vfs = VFS
                };
            }
        }
    }
}

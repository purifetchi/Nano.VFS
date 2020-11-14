using System;
using System.Collections.Generic;
using System.IO;

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

        // Neither do we support writing to it.

        public override void Write(byte[] buffer, WriteMode fileMode) => throw new NotImplementedException();

        public override void Write(string value, WriteMode fileMode) => throw new NotImplementedException();

        /// <summary>
        /// Gets the enumerator for all the files in this directory
        /// </summary>
        /// <returns>VFSEntry Enumerator</returns>
        public virtual IEnumerable<VfsEntry> GetFileEnumerator()
        {
            foreach (var file in Links)
            {
                yield return new VfsEntry
                {
                    Type = VFS.entries[file].Type,
                    entryHash = file,
                    vfs = VFS
                };
            }
        }
    }
}

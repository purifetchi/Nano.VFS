using System.IO;
using System.IO.Compression;

namespace Nano.VFS
{
    public static class VFSExtensions
    {
        /// <summary>
        /// Adds a path in the OS as an OSFileEntry into the VFS
        /// </summary>
        /// <param name="filePath">The file path of the physical file</param>
        /// <param name="relativePath">The relative path it should be mounted as</param>
        public static void Add(this VirtualFileSystem VFS, string filePath, string relativePath)
        {
            var entry = new OSFileEntry
            {
                Type = VirtualEntryType.File,
                FilePath = filePath
            };

            VFS.Add(entry, relativePath);
        }

        /// <summary>
        /// Adds a zip archive entry as a ZipFileEntry into the VFS
        /// </summary>
        /// <param name="archiveEntry">The ZipArchiveEntry object</param>
        public static void Add(this VirtualFileSystem VFS, ZipArchiveEntry archiveEntry)
        {
            // If ArchiveEntry.Name is empty, it means the entry is a directory.
            // Those will get automatically created.
            if (string.IsNullOrEmpty(archiveEntry.Name))
                return;

            var entry = new ZipFileEntry
            {
                Type = VirtualEntryType.File,
                ArchiveEntry = archiveEntry
            };

            // ArchiveEntry.FullName doesn't include the root. Add it.
            VFS.Add(entry, Path.Combine("\\", archiveEntry.FullName));
        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace Nano.VFS
{
    /// <summary>
    /// In-memory virtual file system that allows for modular entries
    /// </summary>
    public class VirtualFileSystem
    {
        /// <summary>
        /// The VFS entry container. Maps from relative path hashes to VirtualFileEntries
        /// </summary>
        internal readonly Dictionary<uint, VirtualFileEntry> entries = new Dictionary<uint, VirtualFileEntry>();

        /// <summary>
        /// Gets the SDBM hash of a given string
        /// </summary>
        /// <param name="val">The string to hash</param>
        /// <returns>The hash of the string</returns>
        private static uint GetEntryHash(string val)
        {
            var hash = 0u;
            for (var i = 0; i < val.Length; i++)
                hash = val[i] + (hash << 6) + (hash << 16) - hash;

            return hash;
        }

        /// <summary>
        /// Adds an arbitrary entry object at the given relative path
        /// </summary>
        /// <param name="entry">The VirtualFileEntry to be added</param>
        /// <param name="relativePath">The relative path of the entry</param>
        public void Add(VirtualFileEntry entry, string relativePath)
        {
            // Get the top level folder of the current file
            // and try to traverse backwards to generate the folder structure
            var path = Path.GetDirectoryName(relativePath);
            CreateFolderStructure(path);

            // Attach the filename to the current entry from the relative file path
            // Also link it to the current VFS
            entry.Name = Path.GetFileName(relativePath);
            entry.VFS = this;

            // Add the entry and link it to the top-level directory
            entries[GetEntryHash(relativePath)] = entry;
            LinkEntryAndDirectory(relativePath, path);
        }

        /// <summary>
        /// Creates an entire folder structure, traversing the top-level directory backwards
        /// </summary>
        /// <param name="relativePath">The top level directory</param>
        /// <returns>Whether the folder one level bottom was created successfully</returns>
        public bool CreateFolderStructure(string relativePath)
        {
            if (IsValidEntryName(relativePath) && !DirectoryExists(relativePath))
            {
                // Create the directory that's at the current level
                CreateDirectory(relativePath);

                // Get the directory one level below the CWD
                var oneLower = Path.GetDirectoryName(relativePath);

                // If the lower level directory already exists, just link it.
                if (IsValidEntryName(oneLower) && DirectoryExists(oneLower))
                    LinkEntryAndDirectory(relativePath, oneLower);

                // Else, if we manage to create the directory that's at the lower level
                // also link it.
                else if (CreateFolderStructure(oneLower))
                    LinkEntryAndDirectory(relativePath, oneLower);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new directory
        /// </summary>
        /// <param name="relativePath">The relative path of the directory</param>
        public void CreateDirectory(string relativePath)
        {
            entries[GetEntryHash(relativePath)] = new DirectoryFileEntry
            {
                Type = VirtualEntryType.Directory,
                Links = new List<uint>(),
                Name = Path.GetFileName(relativePath),
                VFS = this
            };
        }

        /// <summary>
        /// Checks if a file exists
        /// </summary>
        /// <param name="relativePath">The relative path of the file</param>
        /// <returns>Whether that file exists</returns>
        public bool FileExists(string relativePath)
        {
            return entries.ContainsKey(GetEntryHash(relativePath)) &&
                   entries[GetEntryHash(relativePath)].Type == VirtualEntryType.File;
        }

        /// <summary>
        /// Checks if a directory exists
        /// </summary>
        /// <param name="relativePath">The relative path of the directory</param>
        /// <returns>Whether that directory exists</returns>
        public bool DirectoryExists(string relativePath)
        {
            return entries.ContainsKey(GetEntryHash(relativePath)) && 
                   entries[GetEntryHash(relativePath)].Type == VirtualEntryType.Directory;
        }

        /// <summary>
        /// Is the entry name valid?
        /// </summary>
        /// <param name="relativePath">The path of the entry</param>
        /// <returns>Whether that entry name is valid</returns>
        public bool IsValidEntryName(string relativePath)
        {
            return !string.IsNullOrEmpty(relativePath);
        }

        /// <summary>
        /// Links a given entry with the parent directory
        /// </summary>
        /// <param name="entryPath">The path of the entry to be linked</param>
        /// <param name="directoryPath">The directory to link to.</param>
        private void LinkEntryAndDirectory(string entryPath, string directoryPath)
        {
            // Get the hashes of both the directory and the entry we're linking
            var directoryHash = GetEntryHash(directoryPath);
            var entryHash = GetEntryHash(entryPath);

            // Cast the directory to a DirectoryFileEntry, and add the linked entry into the links List
            var dir = entries[directoryHash] as DirectoryFileEntry;
            dir.Links.Add(entryHash);

            // Set the entry's BelongTo value to the directory.
            var entry = entries[entryHash];
            entry.BelongsTo = directoryHash;
        }

        /// <summary>
        /// Reads the given entry as a byte array
        /// </summary>
        /// <param name="relativePath">The path of the entry</param>
        /// <returns>The read array</returns>
        public byte[] ReadFile(string relativePath)
        {
            if (!FileExists(relativePath))
                throw new FileNotFoundException();

            return entries[GetEntryHash(relativePath)].Read();
        }

        /// <summary>
        /// Reads the given entry into a buffer
        /// </summary>
        /// <param name="relativePath">The path of the entry</param>
        /// <param name="buffer">The buffer to write to</param>
        /// <param name="offset">The offset at which the entry should be written</param>
        /// <param name="length">The length of the buffer</param>
        public void ReadFile(string relativePath, byte[] buffer, int offset, int length)
        {
            if (!FileExists(relativePath))
                throw new FileNotFoundException();

            entries[GetEntryHash(relativePath)].Read(buffer, offset, length);
        }

        /// <summary>
        /// Reads the given entry as a string
        /// </summary>
        /// <param name="relativePath">The path of the entry</param>
        /// <returns>The read string</returns>
        public string ReadFileAsString(string relativePath)
        {
            if (!FileExists(relativePath))
                throw new FileNotFoundException();

            return entries[GetEntryHash(relativePath)].ReadString();
        }

        /// <summary>
        /// Returns an enumerator of every single entry in the VFS
        /// </summary>
        /// <returns>VFSEntry Enumerator</returns>
        public IEnumerable<VFSEntry> GetAllEntries()
        {
            foreach (var file in entries)
                yield return new VFSEntry
                {
                    Type = file.Value.Type,
                    entryHash = file.Key,
                    vfs = this
                };
        }

        /// <summary>
        /// Returns an enumerator of all entries in a given directory
        /// </summary>
        /// <param name="path">The path of the directory</param>
        /// <returns>VFSEntry Enumerator</returns>
        public IEnumerable<VFSEntry> GetEntriesInDirectory(string path)
        {
            if (!DirectoryExists(path))
                throw new DirectoryNotFoundException();

            var directory = entries[GetEntryHash(path)] as DirectoryFileEntry;
            return directory.GetFileEnumerator();
        }
    }
}

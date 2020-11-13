# Nano.VFS
Nano.VFS is a simple and modular implementation of an in-memory virtual file system. It allows you to create custom entry wrappers that mount into a centralized VFS class, meaning that you can mix different storage formats.

# Provided VFS entry backends

- OSFileEntry (Can mount directories and files from the FS)
- ZipFileEntry (Can mount directories and files from ZIP archives)

# Usage

**Mounting a zip archive**
```cs
// Create a new VirtualFileSystem instance
var vfs = new VirtualFileSystem();

// Create a file stream for the zip archive and open it with System.IO.Compression's ZipArchive
var fileStream = new FileStream("archive.zip", FileMode.Open);
var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

// Add every entry from the archive into the VFS
// This already creates the folder structure
foreach (var entry in archive.Entries)
{
	vfs.Add(entry);
}  
```

**Mounting files from the filesystem**
```cs
var path = "C://";

// Create a new VirtualFileSystem instance
var vfs = new VirtualFileSystem();

// Iterate over every single file in the directory, recursively
foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
{
	// Remove the actual root path part from the given path
	var relative = file.Replace(path, "");
	vfs.Add(file, relative);
}
```

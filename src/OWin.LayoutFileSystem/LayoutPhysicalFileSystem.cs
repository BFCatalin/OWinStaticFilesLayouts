using Microsoft.Owin.FileSystems;
using System;
using System.Collections.Generic;
using System.IO;

namespace OWin.LayoutFileSystem
{
    public class LayoutPhysicalFileSystem : IFileSystem
    {
        private class LayoutPhysicalFileInfo : IFileInfo
        {
            private readonly FileInfo _info;

            public long Length
            {
                get
                {
                    return this._info.Length;
                }
            }

            public string PhysicalPath
            {
                get
                {
                    return this._info.FullName;
                }
            }

            public string Name
            {
                get
                {
                    return this._info.Name;
                }
            }

            public DateTime LastModified
            {
                get
                {
                    return this._info.LastWriteTime;
                }
            }

            public bool IsDirectory
            {
                get
                {
                    return false;
                }
            }

            public LayoutPhysicalFileInfo(FileInfo info)
            {
                this._info = info;
            }

            public Stream CreateReadStream()
            {
                return new FileStream(this.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan);
            }
        }

        private class LayoutPhysicalDirectoryInfo : IFileInfo
        {
            private readonly DirectoryInfo _info;

            public long Length
            {
                get
                {
                    return -1L;
                }
            }

            public string PhysicalPath
            {
                get
                {
                    return this._info.FullName;
                }
            }

            public string Name
            {
                get
                {
                    return this._info.Name;
                }
            }

            public DateTime LastModified
            {
                get
                {
                    return this._info.LastWriteTime;
                }
            }

            public bool IsDirectory
            {
                get
                {
                    return true;
                }
            }

            public LayoutPhysicalDirectoryInfo(DirectoryInfo info)
            {
                this._info = info;
            }

            public Stream CreateReadStream()
            {
                return null;
            }
        }

        private static readonly Dictionary<string, string> RestrictedFileNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"con",
				string.Empty
			},
			{
				"prn",
				string.Empty
			},
			{
				"aux",
				string.Empty
			},
			{
				"nul",
				string.Empty
			},
			{
				"com1",
				string.Empty
			},
			{
				"com2",
				string.Empty
			},
			{
				"com3",
				string.Empty
			},
			{
				"com4",
				string.Empty
			},
			{
				"com5",
				string.Empty
			},
			{
				"com6",
				string.Empty
			},
			{
				"com7",
				string.Empty
			},
			{
				"com8",
				string.Empty
			},
			{
				"com9",
				string.Empty
			},
			{
				"lpt1",
				string.Empty
			},
			{
				"lpt2",
				string.Empty
			},
			{
				"lpt3",
				string.Empty
			},
			{
				"lpt4",
				string.Empty
			},
			{
				"lpt5",
				string.Empty
			},
			{
				"lpt6",
				string.Empty
			},
			{
				"lpt7",
				string.Empty
			},
			{
				"lpt8",
				string.Empty
			},
			{
				"lpt9",
				string.Empty
			},
			{
				"clock$",
				string.Empty
			}
		};

        public string Root
        {
            get;
            private set;
        }

        public LayoutPhysicalFileSystem(string root)
        {
            this.Root = LayoutPhysicalFileSystem.GetFullRoot(root);
            if (!Directory.Exists(this.Root))
            {
                throw new DirectoryNotFoundException(this.Root);
            }
        }

        private static string GetFullRoot(string root)
        {
            string applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string text = Path.GetFullPath(Path.Combine(applicationBase, root));
            if (!text.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                text += Path.DirectorySeparatorChar;
            }
            return text;
        }

        private string GetFullPath(string path)
        {
            string fullPath = Path.GetFullPath(Path.Combine(this.Root, path));
            string result;
            if (!fullPath.StartsWith(this.Root, StringComparison.OrdinalIgnoreCase))
            {
                result = null;
            }
            else
            {
                result = fullPath;
            }
            return result;
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            bool result;
            try
            {
                if (subpath.StartsWith("/", StringComparison.Ordinal))
                {
                    subpath = subpath.Substring(1);
                }
                string fullPath = this.GetFullPath(subpath);
                if (fullPath != null)
                {
                    FileInfo fileInfo2 = new FileInfo(fullPath);
                    if (fileInfo2.Exists && !this.IsRestricted(fileInfo2))
                    {
                        string compiled = Path.Combine(this.Root, ".Compiled");
                        LayoutHtmlFile htmlFile = new LayoutHtmlFile(fullPath);
                        fileInfo2 = htmlFile.Compile(compiled);
                        fileInfo = new LayoutPhysicalFileSystem.LayoutPhysicalFileInfo(fileInfo2);
                        result = true;
                        return result;
                    }
                }
            }
            catch (ArgumentException)
            {
            }
            fileInfo = null;
            result = false;
            return result;
        }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            bool result2;
            try
            {
                
                if (subpath.StartsWith("/", StringComparison.Ordinal))
                {
                    subpath = subpath.Substring(1);
                }
                string fullPath = this.GetFullPath(subpath);
                if (fullPath != null)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
                    bool result;
                    if (!directoryInfo.Exists)
                    {
                        contents = null;
                        result = false;
                        result2 = result;
                        return result2;
                    }
                    FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
                    IFileInfo[] array = new IFileInfo[fileSystemInfos.Length];
                    for (int num = 0; num != fileSystemInfos.Length; num++)
                    {
                        FileInfo fileInfo = fileSystemInfos[num] as FileInfo;
                        if (fileInfo != null)
                        {
                            array[num] = new LayoutPhysicalFileSystem.LayoutPhysicalFileInfo(fileInfo);
                        }
                        else
                        {
                            array[num] = new LayoutPhysicalFileSystem.LayoutPhysicalDirectoryInfo((DirectoryInfo)fileSystemInfos[num]);
                        }
                    }
                    contents = array;
                    result = true;
                    result2 = result;
                    return result2;
                }
            }
            catch (ArgumentException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (IOException)
            {
            }
            contents = null;
            result2 = false;
            return result2;
        }

        private bool IsRestricted(FileInfo fileInfo)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
            return LayoutPhysicalFileSystem.RestrictedFileNames.ContainsKey(fileNameWithoutExtension);
        }
    }
}

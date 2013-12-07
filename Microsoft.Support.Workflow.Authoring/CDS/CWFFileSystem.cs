using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.Services;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    public class CWFFileSystem : PhysicalFileSystem, IFileSystem
    {
        public CWFFileSystem(string root):base(root)
        {
        }
      
        public string GetTempPath(string path)
        {
            return Path.Combine(FileService.GetTempDirectoryPath(), path);
        }

        public override void AddFile(string path, Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!CheckFileExtentionIsDll(path))
            {
                return;
            }

            AddFileCore(path, targetStream => stream.CopyTo(targetStream));
        }

        public override void AddFile(string path, Action<Stream> writeToStream)
        {
            if (writeToStream == null)
            {
                throw new ArgumentNullException("writeToStream");
            }

            if (!CheckFileExtentionIsDll(path))
            {
                return;
            }

            AddFileCore(path, writeToStream);
        }

        private bool CheckFileExtentionIsDll(string path)
        {
            return Path.GetExtension(path) == ".dll";
        }

        private void AddFileCore(string path, Action<Stream> writeToStream)
        {
            string tempFullPath = GetTempPath(path);
            EnsureDirectory(Path.GetDirectoryName(tempFullPath));

            using (Stream outputStream = File.Create(tempFullPath))
            {
                writeToStream(outputStream);
            }

            AssemblyName assemblyName = AssemblyName.GetAssemblyName(tempFullPath);
            ActivityAssemblyItem assembly;
            bool isLoaded = Utility.LoadCachedAssembly(Caching.ActivityAssemblyItems, assemblyName, out assembly);
            if (assembly == null)
            {
                assembly = new ActivityAssemblyItem()
                {
                    Name = assemblyName.Name,
                    Version = assemblyName.Version,
                    Location = tempFullPath,
                    AssemblyName = assemblyName,
                };

                Caching.CacheAssembly(new[] { assembly }.ToList(), true);
                Caching.Refresh();
            }
        }
    }
}

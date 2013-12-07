using Microsoft.DynamicImplementations;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Tests.CDS
{
    public class PackageUtility
    {
        public static IPackage CreatePackage(string id,
                                              string version = "1.0",
                                              IEnumerable<string> content = null,
                                              IEnumerable<string> assemblyReferences = null,
                                              IEnumerable<string> tools = null,
                                              IEnumerable<PackageDependency> dependencies = null,
                                              int downloadCount = 0,
                                              string description = null,
                                              string summary = null,
                                              bool listed = true,
                                              string tags = "",
                                              string language = null,
                                              IEnumerable<string> satelliteAssemblies = null,
                                              string minClientVersion = null)
        {
            assemblyReferences = assemblyReferences ?? Enumerable.Empty<string>();
            satelliteAssemblies = satelliteAssemblies ?? Enumerable.Empty<string>();

            return CreatePackage(id,
                                 version,
                                 content,
                                 CreateAssemblyReferences(assemblyReferences),
                                 tools,
                                 dependencies,
                                 downloadCount,
                                 description,
                                 summary,
                                 listed,
                                 tags,
                                 language,
                                 CreateAssemblyReferences(satelliteAssemblies),
                                 minClientVersion);
        }

        public static IPackage CreatePackage(string id,
                                              string version,
                                              IEnumerable<string> content,
                                              IEnumerable<IPackageAssemblyReference> assemblyReferences,
                                              IEnumerable<string> tools,
                                              IEnumerable<PackageDependency> dependencies,
                                              int downloadCount,
                                              string description,
                                              string summary,
                                              bool listed,
                                              string tags,
                                              string language,
                                              IEnumerable<IPackageAssemblyReference> satelliteAssemblies,
                                              string minClientVersion = null)
        {
            var dependencySets = new List<PackageDependencySet>
            {
                new PackageDependencySet(null, dependencies ?? Enumerable.Empty<PackageDependency>())
            };

            return CreatePackage(id,
                                 version,
                                 content,
                                 assemblyReferences,
                                 tools,
                                 dependencySets,
                                 downloadCount,
                                 description,
                                 summary,
                                 listed,
                                 tags,
                                 language,
                                 satelliteAssemblies,
                                 minClientVersion);
        }


        public static IPackage CreatePackage(string id,
                                              string version,
                                              IEnumerable<string> content,
                                              IEnumerable<IPackageAssemblyReference> assemblyReferences,
                                              IEnumerable<string> tools,
                                              IEnumerable<PackageDependencySet> dependencySets,
                                              int downloadCount,
                                              string description,
                                              string summary,
                                              bool listed,
                                              string tags,
                                              string language,
                                              IEnumerable<IPackageAssemblyReference> satelliteAssemblies,
                                              string minClientVersion = null)
        {
            content = content ?? Enumerable.Empty<string>();
            assemblyReferences = assemblyReferences ?? Enumerable.Empty<IPackageAssemblyReference>();
            satelliteAssemblies = satelliteAssemblies ?? Enumerable.Empty<IPackageAssemblyReference>();
            dependencySets = dependencySets ?? Enumerable.Empty<PackageDependencySet>();
            tools = tools ?? Enumerable.Empty<string>();
            description = description ?? "Mock package " + id;

            var allFiles = new List<IPackageFile>();
            allFiles.AddRange(CreateFiles(content, "content"));
            allFiles.AddRange(CreateFiles(tools, "tools"));
            allFiles.AddRange(assemblyReferences);
            allFiles.AddRange(satelliteAssemblies);

            var mockPackage = new Implementation<IPackage>();
            mockPackage.Register(m => m.IsAbsoluteLatestVersion).Return(true);
            mockPackage.Register(m => m.IsLatestVersion).Return(String.IsNullOrEmpty(SemanticVersion.Parse(version).SpecialVersion));
            mockPackage.Register(m => m.Id).Return(id);
            mockPackage.Register(m => m.Listed).Return(true);
            mockPackage.Register(m => m.Version).Return(new SemanticVersion(version));
            mockPackage.Register(m => m.GetFiles()).Return(allFiles);
            mockPackage.Register(m => m.AssemblyReferences).Return(assemblyReferences);
            mockPackage.Register(m => m.DependencySets).Return(dependencySets);
            mockPackage.Register(m => m.Description).Return(description);
            mockPackage.Register(m => m.Language).Return("en-US");
            mockPackage.Register(m => m.Authors).Return(new[] { "Tester" });
            mockPackage.Register(m => m.GetStream()).Return(new MemoryStream());
            mockPackage.Register(m => m.LicenseUrl).Return(new Uri("ftp://test/somelicense.txts"));
            mockPackage.Register(m => m.Summary).Return(summary);
            mockPackage.Register(m => m.FrameworkAssemblies).Return(Enumerable.Empty<FrameworkAssemblyReference>());
            mockPackage.Register(m => m.Tags).Return(tags);
            mockPackage.Register(m => m.Title).Return(String.Empty);
            mockPackage.Register(m => m.DownloadCount).Return(downloadCount);
            mockPackage.Register(m => m.RequireLicenseAcceptance).Return(false);
            mockPackage.Register(m => m.Listed).Return(listed);
            mockPackage.Register(m => m.Language).Return(language);
            mockPackage.Register(m => m.IconUrl).Return((Uri)null);
            mockPackage.Register(m => m.ProjectUrl).Return((Uri)null);
            mockPackage.Register(m => m.ReleaseNotes).Return("");
            mockPackage.Register(m => m.Owners).Return(new string[0]);
            mockPackage.Register(m => m.Copyright).Return("");
            mockPackage.Register(m => m.MinClientVersion).Return(minClientVersion == null ? new Version() : Version.Parse(minClientVersion));
            if (!listed)
            {
                mockPackage.Register(m => m.Published).Return(Constants.Unpublished);
            }
            else
            {
                mockPackage.Register(m => m.Published).Return(DateTimeOffset.Now);
            }
            var targetFramework = allFiles.Select(f => f.TargetFramework).Where(f => f != null);
            mockPackage.Register(m => m.GetSupportedFrameworks()).Return(targetFramework);
            return mockPackage.Instance;
        }

        public static List<IPackageFile> CreateFiles(IEnumerable<string> fileNames, string directory = "")
        {
            var files = new List<IPackageFile>();
            foreach (var fileName in fileNames)
            {
                string path = PathFixUtility.FixPath(Path.Combine(directory, fileName));
                var mockFile = new Implementation<IPackageFile>();
                mockFile.Register(m => m.Path).Return(path);
                mockFile.Register(m => m.GetStream()).Return(new MemoryStream(Encoding.Default.GetBytes(path)));

                string effectivePath;
                FrameworkName fn = VersionUtility.ParseFrameworkNameFromFilePath(path, out effectivePath);
                mockFile.Register(m => m.TargetFramework).Return(fn);
                mockFile.Register(m => m.EffectivePath).Return(effectivePath);
                mockFile.Register(m => m.SupportedFrameworks).Return(
                    fn == null ? new FrameworkName[0] : new FrameworkName[] { fn });

                files.Add(mockFile.Instance);
            }
            return files;
        }

        private static List<IPackageAssemblyReference> CreateAssemblyReferences(IEnumerable<string> fileNames)
        {
            var assemblyReferences = new List<IPackageAssemblyReference>();
            foreach (var fileName in fileNames)
            {
                var mockAssemblyReference = new Implementation<IPackageAssemblyReference>();
                mockAssemblyReference.Register(m => m.GetStream()).Return(new MemoryStream());
                mockAssemblyReference.Register(m => m.Path).Return(fileName);
                mockAssemblyReference.Register(m => m.Name).Return(Path.GetFileName(fileName));


                string effectivePath;
                FrameworkName fn;
                try
                {
                    fn = ParseFrameworkName(fileName, out effectivePath);
                }
                catch (ArgumentException)
                {
                    effectivePath = fileName;
                    fn = VersionUtility.UnsupportedFrameworkName;
                }

                if (fn != null)
                {
                    mockAssemblyReference.Register(m => m.EffectivePath).Return(effectivePath);
                    mockAssemblyReference.Register(m => m.TargetFramework).Return(fn);
                    mockAssemblyReference.Register(m => m.SupportedFrameworks).Return(new[] { fn });
                }
                else
                {
                    mockAssemblyReference.Register(m => m.EffectivePath).Return(fileName);
                }

                assemblyReferences.Add(mockAssemblyReference.Instance);
            }
            return assemblyReferences;
        }

        private static FrameworkName ParseFrameworkName(string fileName, out string effectivePath)
        {
            if (fileName.StartsWith("lib\\"))
            {
                fileName = fileName.Substring(4);
                return VersionUtility.ParseFrameworkFolderName(fileName, strictParsing: false, effectivePath: out effectivePath);
            }

            effectivePath = fileName;
            return null;
        }
    }
}

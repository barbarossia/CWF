using NuGet;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    internal static class PackageSourceBuilder
    {
        internal static PackageSourceProvider CreateSourceProvider(ISettings settings)
        {
            var packageSourceProvider = new PackageSourceProvider(settings);
            return packageSourceProvider;
        }
    }
}

using NuGet;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Support.Workflow.Authoring.CDS
{
    public static class AggregateRepositoryHelper
    {
        public static AggregateRepository CreateAggregateRepositoryFromSources(IPackageRepositoryFactory factory, IPackageSourceProvider sourceProvider, IEnumerable<string> sources)
        {
            AggregateRepository repository;
            if (sources != null && sources.Any())
            {
                var repositories = sources.Select(s => sourceProvider.ResolveSource(s))
                                             .Select(factory.CreateRepository)
                                             .ToList();
                repository = new AggregateRepository(repositories);
            }
            else
            {
                repository = sourceProvider.CreateAggregateRepository(factory, ignoreFailingRepositories: true);
            }

            return repository;
        }
    }
}

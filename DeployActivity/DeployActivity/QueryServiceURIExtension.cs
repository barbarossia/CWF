using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeployActivity
{
    /// <summary>
    /// In order to use a Deploy activity in a workflow, the workflow host must supply a URI for QueryService
    /// through this extension. See DeployTest for an example.
    /// </summary>
    public class QueryServiceURIExtension
    {
        public QueryServiceURIExtension(string uriOfQueryService) : this(new Uri(uriOfQueryService))
        {

        }

        public QueryServiceURIExtension(Uri uriOfQueryService)
        {
            this.QueryServiceURI = uriOfQueryService;
        }

        public Uri QueryServiceURI { get; private set; }
    }
}

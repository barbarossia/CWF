using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Data {
    public enum Env {
        Dev = 1,
        Test = 2,
        Stage = 3,
        Prod = 4,

        Any = 11,
        All = 12,
    }
}

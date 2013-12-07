using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling
{
    [Serializable]
    public class CDSPackageException : Exception
    {
        public CDSPackageException()
            : base(null)
        {
        }

        public CDSPackageException(string msg) : base(msg)
        {

        }

        public CDSPackageException(string msgFmt, params string[] msgParams)
            : base(string.Format(msgFmt, msgParams))
        {

        }

        public CDSPackageException(string msg, Exception inner)
            : base(msg, inner)
        {

        }
    }
}

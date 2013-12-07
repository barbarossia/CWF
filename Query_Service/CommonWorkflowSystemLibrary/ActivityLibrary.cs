using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow
{
    class xxActivityLibrary
    {
        /// <summary>
        /// Added during the conversion of the EDm to ER model
        /// </summary>
        public Guid NEWActivityLibraryID { get; set; }
        public Int32 NEWId { get; set; }

        /// <summary>
        /// Used only in the EDm model
        /// </summary>
        public Guid Id { get; set; }

        public string Name { get; set; }
        public AuthorityGroup AuthGroup { get; set; }

    }
}

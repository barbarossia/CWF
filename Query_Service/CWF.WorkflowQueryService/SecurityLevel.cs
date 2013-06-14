using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CWF.WorkflowQueryService
{
    /// <summary>
    /// The security level of current user.
    /// </summary>
    public enum SecurityLevel
    {
        /// <summary>
        /// Person with administration rights within the workflow framework development tool
        /// </summary>
        Administrator = 90,

        /// <summary>
        /// Person with a framework team developer rights
        /// </summary>
        FrameworkDeveloper = 50,

        /// <summary>
        /// A person with the user rights of an application developer (e.g. OAS Developers) 
        /// </summary>
        Developer = 40,

        /// <summary>
        /// A person with the rights to create and develop workflows
        /// </summary>
        Author = 30,

        /// <summary>
        /// The read only. Person with read only rights to view workflows
        /// </summary>
        ReadOnly = 20,


        /// <summary>
        /// Cannot determine access rights.
        /// </summary>
        Offline = 10,

        /// <summary>
        /// The no access.
        /// </summary>
        NoAccess = 0,
    }
}
namespace CWF.DataContracts
{
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    public enum TaskActivityStatus
    {
        New = 0,

        Assigned = 1,

        Editing = 2,

        CheckedIn = 3,

        Unassigned = 4
    }
}

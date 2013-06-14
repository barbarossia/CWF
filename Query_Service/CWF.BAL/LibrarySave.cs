using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CWF.Constants;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

using System.Data.Entity;
using CWF.DataContracts;
using CWF.DAL;


namespace CWF.BAL
{
    public class SaveLibrary
    {
        ///// <summary>
        ///// OLD
        ///// </summary>
        ///// <param name="library"></param>
        ///// <param name="assembly"></param>
        ///// <returns></returns>
        //static public LibrarySaveReplyDC LibrarySave(LibrarySaveRequestDC request)
        //{
        //    LibrarySaveReplyDC reply = new LibrarySaveReplyDC();
        //    return reply;
        //}

        //#region [ helper ]
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //static private string FormatAndCreateGuidForLog()
        //{
        //    string guid = Guid.NewGuid().ToString();
        //    return "[" + guid + "]";
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="library"></param>
        ///// <param name="assembly"></param>
        ///// <returns></returns>
        ////static private ActivityLibrary ActivityLibraryNewPopulate(Library library, Byte[] assembly)
        ////{
        ////    ActivityLibrary reply = new ActivityLibrary()
        ////    {
        ////        Id = library.Id,
        ////        Name = library.Name,
        ////        VersionNumber = library.VersionNumber,
        ////        Executable = assembly,
        ////        Category = library.Category,
        ////        HasActivities = library.HasActivities,
        ////        Description = library.Description,
        ////        ImportedBy = library.ImportedBy,
        ////        Status = library.Status,
        ////        AuthGroup = "" //todo review use of this field
        ////    };
        ////    return reply;
        ////}
        //#endregion
    }
}

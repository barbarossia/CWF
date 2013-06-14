using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DataContracts;

namespace Microsoft.Support.Workflow.Service.BusinessServices.Tests.UnitTests
{
    /// <summary>
    /// Defines dynamic implementations to isolate the activity library data access layer calls.
    /// </summary>
    public static class ActivityLibraryRepositoryServiceIsolator
    {
        /// <summary>
        /// Gets a dynamic implementation that simulates the success response return a list of activity libraries.
        /// </summary>
        /// <returns>Dynamic implementation of type ActivityLibraryRepositoryService.</returns>
        public static ImplementationOfType GetActivityLibrariesSuccessResponseMock(List<ActivityLibraryDC> list)
        {
            ImplementationOfType impl = new ImplementationOfType(typeof(ActivityLibraryRepositoryService));
            impl.Register(() => ActivityLibraryRepositoryService.GetActivityLibraries(Argument<ActivityLibraryDC>.Any, true))
                .Execute(delegate { return list; });

            return impl;
        }

        /// <summary>
        /// Gets a dynamic implementation that simulates the exception response return a list of activity libraries.
        /// </summary>
        /// <returns>Dynamic implementation of type ActivityLibraryRepositoryService.</returns>
        public static ImplementationOfType GetActivityLibrariesExceptionResponseMock(Exception e)
        {
            ImplementationOfType impl = new ImplementationOfType(typeof(ActivityLibraryRepositoryService));
            impl.Register(() => ActivityLibraryRepositoryService.GetActivityLibraries(Argument<ActivityLibraryDC>.Any, true))
                .Execute(delegate { throw e; return null; });

            return impl;
        }
    }
}

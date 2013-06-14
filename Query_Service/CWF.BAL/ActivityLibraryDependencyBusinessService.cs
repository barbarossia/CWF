using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DAL;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    /// <summary>
    /// Defines the business services for handling ActivityLibraryDependency entities.
    /// </summary>
    public static class ActivityLibraryDependencyBusinessService
    {
        /// <summary>
        /// Validates input and gets the list of all the dependencies in the dependency hierarchy for an an activity library.
        /// </summary>
        /// <param name="request">Request that defines the root activity library for which the dependencies are to be found.</param>
        /// <returns>Response that contains a list of dependencies.</returns>
        public static List<StoreActivityLibrariesDependenciesDC> GetActivityLibraryDependencyTree(StoreActivityLibrariesDependenciesDC request)
        {
            List<StoreActivityLibrariesDependenciesDC> reply = null;
            try
            {
                // Validates the input and throws ValidationException for any issues found.
                request.ValidateRequest();

                reply = ActivityLibraryDependencyRepositoryService.GetActivityLibraryDependencyTree(request);
            }
            catch (ValidationException e)
            {
                e.HandleException();
            }
            catch (DataAccessException e)
            {
                e.HandleException();
            }

            return reply;
        }
    }
}

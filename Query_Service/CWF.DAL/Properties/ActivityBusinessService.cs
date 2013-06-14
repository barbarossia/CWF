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
    /// Defines the business services for handling Activity entities.
    /// </summary>
    public static class ActivityBusinessService
    {
        /// <summary>
        /// Performs input validation and gets activities by activity library ID or Name & Version combination.
        /// </summary>
        /// <param name="request">Request that specifies activity library identifier info.</param>
        /// <param name="includeXaml">Flag that indicates whether activity XAML should be returned.</param>
        /// <returns>Response that contains a list of activities.</returns>
        public static GetActivitiesByActivityLibraryNameAndVersionReplyDC GetActivitiesByActivityLibrary(GetActivitiesByActivityLibraryNameAndVersionRequestDC request, bool includeXaml)
        {
            var reply = new GetActivitiesByActivityLibraryNameAndVersionReplyDC();

            try
            {
                // Validates the input and throws ValidationException for any issues found.
                request.ValidateRequest();

                var newRequest = new GetLibraryAndActivitiesDC();
                var activityLibraryDC = new ActivityLibraryDC();
                var newReply = new List<GetLibraryAndActivitiesDC>();

                activityLibraryDC.IncallerVersion = request.IncallerVersion;
                activityLibraryDC.Incaller = request.Incaller;
                activityLibraryDC.Name = request.Name;
                activityLibraryDC.VersionNumber = request.VersionNumber;
                newRequest.ActivityLibrary = activityLibraryDC;

                newReply = ActivityRepositoryService.GetActivitiesByActivityLibrary(newRequest, includeXaml);

                if (newReply != null && newReply.Count > 0)
                {
                    reply.List = newReply[0].StoreActivitiesList;
                }
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

        /// <summary>
        /// Performs input validation and gets activities by searching with some params
        /// </summary>
        /// <param name="request">Request that specifies the search parameters</param>
        /// <returns>Response that contains a list of activities.</returns>
        public static ActivitySearchReplyDC SearchActivities(ActivitySearchRequestDC request)
        {
            var reply = new ActivitySearchReplyDC();

            try
            {
                // Validates the input and throws ValidationException for any issues found.
                request.ValidateRequest();                                   

                reply = ActivityRepositoryService.SearchActivities(request);                
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

        /// <summary>
        /// Performs input validation and checks if an activity exists in the data store
        /// </summary>
        /// <param name="request">Request that specifies activity identifier info.</param>
        /// <returns>Result of the operation.</returns>
        public static StatusReplyDC CheckActivityExists(StoreActivitiesDC request)
        {
            var reply = new StatusReplyDC();

            try
            {
                // Validates the input and throws ValidationException for any issues found.
                request.ValidateRequest();

                reply = ActivityRepositoryService.CheckActivityExists(request);

                return reply;
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

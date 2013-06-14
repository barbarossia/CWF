

namespace Microsoft.Support.Workflow.Service.BusinessServices
{

    using System.Collections.Generic;
    using CWF.DAL;
    using CWF.DataContracts;
    using DataAccessServices;

    /// <summary>
    /// Defines the business services for handling ActivityLibrary entities.
    /// </summary>
    public static class ActivityLibraryBusinessService
    {
        /// <summary>
        /// Performs input validation and gets activity libraries that match with search 
        /// parameters while including assemblies with each activity library entry found.
        /// </summary>
        /// <param name="request">ActivityLibraryDC data contract.</param>
        /// <returns>List of ActivityLibraryDC.</returns>
        public static List<ActivityLibraryDC> GetActivityLibraries(ActivityLibraryDC request)
        {
            List<ActivityLibraryDC> reply = null;
            try
            {
                // Validates the input and throws ValidationException for any issues found.
                request.ValidateGetRequest();

                // Gets the activity libraries that match search criteria.
                reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
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
        /// Performs input validation and gets activity libraries that match with search 
        /// parameters without including assemblies.
        /// </summary>
        /// <param name="request">GetLibraryAndActivitiesDC.</param>
        /// <returns>GetLibraryAndActivitiesDC object.</returns>
        public static GetAllActivityLibrariesReplyDC GetActivityLibrariesWithoutDlls(GetAllActivityLibrariesRequestDC request)
        {
            ActivityLibraryDC newRequest = null;
            if (request != null)
            {
                newRequest = new ActivityLibraryDC();
                newRequest.Name = null;
                newRequest.VersionNumber = null;
                newRequest.Incaller = request.Incaller;
                newRequest.IncallerVersion = request.IncallerVersion;
            }

            GetAllActivityLibrariesReplyDC reply = new GetAllActivityLibrariesReplyDC();           
                        
            try
            {
                // Validates the input and throws ValidationException for any issues found.
                newRequest.ValidateGetRequest();

                // Gets the activity libraries that match search criteria.
                reply.List = ActivityLibraryRepositoryService.GetActivityLibraries(newRequest, false);
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
        /// Performs input validation and gets a list of activity libraries that don't exist in 
        /// the data store based on an initial list to check.
        /// </summary>
        /// <param name="request">ActivityLibrariesCheckExistsRequest data contract.</param>
        /// <returns>List of ActivityLibraryDC.</returns>
        public static GetMissingActivityLibrariesReply GetMissingActivityLibraries(GetMissingActivityLibrariesRequest request)
        {
            var reply = new GetMissingActivityLibrariesReply();

            try
            {
                // Validates the input and throws ValidationException for any issues found.
                request.ValidateRequest();

                // Gets the activity libraries that are not present in the asset store and match the name and version criteria.
                reply.MissingActivityLibraries = ActivityLibraryRepositoryService.GetMissingActivityLibraries(request);
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

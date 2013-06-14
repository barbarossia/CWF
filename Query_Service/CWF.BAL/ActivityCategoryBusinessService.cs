using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using CWF.DAL;
using Microsoft.Support.Workflow.Service.DataAccessServices;

namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    /// <summary>
    /// Defines the business services for handling ActivityCategory entities.
    /// </summary>
    public static class ActivityCategoryBusinessService
    {
        /// <summary>
        /// Performs input validation and gets activity categories by ID, Name or by ID & Name.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>List of activity categories found.</returns>
        public static List<ActivityCategoryByNameGetReplyDC> GetActivityCategories(ActivityCategoryByNameGetRequestDC request)
        {
            List<ActivityCategoryByNameGetReplyDC> reply = null;
            try
            {
                // Validates the input and throws ValidationException for any issues found.
                request.ValidateRequest();

                reply = ActivityCategoryRepositoryService.GetActivityCategories(request);
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

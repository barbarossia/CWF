namespace Microsoft.Support.Workflow.Service.BusinessServices
{
    using System;
    using System.Linq;
    using CWF.DataContracts;
    using QueryService.Common;

    /// <summary>
    /// Defines extension methods on ActivityLibraryDC data contract.
    /// </summary>
    internal static class DataContractValidationExtensions
    {
        /// <summary>
        /// Validates the input for an activity library get request.
        /// </summary>
        /// <param name="activityLibrary">Activity library data contract to validate.</param>
        internal static void ValidateGetRequest(this ActivityLibraryDC activityLibrary)
        {
            if (activityLibrary == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }

            if (String.IsNullOrEmpty(activityLibrary.Incaller))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
            }

            if (String.IsNullOrEmpty(activityLibrary.IncallerVersion))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerVersionRequired);
            }
        }

        /// <summary>
        /// Validates the input for a request.
        /// </summary>
        /// <param name="request">Request Data Contract to validate.</param>
        internal static void ValidateRequest(this RequestHeader request)
        {
            if (request == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }
            
            if (String.IsNullOrEmpty(request.Incaller))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
            }

            if (String.IsNullOrEmpty(request.IncallerVersion))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerVersionRequired);
            }
        }

        /// <summary>
        /// Validates the input for a request.
        /// </summary>
        /// <param name="request">Request data contract to validate.</param>
        internal static void ValidateRequest(this GetMissingActivityLibrariesRequest request)
        {
            if (request == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }

            (request as RequestHeader).ValidateRequest();

            if((request.ActivityLibrariesList == null) || (!request.ActivityLibrariesList.Any()))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.ActivityLibrariesListRequired);
            }
        }

        /// <summary>
        /// Validates the input for a request.
        /// </summary>
        /// <param name="request">Request data contract to validate.</param>
        internal static void ValidateRequest(this ActivityCategoryByNameGetRequestDC request)
        {
            if (request == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }

            (request as RequestHeader).ValidateRequest();
        }

        /// <summary>
        /// Validates the input for a request.
        /// </summary>
        /// <param name="request">Request data contract to validate.</param>
        internal static void ValidateRequest(this StoreActivityLibrariesDependenciesDC request)
        {
            if (request == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }

            if (String.IsNullOrEmpty(request.Incaller))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
            }

            if (String.IsNullOrEmpty(request.IncallerVersion))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerVersionRequired);
            }
        }

        /// <summary>
        /// Validates the input for a request.
        /// </summary>
        /// <param name="request">Request data contract to validate.</param>
        internal static void ValidateRequest(this StoreActivitiesDC request)
        {
            if (request == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }

            if (String.IsNullOrEmpty(request.Incaller))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerNameRequired);
            }

            if (String.IsNullOrEmpty(request.IncallerVersion))
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.CallerVersionRequired);
            }
        }  
  
        /// <summary>
        /// Validates the input for a request.
        /// </summary>
        /// <param name="request">Request data contract to validate.</param>
        internal static void ValidateRequest(this GetActivitiesByActivityLibraryNameAndVersionRequestDC request)
        {
            if (request == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }

            (request as RequestHeader).ValidateRequest();
        }

        /// <summary>
        /// Validates the input for a request.
        /// </summary>
        /// <param name="request">Request data contract to validate.</param>
        internal static void ValidateRequest(this ActivitySearchRequestDC request)
        {
            if (request == null)
            {
                throw new ValidationException(EventCode.BusinessLayerEvent.Validation.RequestIsNull);
            }            
        }
    }
}

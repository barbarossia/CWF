//-----------------------------------------------------------------------
// <copyright file="Primitives.cs" company="Microsoft">
// Copyright
// Primitive operations for validation pipeline in WCF
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DCValidation
{
    using System;
    using CWF.DataContracts;

    /// <summary>
    /// Primitive operations for the validation pipeline
    /// </summary>
    public static class Primitives
    {
        #region [ IsGuidEmpty ]
        /// <summary>
        /// Tests for an empty guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>Returns true if empty</returns>
        public static bool IsGuidEmpty(Guid guid)
        {
            if (guid == Guid.Empty)
                return true;
            else
                return false;
        }
        #endregion

        #region [ IsObjectNull ]
        /// <summary>
        /// tests the object for null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true iof null</returns>
        public static bool IsObjectNull(object obj)
        {
            if (obj == null)
                return true;
            else
                return false;
        }
        #endregion

        #region [ IsIntLeqZero ]
        /// <summary>
        /// tests for int LEQ to zero
        /// </summary>
        /// <param name="testInt"></param>
        /// <returns>true if LEQ 0</returns>
        public static bool IsIntLeqZero(int testInt)
        {
            if (testInt <= 0)
                return true;
            else
                return false;
        }
        #endregion

        #region [ IsIntZero ]
        /// <summary>
        /// tests for int EQL to zero
        /// </summary>
        /// <param name="testInt"></param>
        /// <returns>true if LEQ 0</returns>
        public static bool IsIntZero(int testInt)
        {
            if (testInt == 0)
                return true;
            else
                return false;
        }
        #endregion

        #region [ IsStringNullEmptyOrWhitespacentLeqZero ]
        /// <summary>
        /// tests for null, empty, or whitespace string
        /// </summary>
        /// <param name="testString">string to test</param>
        /// <returns>true is cnditions exist</returns>
        public static bool IsStringNullEmptyOrWhitespace(string testString)
        {
            if (string.IsNullOrEmpty(testString) || string.IsNullOrWhiteSpace(testString))
                return true;
            else
                return false;
        }
        #endregion

        #region [ CreateStatusReplyObject ]
        /// <summary>
        /// Creates a StatusReply object for the error found
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorGuid"></param>
        /// <returns>Statusreply object</returns>
        public static CWF.DataContracts.StatusReplyDC CreateStatusReplyObject(int errorCode, string errorMessage, string errorGuid)
        {
            CWF.DataContracts.StatusReplyDC reply = new StatusReplyDC();
            reply.Errorcode = errorCode;
            reply.ErrorGuid = errorGuid;
            reply.ErrorMessage = errorMessage;
            return reply;
        }
        #endregion

        #region [ BasicParameterCheck ]
        /// <summary>
        /// Check the request object for null and the incaller/incallerVersion objects for null and white space
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns>StatusReplyDC object</returns>
        public static StatusReplyDC BasicParameterCheck(RequestReplyCommonHeader request, string action)
        {
            CWF.DataContracts.StatusReplyDC reply = new StatusReplyDC();
            if (Primitives.IsObjectNull(request))
                reply = Primitives.CreateStatusReplyObject(Constants.SprocValues.REQUEST_OBJECT_IS_NULL_ID, string.Format(Constants.SprocValues.REQUEST_OBJECT_IS_NULL_MSG, action, "StoreActivitiesDC"), string.Empty);
            else
                if (Primitives.IsStringNullEmptyOrWhitespace(request.Incaller))
                    reply = Primitives.CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INCALLER_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INCALLER_MSG, string.Empty);
                else
                    if (Primitives.IsStringNullEmptyOrWhitespace(request.IncallerVersion))
                        reply = Primitives.CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INCALLERVERSION_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INCALLERVERSION_MSG, string.Empty);
            return reply;
        }

        #endregion

        #region [ CheckCreateStoreActivity ]

        /// <summary>
        /// Validates the request DC for CreateStoreActivity
        /// </summary>
        /// <param name="request">request object</param>
        /// <returns>StatusReplyDC object</returns>
        public static StatusReplyDC CheckCreateStoreActivity(StoreActivitiesDC request)
        {
            StatusReplyDC reply = new StatusReplyDC();
            //// Guid
            if (IsGuidEmpty(request.Guid))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INGUID_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INGUID_MSG, string.Empty);
                return reply;
            }
            //// Name
            if (IsStringNullEmptyOrWhitespace(request.Name))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INNAME_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INNAME_MSG, string.Empty);
                return reply;
            }
            //// shortName
            if (IsStringNullEmptyOrWhitespace(request.ShortName))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INSHORTNAMENAME_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INSHORTNAMENAME_MSG, string.Empty);
                return reply;
            }
            //// Description
            if (IsStringNullEmptyOrWhitespace(request.Description))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INDESCRIPTION_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INDESCRIPTION_MSG, string.Empty);
                return reply;
            }
            //// MetaTags
            if (IsStringNullEmptyOrWhitespace(request.MetaTags))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INMETATAGS_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INMETATAGS_MSG, string.Empty);
                return reply;
            }
            //// IsSwitch - not null
            //// IsService - not null
            //// IsUxActivity - not null
            //// DefaultRender
            if (IsStringNullEmptyOrWhitespace(request.DefaultRender))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INDEFAULRENDER_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INDEFAULRENDER_MSG, string.Empty);
                return reply;
            }
            //// ActivityCategoryName
            if (IsStringNullEmptyOrWhitespace(request.ActivityCategoryName))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INACTIVITYCATEGORYNAME_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INACTIVITYCATEGORYNAME_MSG, string.Empty);
                return reply;
            }
            //// ToolBoxName
            if (IsStringNullEmptyOrWhitespace(request.ToolBoxName))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INTOOLBOXNAME_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INTOOLBOXNAME_MSG, string.Empty);
                return reply;
            }
            //// IsToolBoxActivity
            //// Version
            if (IsStringNullEmptyOrWhitespace(request.Version))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INVERSION_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INVERSION_MSG, string.Empty);
                return reply;
            }
            //// StatusName
            if (IsStringNullEmptyOrWhitespace(request.StatusCodeName))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INSTATUSCODENAME_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INSTATUSCODENAME_MSG, string.Empty);
                return reply;
            }
            //// WorkflowTypeName
            if (IsStringNullEmptyOrWhitespace(request.WorkFlowTypeName))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INWORKFLOWTYPENAME_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INWORKFLOWTYPENAME_MSG, string.Empty);
                return reply;
            }
            //// InsertedByUserAlias
            if (IsStringNullEmptyOrWhitespace(request.InsertedByUserAlias))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_ININSERTEDBYUSERALIAS_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_ININSERTEDBYUSERALIAS_MSG, string.Empty);
                return reply;
            }
            return reply;
        }

        #endregion

        #region [ CheckUpdateStoreActivity ]

        /// <summary>
        /// Checks the parameters required for an StoreActivity update
        /// </summary>
        /// <param name="request"></param>
        /// <returns>StatusReplyDC object</returns>
        public static StatusReplyDC CheckUpdateStoreActivity(StoreActivitiesDC request)
        {
            StatusReplyDC reply = new StatusReplyDC();
            //// Id <= 0
            if (Primitives.IsIntLeqZero(request.Id))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INID_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, string.Empty);
                return reply;
            }

            //// 
            if (Primitives.IsStringNullEmptyOrWhitespace(request.UpdatedByUserAlias))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INUPDATEDBYUSERALIAS_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INUPDATEDBYUSERALIAS_MSG, string.Empty);
                return reply;
            }

            return reply;
        }

        #endregion

        #region [ CheckDeleteStoreActivity ]

        /// <summary>
        /// Validates the DeleteStoreActivity request DC
        /// </summary>
        /// <param name="request">StoreActivitiesDC object</param>
        /// <returns>StatusReplyDC object</returns>
        public static StatusReplyDC CheckDeleteStoreActivity(StoreActivitiesDC request)
        {
            StatusReplyDC reply = new StatusReplyDC();

            //// Check Id for >= 0
            if (Primitives.IsIntLeqZero(request.Id))
            {
                reply = CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_VALUE_INID_ID, Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, string.Empty);
                return reply;
            }

            return reply;
        }

        #endregion

        #region [ CheckGetStoreActivity ]

        /// <summary>
        /// Checks the parameters for a StoreActivities get
        /// Rules are:
        /// 1. InID - gets a specific
        /// 2. InGuid - gets a specific
        /// 3. InName - gets one or more
        /// 4. InName and InVersion - gets a specific
        /// 5. InVersion and no InName - invalid
        /// </summary>
        /// <param name="request"></param>
        /// <returns>StatusReplyDC object</returns>
        public static StatusReplyDC CheckGetStoreActivity(StoreActivitiesDC request)
        {
            StatusReplyDC reply = new StatusReplyDC();

            //// matrix of valid combinations
            ////  case    InId     InGuid     InName     InVersion
            ////  ================================================
            ////    1      x         0          0           0
            ////    2      0         x          0           0
            ////    3      0         0          x           0
            ////    4      0         0          x           x
            ////    5      0         0          0           0

            //// Case #5
            if (Primitives.IsIntZero(request.Id) && Primitives.IsGuidEmpty(request.Guid) && Primitives.IsStringNullEmptyOrWhitespace(request.Name) && Primitives.IsStringNullEmptyOrWhitespace(request.Version))
                return reply;

            //// Case #3
            if (Primitives.IsIntZero(request.Id) && Primitives.IsGuidEmpty(request.Guid) && !Primitives.IsStringNullEmptyOrWhitespace(request.Name) && Primitives.IsStringNullEmptyOrWhitespace(request.Version))
                return reply;

            //// Case #4
            if (Primitives.IsIntZero(request.Id) && Primitives.IsGuidEmpty(request.Guid) && !Primitives.IsStringNullEmptyOrWhitespace(request.Name) && !Primitives.IsStringNullEmptyOrWhitespace(request.Version))
                return reply;

            //// Case #2
            if (Primitives.IsIntZero(request.Id) && !Primitives.IsGuidEmpty(request.Guid) && Primitives.IsStringNullEmptyOrWhitespace(request.Name) && Primitives.IsStringNullEmptyOrWhitespace(request.Version))
                return reply;

            //// Case #1
            if (!Primitives.IsIntLeqZero(request.Id) && Primitives.IsGuidEmpty(request.Guid) && Primitives.IsStringNullEmptyOrWhitespace(request.Name) && Primitives.IsStringNullEmptyOrWhitespace(request.Version))
                return reply;
            reply = Primitives.CreateStatusReplyObject(Constants.SprocValues.INVALID_PARMETER_COMBINATION_ON_GET_ID, Constants.SprocValues.INVALID_PARMETER_COMBINATION_ON_GET_MSG, string.Empty);
            return reply;
        }

        #endregion
    }
}

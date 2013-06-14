//-----------------------------------------------------------------------
// <copyright file="ErrorConstants.cs" company="Microsoft">
// Copyright
// All error constants (strings and ints)
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Support.Workflow.Service.DataAccessServices
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Common location for all error constants - both integer error codes, and string descriptions
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "self documentation in each triplet")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "const int and strings require upper case and underscore")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Large error constant section with Doc, id, and message in sequence")]
    public static class SprocValues
    {
        public const string DAL_CALLER_INFO = "CWF.DAL";
        public const string DAL_CALLER_VERSION = "1.0.0.0";
        public const string LOG_EVENT_SOURCE = "CWF.WorkflowsService";
        public const string LOG_LOCATION = "CWF";
        public const string NOT_IMPLEMENTED = "Not Implemented";

        //// LibrarySave messages
        public const string LIBRARYSAVE_NO_ASSEMBLY = "{0}Assembly required to perform LibrarySave";
        public const string LIBRARYSAVE_NO_ASSEMBLY_LIBRARYNAMEPASSED = "{0}Assembly required to perform LibrarySave for library name {1}";
        public const string LIBRARYSAVE_NO_LIBRARY = "{0}Library required to perform LibrarySave";
        public const string DAL_ERROR_LOGGED_IN_SPROC = "See previous error in log for {0}";

        /// <summary>
        /// The following error codes are a contract between the SQL Db sprocs and the DAL layer.
        /// that is passed in the OUT parameter of the call.
        /// All other cases are generated via the RETURN operator in the sproc.
        /// Range/Usage

        ///  0                   Good reply
        ///                      Soft errors
        ///  55000 -   55019     Basic structure issues
        ///  55020 -   55039     Delete sproc errors during execution (parameter value doesn't point to valid DB row)
        ///  55040 -   55059     Get sproc errors during execution (parameter value doesn't point to valid DB row)
        ///  55060 -   55065     CreateOrUpdate errors during execution (parameter value doesn't point to valid DB row)
        ///  55066 -   55098     BAL layer error
        ///  56099               SQL Server error occured during execution
        ///  55100 -   55150     Sproc required parameter failure
        /// 
        /// Ranges for errors:
        /// 
        /// Administrative  56000-56999	
        /// Operational     55000-55999	
        /// Analytic        n/a (but 54000-54999 reserved)	
        /// Debug           n/a (but 53000-53999 reserved)	
        /// APIUsage        n/a (but 52000-52999 reserved)	
        /// 
        /// </summary>
        //// Sproc caller types
        public enum SprocCallerTypes
        {
            /// <summary>
            /// DAL Get call
            /// </summary>
            Get,

            /// <summary>
            /// DAL Delete Call
            /// </summary>
            Delete,

            /// <summary>
            /// DAL Create or update call
            /// </summary>
            CreateOrUpdate
        }
        //// Good return
        public const int REPLY_ERRORCODE_VALUE_OK = 0;
        //// Basic structure issues
        public const int REQUEST_OBJECT_IS_NULL_ID = 55000;
        public const string REQUEST_OBJECT_IS_NULL_MSG = "request object{0} is null for call on {1}";

        public const int REQUEST_ACTION_OBJECT_IS_NULL_ID = 55002;
        public const string REQUEST_ACTION_OBJECT_IS_NULL_MSG = "request action is null";

        //// 55003  "request.StoreDependenciesRootActivityLibrary is null"
        public const int REQUEST_ACTION_STORE_DEPENDENCIES_ROOTACTIVITY_LIBRARY_IS_NULL_ID = 55003;
        public const string REQUEST_ACTION_STORE_DEPENDENCIES_ROOTACTIVITY_LIBRARY_IS_NULL_MSG = "request.StoreDependenciesRootActivityLibrary is null";

        //// 55004  "request.StoreDependenciesDependentActiveLibraryList is null"
        public const int REQUEST_ACTION_STORE_DEPENDENCIES_DEPENDENT_ACTIVITY_LIBRARY_IS_NULL_ID = 55004;
        public const string REQUEST_ACTION_STORE_DEPENDENCIES_DEPENDENT_ACTIVITY_LIBRARY_IS_NULL_MSG = "request.StoreDependenciesDependentActiveLibraryList is null";

        //// 55005
        public const int GENERIC_CATCH_ID = 56001;
        //// returns the exception message

        //// 55020
        //// Sproc return numbers & messages for StatusDC
        //// xxxxxxDelete
        public const int DELETE_INVALID_ID = 55020;
        public const string DELETE_INVALID_ID_MSG_FORMAT = "Invalid @InId ({0}) attempting to perform a DELETE on table {1}";
        public const string DELETE_INVALID_ID_MSG_NOFORMAT = "Invalid  @InId attempting to perform a DELETE on table";

        public const int DELETE_INVALID_NAME = 55022;
        public const string DELETE_INVALID_NAME_MSG_FORMAT = "Invalid @InName ({0}) attempting to perform a DELETE on table {1}";
        public const string DELETE_INVALID_NAME_MSG_NOFORMAT = "Invalid @InName attempting to perform a DELETE on table";

        public const int DELETE_INVALID_GUID = 55023;
        public const string DELETE_INVALID_GUID_MSG_FORMAT = "Invalid @InGuid ({0}) attempting to perform a DELETE on table {1}";
        public const string DELETE_INVALID_GUID_MSG_NOFORMAT = "Invalid @InGuid attempting to perform a DELETE on table";

        public const int DELETE_ALREADY_SOFT_DELETED = 55021;
        public const string DELETE_ALREADY_SOFT_DELETED_FORMAT = "Row already deleted - @InId ({0}) attempting to perform a DELETE on table {1}";
        public const string DELETE_ALREADY_SOFT_DELETED_NOFORMAT = "Row already deleted - @InId attempting to perform a DELETE on table";

        public const int DELETE_NOPARAMETERS = 55024;
        public const string DELETE_NOPARAMETERS_FORMAT = "Invalid Parameter Value - @InId ({0}), @InName, @InGuid cannot all be null attempting to perform a DELETE on table {1}";
        public const string DELETE_NOPARAMETERS_NOFORMAT = "Invalid Parameter Value (@InId, @InName, @inGuid cannot all be null";

        //// xxxxxxGET
        public const int GET_INVALID_ID = 55040;
        public const string GET_INVALID_ID_MSG_FORMAT = "Invalid @InId ({0}) attempting to perform a GET on table {1}";
        public const string GET_INVALID_ID_MSG_NOFORMAT = "Invalid @InId attempting to perform a GET on table";

        public const int GET_INVALID_GUID = 55041;
        public const string GET_INVALID_GUID_MSG_FORMAT = "Invalid @InGUID ({0}) attempting to perform a GET on table {1}";
        public const string GET_INVALID_GUID_MSG_NOFORMAT = "Invalid @InGUID attempting to perform a GET on table";

        public const int GET_INVALID_NAME = 55042;
        public const string GET_INVALID_NAME_MSG_FORMAT = "Invalid @InName ({0}) attempting to perform a GET on table {1}";
        public const string GET_INVALID_NAME_MSG_NOFORMAT = "Invalid @InName attempting to perform a GET on table";

        public const int GET_INVALID_SHORTNAME = 55043;
        public const string GET_INVALID_SHORTNAME_MSG_FORMAT = "Invalid @InShortName ({0}) attempting to perform a GET on table {1}";
        public const string GET_INVALID_SHORTNAME_MSG_NOFORMAT = "Invalid @InShortName attempting to perform a GET on table";

        public const int GET_INVALID_GETID_ON_SOFTDELETEDROW_ID = 55044;
        public const string GET_INVALID_GETID_ON_SOFTDELETEDROW_MSG_FORMAT = "Invalid @InId ({0}) attempting to perform a GET on table {1} that is marked soft delete";
        public const string GET_INVALID_GETID_ON_SOFTDELETEDROW_MSG_NOFORMAT = "Invalid @InId attempting to perform a GET on table that is marked soft delete";

        public const int GET_INVALID_GETGUID_ON_SOFTDELETEDROW_ID = 55045;
        public const string GET_INVALID_GETGUID_ON_SOFTDELETEDROW_MSG_FORMAT = "Invalid @InGuid ({0}) attempting to perform a GET on table {1} that is marked soft delete";
        public const string GET_INVALID_GETGUID_ON_SOFTDELETEDROW_MSG_NOFORMAT = "Invalid @InGuid attempting to perform a GET on table that is marked soft delete";

        public const int GET_INVALID_GETNAMEVERSION_ON_SOFTDELETEDROW_ID = 55046;
        public const string GET_INVALID_GETNAMEVERSION_ON_SOFTDELETEDROW_MSG_FORMAT = "Invalid @Name/@Version ({0}) attempting to perform a GET on table {1} that is marked soft delete";
        public const string GET_INVALID_GETNAMEVERSION_ON_SOFTDELETEDROW_MSG_NOFORMAT = "Invalid @Name/@Version attempting to perform a GET on table that is marked soft delete";

        public const int GET_INVALID_GETNAMEVERSION_ID = 55047;
        public const string GET_INVALID_GETNAMEVERSION_MSG_FORMAT = "Invalid @Name/@Version ({0}) attempting to perform a GET on table {1} ";
        public const string GET_INVALID_GETNAMEVERSION_MSG_NOFORMAT = "Invalid @Name/@Version attempting to perform a GET on table";

        public const int GET_STOREACTIVITY_WITH_CATEGORYNAME_ID = 56048;
        public const string GET_STOREACTIVITY_WITH_CATEGORYNAME_MSG_FORMAT = "No StoreActivity entries with @InCategoryName FK [{0}] while attempting to perform a GET on table {1}";
        public const string GET_STOREACTIVITY_WITH_CATEGORYNAME_MSG_NOFORMAT = "No StoreActivity entries with @InCategoryName FK while attempting to perform a GET on table StoreActivities";

        //// xxxxxxCreateOrUpdate
        public const int UPDATE_INVALID_ID = 55060;
        public const string UPDATE_INVALID_ID_MSG_FORMAT = "Invalid @InId ({0}) attempting to perform an UPDATE on table {1}";
        public const string UPDATE_INVALID_ID_MSG_NOFORMAT = "Invalid @InId attempting to perform an UPDATE on table";

        public const int UPDATE_INVALID_STOREACTIVITIESNAME_AND_VERSION = 55061;
        public const string UPDATE_INVALID_STOREACTIVITIESNAME_AND_VERSION_MSG_FORMAT = "Invalid @InStoreActivitName/@InStoreActivitiesVersion ({0}) attempting to perform an UPDATE on table {1}";
        public const string UPDATE_INVALID_STOREACTIVITIESNAME_AND_VERSION_MSG_NOFORMAT = "Invalid @InStoreActivitName/@InStoreActivitiesVersion attempting to perform an UPDATE on table";
        
        public const int UPDATE_INVALID_AUTHGROUPNAME_ID = 55063;
        public const string UPDATE_INVALID_AUTHGROUPNAME_MSG_FORMAT = "Invalid @InAuthGroupName ({0}) attempting to perform an UPDATE on table {1}";
        public const string UPDATE_INVALID_AUTHGROUPNAME_MSG_NOFORMAT = "Invalid @InAuthGroupName attempting to perform an UPDATE on table";

        //// 55097, "ActivityLibrary is marked for production and cannot be removed"
        public const int ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_ID = 55097;
        public const string ACTIVITYLIBRARY_MARKED_FOR_PRODUCTION_MSG = "ActivityLibrary is marked for production and cannot be removed";

        //// 55098, "Bad activityLibraryName/activityLibraryVersion in StoreActivitiesDC - All activityLibraryName/activityLibraryVersion entries are not identical"
        public const int ACTIVITYLIBRARY_NAME_DOES_NOT_MATCH_STOREACTIVITY_ID = 55098;
        public const string ACTIVITYLIBRARY_NAME_DOES_NOT_MATCH_STOREACTIVITY_MSG = "Bad activityLibraryName/activityLibraryVersion in StoreActivitiesDC - All activityLibraryName/activityLibraryVersion entries are not identical";

        //// 55100 "Invalid Parameter Value (@inCaller)"
        public const int INVALID_PARMETER_VALUE_INCALLER_ID = 55100;

        public const string INVALID_PARMETER_VALUE_INCALLER_MSG = "Invalid Parameter Value (@inCaller)";

        //// 55101 "Invalid Parameter Value (@inCallerversion)"
        public const int INVALID_PARMETER_VALUE_INCALLERVERSION_ID = 55101;
        public const string INVALID_PARMETER_VALUE_INCALLERVERSION_MSG = "Invalid Parameter Value (@inCallerVersion)";

        //// 55102 "Invalid Parameter Value (@InInsertedByUserAlias)"
        public const int INVALID_PARMETER_VALUE_ININSERTEDBYUSERALIAS_ID = 55102;
        public const string INVALID_PARMETER_VALUE_ININSERTEDBYUSERALIAS_MSG = "Invalid Parameter Value (@InInsertedByUserAlias)";

        //// 55103 "Invalid Parameter Value (@InUpdatedByUserAlias)"
        public const int INVALID_PARMETER_VALUE_INUPDATEDBYUSERALIAS_ID = 55103;
        public const string INVALID_PARMETER_VALUE_INUPDATEDBYUSERALIAS_MSG = "Invalid Parameter Value (@InUpdatedByUserAlias)";

        //// 55104 "Invalid Parameter Value (@InVersion)"
        public const int INVALID_PARMETER_VALUE_INVERSION_ID = 55104;
        public const string INVALID_PARMETER_VALUE_INVERSION_MSG = "Invalid Parameter Value (@InVersion)";

        //// 55105 "Invalid Parameter Value (@InGUID)"
        public const int INVALID_PARMETER_VALUE_INGUID_ID = 55105;
        public const string INVALID_PARMETER_VALUE_INGUID_MSG = "Invalid Parameter Value (@InGUID)";

        //// 55106 "Invalid Parameter Value (@InName)"
        public const int INVALID_PARMETER_VALUE_INNAME_ID = 55106;
        public const string INVALID_PARMETER_VALUE_INNAME_MSG = "Invalid Parameter Value (@InName)";

        //// 55107 "Invalid Parameter Value (@InDescription)"
        public const int INVALID_PARMETER_VALUE_INDESCRIPTION_ID = 55107;
        public const string INVALID_PARMETER_VALUE_INDESCRIPTION_MSG = "Invalid Parameter Value (@InDescription)";

        //// 55108 "Invalid Parameter Value (@InMetaTags)"
        public const int INVALID_PARMETER_VALUE_INMETATAGS_ID = 55108;
        public const string INVALID_PARMETER_VALUE_INMETATAGS_MSG = "Invalid Parameter Value (@InMetaTags)";

        //// 55109 "Invalid Parameter Value (@InIsSwitch)"
        public const int INVALID_PARMETER_VALUE_ISSWITCH_ID = 55109;
        public const string INVALID_PARMETER_VALUE_ISSWITCH_MSG = "Invalid Parameter Value (@InIsSwitch)";

        //// 55110 "Invalid Parameter Value (@InIsService)"
        public const int INVALID_PARMETER_VALUE_ISSERVICE_ID = 55110;
        public const string INVALID_PARMETER_VALUE_ISSERVICE_MSG = "Invalid Parameter Value (@InIsService)";

        //// 55111 "Invalid Parameter Value (@InActivityLibraryName)"
        public const int INVALID_PARMETER_VALUE_ACTIVITYLIBRARYNAME_ID = 55111;
        public const string INVALID_PARMETER_VALUE_ACTIVITYLIBRARYNAME_MSG = "Invalid Parameter Value (@InActivityLibraryName)";

        //// 55112 "Invalid Parameter Value (@InActivityLibraryVersion)"
        public const int INVALID_PARMETER_VALUE_ACTIVITYLIBRARYVERSION_ID = 55112;
        public const string INVALID_PARMETER_VALUE_ACTIVITYLIBRARYVERSION_MSG = "Invalid Parameter Value (@InActivityLibraryVersion)";

        //// 55113 "Invalid Parameter Value (@InIsUxActivity)"
        public const int INVALID_PARMETER_VALUE_ISUXACTIVITY_ID = 55113;
        public const string INVALID_PARMETER_VALUE_ISUXACTIVITY_MSG = "Invalid Parameter Value (@InIsUxActivity)";

        //// 55117 "Invalid Parameter Value (@InWorkflowTypeName)"
        public const int INVALID_PARMETER_VALUE_INWORKFLOWTYPENAME_ID = 55117;
        public const string INVALID_PARMETER_VALUE_INWORKFLOWTYPENAME_MSG = "Invalid Parameter Value (@InWorkflowTypeName)";

        //// 55118 "Invalid Parameter Value (@InAuthGroupName)"
        public const int INVALID_PARMETER_VALUE_INAUTHGROUPNAME_ID = 55118;
        public const string INVALID_PARMETER_VALUE_INAUTHGROUPNAME_MSG = "Invalid Parameter Value (@InAuthGroupName)";

        //// 55119 "Invalid Parameter Value (@InIconName) - not in [dbo].[ltblIcons]"
        public const int INVALID_PARMETER_VALUE_INICONNAME_ID = 55119;
        public const string INVALID_PARMETER_VALUE_INICONNAME_MSG = "Invalid Parameter Value (@InIconName)";

        //// 55120 "Invalid Parameter Value (@ToolBoxTabName) - not in [dbo].[ltblToolBoxTabName]"
        public const int INVALID_PARMETER_VALUE_INTOOLBOXTABNAME_ID = 55120;
        public const string INVALID_PARMETER_VALUE_INTOOLBOXTABNAME_MSG = "Invalid Parameter Value (@ToolBoxTabName)";

        //// 55121 "Invalid Parameter Value (@StatusID) - not in [dbo].[etblStatusCodes]"
        public const int INVALID_PARMETER_VALUE_INSTATUSID_ID = 55121;
        public const string INVALID_PARMETER_VALUE_INSTATUSID_MSG = "Invalid Parameter Value (@StatusID)";

        //// TODO fix duplicate
        //// 5122 "Invalid Parameter Value (@InIconName) - not in [dbo].[ltblIcons]"
        //// public const int INVALID_PARMETER_VALUE_InIconName_ID = 5122;
        //// public const string INVALID_PARMETER_VALUE_InIconName_MSG = "Invalid Parameter Value (@InIconName)";

        //// 55123 "Invalid Parameter Value (@InId)"
        public const int INVALID_PARMETER_VALUE_INID_ID = 55123;
        public const string INVALID_PARMETER_VALUE_INID_MSG = "Invalid Parameter Value (@InId)";

        //// 55124 "Invalid Parameter combination on get"
        public const int INVALID_PARMETER_COMBINATION_ON_GET_ID = 55124;
        public const string INVALID_PARMETER_COMBINATION_ON_GET_MSG = "Invalid Parameter combination on get";

        //// 55125 "Invalid Parameter Value (@InName/@inVersion)"
        public const int INVALID_PARMETER_VALUE_INNAMEINVERSION_ID = 55125;
        public const string INVALID_PARMETER_VALUE_INNAMEINVERSION_MSG = "Invalid Parameter Value (@InName/@inVersion)";

        //// 55126 "Invalid Parameter Value (@InStatusCodeName)"
        public const int INVALID_PARMETER_VALUE_INSTATUSCODENAME_ID = 55126;
        public const string INVALID_PARMETER_VALUE_INSTATUSCODENAME_MSG = "Invalid Parameter Value (@InStatusCodeName)";

        //// 55127 "Invalid Parameter Value (@InCode)"
        public const int INVALID_PARMETER_VALUE_INCODE_ID = 55127;
        public const string INVALID_PARMETER_VALUE_INCODE_MSG = "Invalid Parameter Value (@InCode)";

        //// 55128 "Invalid Parameter Value (@inActivityLibraryName)"
        public const int INVALID_PARMETER_VALUE_INACTIVITYLIBRARYNAME_ID = 55128;
        public const string INVALID_PARMETER_VALUE_INACTIVITYLIBRARYNAME_MSG = "Invalid Parameter Value (@inActivityLibraryName)";

        public const int UNKNOWN_ERROR_VALUE_129 = 55129; // was not defined in the calling code, but the error code is expected from the sproc

        //// 55130 "Invalid Parameter Value (@inActivityLibraryDependentName)"
        public const int INVALID_PARMETER_VALUE_INACTIVITYLIBRARYDEPEnDENTNAME_ID = 55130;
        public const string INVALID_PARMETER_VALUE_INACTIVITYLIBRARYDEPENDENTNAME_MSG = "Invalid Parameter Value (@inActivityLibraryDependentName)";

        //// 55131 "Invalid Parameter Value (@inActivityLibraryDependentVersionNumber)"
        public const int INVALID_PARMETER_VALUE_INACTIVITYLIBRARYDEPENDENTVERSIONNUMBER_ID = 55131;
        public const string INVALID_PARMETER_VALUE_INACTIVITYLIBRARYDEPENDENTVERSIONNUMBER_MSG = "Invalid Parameter Value (@inActivityLibraryDependentVersionNumber)";

        //// 55132 "Invalid Parameter Value (@inActivityLibraryName/@inActivityLibraryVersionNumber)"
        public const int INVALID_PARMETER_VALUE_INACTIVITYLIBRARYVERSIONNUMBER_ID = 55132; //// also 129
        public const string INVALID_PARMETER_VALUE_INACTIVITYLIBRARYVERSIONNUMBER_MSG = "Invalid Parameter Value (@inActivityLibraryVersionNumber)";

        //// 55134 "Invalid Parameter Value (@inShortName)"
        public const int INVALID_PARMETER_VALUE_INSHORTNAMENAME_ID = 55134;




        public const string INVALID_PARMETER_VALUE_INSHORTNAMENAME_MSG = "Invalid Parameter Value (@inShortName)";

        //// 55136 "Invalid Parameter Value (@InIsPickList)"
        public const int INVALID_PARMETER_VALUE_INISPICKLIST_ID = 55136;
        public const string INVALID_PARMETER_VALUE_INISPICKLIST_MSG = "Invalid Parameter Value (@InIsPickList)";

        //// 55137 "Invalid Parameter Value (@InValidationExecutable)"
        public const int INVALID_PARMETER_VALUE_INVALIDATIONEXECUTABLE_ID = 55137;
        public const string INVALID_PARMETER_VALUE_INVALIDATIONEXECUTABLE_MSG = "Invalid Parameter Value (@InValidationExecutable)";

        //// 55138 "Invalid Parameter Value (@InValidationMethod)"
        public const int INVALID_PARMETER_VALUE_INVALIDATIONMETHOD_ID = 55138;
        public const string INVALID_PARMETER_VALUE_INVALIDATIONMETHOD_MSG = "Invalid Parameter Value (@InValidationMethod)";

        ////TODO: RECONCILE THESE
        //// 5139 "Invalid Parameter Value (@InValidationMethod)"
        //// public const int INVALID_PARMETER_VALUE_InValidationMethod_ID = 5139;
        //// public const string INVALID_PARMETER_VALUE_InValidationMethod_MSG = "Invalid Parameter Value (@InValidationMethod)";

        //// 55140 "Invalid Parameter Value (@InBusinessImpactClassification)"
        public const int INVALID_PARMETER_VALUE_INBUSINESSIMPACTCLASSIFICATION_ID = 55140;
        public const string INVALID_PARMETER_VALUE_INBUSINESSIMPACTCLASSIFICATION_MSG = "Invalid Parameter Value (@InBusinessImpactClassification)";

        ////TODO: RECONCILE THESE
        //// 55150 "Invalid Parameter Value (@InId, @InName, @InVersion cannot all be null)"
        //// public const int INVALID_PARMETER_VALUE_InBusinessImpactClassification_ID = 55150;
        //// public const string INVALID_PARMETER_VALUE_InBusinessImpactClassification_MSG = "Invalid Parameter Value (@InBusinessImpactClassification)";

        //// 55151 "Invalid Parameter Value (@InPublishingWorkflow)"
        public const int INVALID_PARMETER_VALUE_InPublishingWorkflow_ID = 55151;
        public const string INVALID_PARMETER_VALUE_InPublishingWorkflow_MSG = "Invalid Parameter Value (@InPublishingWorkflow)";

        //// 55152 "Invalid Parameter Value (@InWorkflowTemplate)"
        public const int INVALID_PARMETER_VALUE_INWORKFLOWTEMPLATE_ID = 55152;
        public const string INVALID_PARMETER_VALUE_INWORKFLOWTEMPLATE_MSG = "Invalid Parameter Value (@InWorkflowTemplate)";

        //// 55154 "Invalid Parameter Value (@InHandleVariable)"
        public const int INVALID_PARMETER_VALUE_INHANDLEVARIABLE_ID = 55154;
        public const string INVALID_PARMETER_VALUE_INHANDLEVARIABLE_MSG = "Invalid Parameter Value (@InHandleVariable)";

        //// 55155 "Invalid Parameter Value (@InPageViewVariable)"
        public const int INVALID_PARMETER_VALUE_InPageViewVariable_ID = 55155;
        public const string INVALID_PARMETER_VALUE_InPageViewVariable_MSG = "Invalid Parameter Value (@InPageViewVariable)";

        //// 55156 "Invalid Parameter Value (@InAuthoringToolLevel)"
        public const int INVALID_PARMETER_VALUE_INAUTHORINGTOOLLEVEL_ID = 55156;
        public const string INVALID_PARMETER_VALUE_INAUTHORINGTOOLLEVEL_MSG = "Invalid Parameter Value (@InAuthoringToolLevel)";

        //// 55157 "Invalid Parameter Value (@InId, @InName, @inGuid cannot all be null)"
        public const int INVALID_PARMETER_VALUE_INIDINNAMEINGUIDCANNOTALLBENULL_ID = 55157;
        public const string INVALID_PARMETER_VALUE_INIDINNAMEINGUIDCANNOTALLBENULL_MSG = "Invalid Parameter Value (@InId, @InName, @inGuid cannot all be null)";

        //// 55158 "Invalid Parameter Value (@InId, and @InName" cannot all be null)"
        public const int INVALID_PARMETER_VALUE_INIDINNAMECANNOTBENULL_ID = 55158;
        public const string INVALID_PARMETER_VALUE_INIDINNAMECANNOTBENULL_MSG = "Invalid Parameter Value (@InId, and @InName cannot be null)";

        //// 55159 "Invalid Parameter Value (@InShowInProduction)"
        public const int INVALID_PARMETER_VALUE_INSHOWINPRODUCTION_ID = 55159;
        public const string INVALID_PARMETER_VALUE_INSHOWINPRODUCTION_MSG = "Invalid Parameter Value (@InShowInProduction)";

        //// 55160 "Invalid Parameter Value (@InLockForChanges)"
        public const int INVALID_PARMETER_VALUE_INLOCKFORCHANGES_ID = 55160;
        public const string INVALID_PARMETER_VALUE_INLOCKFORCHANGES_MSG = "Invalid Parameter Value (@InLockForChanges)";

        //// 55161 "Invalid Parameter Value (@InId, @InName", and @InShortName cannot all be null)"
        public const int INVALID_PARMETER_VALUE_INIDINNAMEINSHORTNAMECANNOTBENULL_ID = 55161;
        public const string INVALID_PARMETER_VALUE_INIDINNAMEINSHORTNAMECANNOTBENULL_MSG = "Invalid Parameter Value (@InId, @InName, and @InShortName cannot all be null)";

        //// 55162 "Invalid Parameter Value combination (@inActivityLibraryName, @inActivityLibraryVersionNumber", @inActivityLibraryDependentName and @inActivityLibraryDependentVersionNumber are an invalid combination)"
        public const int INVALID_PARMETER_VALUE_LIBRARYNAMEANDVERSIONINVALID_ID = 55162;
        public const string INVALID_PARMETER_VALUE_LIBRARYNAMEANDVERSIONINVALID_MSG = "Invalid Parameter Value combination (@inActivityLibraryName, @inActivityLibraryVersionNumber, @inActivityLibraryDependentName and @inActivityLibraryDependentVersionNumber are an invalid combination)";

        //// 55163 "Invalid Parameter Value (@InCategoryName)"
        public const int INVALID_PARMETER_VALUE_INCATEGORYNAME_ID = 55163;
        public const string INVALID_PARMETER_VALUE_INCATEGORYNAM_MSG = "Invalid Parameter Value (@InCategoryName)";

        //// 55164 "Invalid Parameter Value (@InTreeNode)"
        public const int INVALID_PARMETER_VALUE_INTREENODE_ID = 55164;
        public const string INVALID_PARMETER_VALUE_INTREENODE_MSG = "Invalid Parameter Value (@InTreeNode)";

        //// 55165 "Invalid ActivityLibraries dependency"
        public const int INVALID_PARMETER_VALUE_ACTIVITYLIBRARYDEPENDENCY_ID = 56165;
        public const string INVALID_PARMETER_VALUE_ACTIVITYLIBRARYDEPENDENCY_MSG = "Invalid ActivityLibraries dependency";

        //// 55166 "Invalid StoreActivities name for publishingStoreActivityName dependency"
        public const int INVALID_PARMETER_VALUE_STOREACTIVITIESPUBLISHINGNAME_ID = 55166;
        public const string INVALID_PARMETER_VALUE_STOREACTIVITIESPUBLISHINGNAME_MSG = "Invalid StoreActivities name/Version for publishingStoreActivityName dependency";

        //// 55167 "PublishingStoreActivityName is not a publishing workflow"
        public const int INVALID_PARMETER_VALUE_NOTAPUBLISHINGWORKFLOW_ID = 55167;
        public const string INVALID_PARMETER_VALUE_NOTAPUBLISHINGWORKFLOW_MSG = "PublishingStoreActivityName is not a publishing workflow";

        //// 55168 "Invalid Parameter Value (@InPublishingWorkflow) - Does not exist in SoreActivities table"
        public const int INVALID_PARMETER_VALUE_PUBLISHINGWORKFLOWDOESNTEXIST_ID = 55168;
        public const string INVALID_PARMETER_VALUE_PUBLISHINGWORKFLOWDOESNTEXIST_MSG = "Invalid Parameter Value (@InPublishingWorkflow) - Does not exist in SoreActivities table";

        //// 55169 "Invalid Parameter Value (@InWorkflowTemplate) - Does not exist in SoreActivities table"
        public const int INVALID_PARMETER_VALUE_TEMPLATEWORKFLOWDOESNTEXIST_ID = 55169;
        public const string INVALID_PARMETER_VALUE_TEMPLATEWORKFLOWDOESNTEXIST_MSG = "Invalid Parameter Value (@InWorkflowTemplate) - Does not exist in SoreActivities table";

        //// 55170 "Invalid Parameter Value (@InPublishingWorkflow) - Null or empty string"
        public const int INVALID_PARMETER_VALUE_INPUBLISHINGWORKFLOW_ID = 55170;
        public const string INVALID_PARMETER_VALUE_INPUBLISHINGWORKFLOW_MSG = "Invalid Parameter Value (@InPublishingWorkflow) - Null or empty string";

        //// 55171 "Invalid Parameter Value (@InNameHead1)"
        public const int INVALID_PARMETER_VALUE_INNAMEHEAD1_ID = 55171;
        public const string INVALID_PARMETER_VALUE_INNAMEHEAD1_MSG = "Invalid Parameter Value (@InNameHead1)";

        //// 55172 "Invalid Parameter Value (@InVersionHead1)"
        public const int INVALID_PARMETER_VALUE_INVERSIONHEAD1_ID = 55172;
        public const string INVALID_PARMETER_VALUE_INVERSIONHEAD1_MSG = "Invalid Parameter Value (@InVersionHead1)";

        //// 55173 "Invalid Parameter Value (@InNameHead2)"
        public const int INVALID_PARMETER_VALUE_INNAMEHEAD2_ID = 55173;
        public const string INVALID_PARMETER_VALUE_INNAMEHEAD2_MSG = "Invalid Parameter Value (@InNameHead2)";

        //// 55174 "Invalid Parameter Value (@InVersionHead2)"
        public const int INVALID_PARMETER_VALUE_INVERSIONHEAD2_ID = 55174;
        public const string INVALID_PARMETER_VALUE_INVERSIONHEAD2_MSG = "Invalid Parameter Value (@InVersionHead2)";

        //// 55175 "Store Activity Specified by (@InName/@inVersion) does not have publishing workflow assigned"
        public const int INVALID_PARMETER_VALUE_NOPUBLISHINGWORKFLOWASSIGNED_ID = 55175;
        public const string INVALID_PARMETER_VALUE_NOPUBLISHINGWORKFLOWASSIGNED_MSG = "Store Activity Specified by (@InName/@inVersion) does not have publishing workflow assigned";

        //// 55176 "Workflow type does not have publishing workflow assigned"
        public const int INVALID_PARMETER_VALUE_NOPUBLISHINGWORKFLOWASSIGNEDTOTYPE_ID = 55176;
        public const string INVALID_PARMETER_VALUE_NOPUBLISHINGWORKFLOWASSIGNEDTOTYPE_MSG = "Workflow type does not have publishing workflow assigned";

        //// 55177 "Did not find a work flow with the specified name/version"
        public const int INVALID_PARMETER_VALUE_NOWORKFLOWWITHNAMEVERSION_ID = 55177;
        public const string INVALID_PARMETER_VALUE_NOWORKFLOWWITHNAMEVERSIONE_MSG = "Did not find a work flow with the specified name/version";

        //// 55178 "Did not find a compiled work flow"
        public const int INVALID_PARMETER_VALUE_NOCOMPILEDWORKFLOWAVAILABLE_ID = 55178;
        public const string INVALID_PARMETER_VALUE_NOCOMPILEDWORKFLOWAVAILABLE_MSG = "Did not find a compiled work flow";

        //// 55179 "Publishing Write Error"
        public const int INVALID_PARMETER_VALUE_PUBLISHINGWRITEERROR_ID = 55179;
        public const string INVALID_PARMETER_VALUE_PUBLISHINGWRITEERROR_MSG = "Publishing Write Error";

        //// 55180 "Security Error"
        public const int INVALID_CREDENTIALS = 55180;
        public const string INVALID_CREDENTIALS_MSG = "Access is denied for user {0}. ";

        // Activities Error messages 
        public const string ACTIVITY_LIBRARY_DEPENDENCIES_LIST_HEAD_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.ActivityLibraryDependenciesListHeadCreateOrUpdate.ActivityLibraryDependency_CreateOrUpdateListHead";
        public const string PUBLISH_WORKFLOW_BY_WORKFLOW_TYPE_GET_CALL_ERROR_MSG = "DAL.GetPublishingWorkFlowByWorkFlowType.PublishWorkFlow_GetByWorkFlowType";
        public const string PUBLISH_WORKFLOW_BY_NAME_VERSION_GET_CALL_ERROR_MSG = "DAL.PublishWorkflowByNameVersionGet.PublishWorkflow_GetByNameVersion";
        public const string ACTIVITY_LIBRARY_DEPENDENCIES_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.StoreActivityLibraryDependenciesCreateOrUpdate.ActivityLibraryDependency_CreateOrUpdate";
        public const string ACTIVITY_LIBRARY_DEPENDENCIES_DELETE_CALL_ERROR_MSG = " DAL.StoreActivityLibraryDependenciesDelete.ActivityLibraryDependency_Delete";
        public const string STORE_ACTIVITIES_BY_CATEGORY_NAME_WO_EXECUTABLE_GET_CALL_ERROR_MSG = "DAL.StoreActivitiesByCategoryNameWoExecutableGet.Activity_GetByCategoryNameWithoutExecutable";
        public const string STORE_ACTIVITIES_BY_NAME_AND_VERSION_DELETE_CALL_ERROR_MSG = "DAL.StoreActivitiesDelete.Activity_Delete call";
        public const string ACTIVITY_CATEGORY_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.ActivityCategoryCreateOrUpdate.ActivityCategory_CreateOrUpdate";
        public const string ACTIVITY_CATEGORY_DELETE_CALL_ERROR_MSG = "DAL.ActivityCategoryDelete.ActivityCategory_Delete";
        public const string ACTIVITY_LIBRARY_SPECIFIC_EXISTS_CALL_ERROR_MSG = " DAL.ActivityLibrarySpecificExists.ActivityLibrary_Exists";
        public const string ACTIVITY_LIBRARY_GET_CALL_ERROR_MSG = "DAL.ActivityLibrariesGet.ActivityLibrary_Get";
        public const string ACTIVITY_LIBRARY_WO_EXECUTABLE_GET_CALL_ERROR_MSG = "DAL.ActivityLibrariesWoExecutableGet.ActivityLibrary_GetWithoutExecutable";
        public const string ACTIVITY_LIBRARY_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.ActivityLibraryCreateOrUpdate.ActivityLibrary_CreateOrUpdate";
        public const string ACTIVITY_LIBRARIES_DELETE_CALL_ERROR_MSG = "DAL.ActivityLibraryDelete.ActivityLibrary_Delete";
        public const string ACTIVITY_LIBRARIES_GET_CALL_ERROR_MSG = "DAL.ActivityLibraryGet.ActivityLibrary_Get";
        public const string APPLICATIONS_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.ApplicationsCreateOrUpdate.Application_CreateOrUpdate";
        public const string APPLICATIONS_DELETE_CALL_ERROR_MSG = "DAL.ApplicationsDelete.Application_Delete";
        public const string APPLICATIONS_GET_CALL_ERROR_MSG = " DAL.ApplicationsGet.Application_Get";
        public const string AUTHGROUPS_DELETE_CALL_ERROR_MSG = "DAL.AuthGroupsDelete.AuthorizationGroup_Delete";
        public const string AUTHGROUPS_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.AuthGroupsCreateOrUpdate.AuthorizationGroup_CreateOrUpdate";
        public const string AUTHGROUPS_GET_CALL_ERROR_MSG = "DAL.AuthGroupsGet.AuthorizationGroup_Get";
        public const string STATUS_CODE_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.StatusCodesCreateOrUpdate.StatusCode_CreateOrUpdate";
        public const string STATUS_CODE_DELETE_CALL_ERROR_MSG = "DAL.StatusCodeDelete.StatusCode_Delete";
        public const string STATUS_CODE_GET_CALL_ERROR_MSG = "DAL.StatusCodeGet.StatusCode_Get";
        public const string STORE_ACTIVITIES_DELETE_CALL_ERROR_MSG = "DAL.StoreActivitiesDelete.Activity_Delete";
        public const string STORE_ACTIVITIES_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.StoreActivitiesCreateOrUpdate.Activity_CreateOrUpdate";        
        public const string STORE_ACTIVITIES_GET_CALL_ERROR_MSG = " DAL.StoreActivitiesGet.Activity_Get";
        public const string TOOLBOX_TAB_NAME_GET_CALL_ERROR_MSG = "DAL.ToolboxTabNameGet.ToolboxTabName_Get";
        public const string TOOLBOX_TAB_NAME_CREATE_OR_UPDATE_CALL_ERROR_MSG = "DAL.ToolBoxTabNameCreateOrUpdate.ToolBoxTabName_CreateOrUpdate";
        public const string TOOLBOX_TAB_NAME_DELETE_CALL_ERROR_MSG = "DAL.ToolBoxTabNameDelete.ToolBoxTabName_Delete";
        public const string WORKFLOW_TYPE_CREATE_OR_UPDATE_CALL_ERROR_MSG = " DAL.WorkflowTypeCreateOrUpdate.WorkflowType_CreateOrUpdate";

        public const string VersionIncorrectFaultReasonMessage = "Version number is incorrect";
    }
}


//-----------------------------------------------------------------------
// <copyright file="ApplicationsGetRequestDC.cs" company="Microsoft">
// Copyright
// Applications Get Request DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    using Microsoft.Practices.EnterpriseLibrary.Validation;
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

    /// <summary>
    /// Request data contract for ApplicationsGet
    /// </summary>
    [DataContract]
    [HasSelfValidation]
    public class ApplicationsGetRequestDC : RequestHeader
    {
        private int inId;
        private string inName;

        /// <summary>
        /// PK of the application
        /// </summary>
        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        /// <summary>
        /// Name of the application
        /// </summary>
        [DataMember]
        public string InName
        {
            get { return inName; }
            set { inName = value; }
        }

        // Task 20943
        /// <summary>
        /// Validates that the GetRequestObject was set correctly
        /// </summary>
        /// <param name="results">Represents the results of validating the object</param>
        //[SelfValidation]
        //public void CheckApplicationsGetRequestObject(ValidationResults results)
        //{
        //    if (inId < 0)
        //    {
        //        // TODO Eric results.AddResult(new ValidationResult(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, this, null, null, null));
        //    }
        //}
    }
}

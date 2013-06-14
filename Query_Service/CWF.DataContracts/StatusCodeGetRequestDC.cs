//-----------------------------------------------------------------------
// <copyright file="StatusCodeGetRequestDC.cs" company="Microsoft">
// Copyright
// StatusCode Get Request DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    using Microsoft.Practices.EnterpriseLibrary.Validation;
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

    /// <summary>
    /// Request data contract for StatusCodeGet
    /// </summary>
    [DataContract]
    [HasSelfValidation]
    public class StatusCodeGetRequestDC : RequestHeader
    {
        private int inCode;
        private string inName = string.Empty;

        /// <summary>
        /// Status code
        /// </summary>
        [DataMember]
        public int InCode
        {
            get { return inCode; }
            set { inCode = value; }
        }

        /// <summary>
        /// Status code name
        /// </summary>
        [DataMember]
        public string InName
        {
            get { return inName; }
            set { inName = value; }
        }

        /// <summary>
        /// Validates that the GetRequestObject was set correctly
        /// </summary>
        /// <param name="results">Represents the results of validating the object</param>
        //[SelfValidation]
        //public void CheckStatusCodeGetRequestObject(ValidationResults results)
        //{
        //    if (inCode < 0)
        //    {
        //        // TODO Eric results.AddResult(new ValidationResult(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_InCode_MSG, this, null, null, null));
        //    }
        //}
    }
}

//-----------------------------------------------------------------------
// <copyright file="ActivityCategoryByNameGetRequestDC.cs" company="Microsoft">
// Copyright
// ActivityCategory B yName Get Request DC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DataContracts
{
    using System.Runtime.Serialization;

    using Microsoft.Practices.EnterpriseLibrary.Validation;
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

    /// <summary>
    /// Request data contract for ActivityCategoryByNameGet
    /// </summary>
    [DataContract]
    [HasSelfValidation]
    public class ActivityCategoryByNameGetRequestDC : RequestHeader
    {
        private string inName;
        private int inId;

        /// <summary>
        /// PK of the ActivityCategory
        /// </summary>
        [DataMember]
        public int InId
        {
            get { return inId; }
            set { inId = value; }
        }

        /// <summary>
        /// Name of the ActivityCategory
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
        //public void CheckActivityCategoryGetRequestObject(ValidationResults results)
        //{
        //    if (inId < 0)
        //    {
        //        // TODO Eric results.AddResult(new ValidationResult(CWF.Constants.SprocValues.INVALID_PARMETER_VALUE_INID_MSG, this, null, null, null));
        //    }
        //}
    }
}

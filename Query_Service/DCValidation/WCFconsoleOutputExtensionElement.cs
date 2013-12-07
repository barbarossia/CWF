//-----------------------------------------------------------------------
// <copyright file="WCFconsoleOutputExtensionElement.cs" company="Microsoft">
// Copyright
// WCF pipeline POC
// </copyright>
//-----------------------------------------------------------------------

namespace CWF.DCValidation
{
    using System;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// Console extension element for console output in WCF pipeline POC
    /// </summary>
    public class CWFconsoleOutputExtensionElement : BehaviorExtensionElement
    {
        /// <summary>
        /// BehaviorType property
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(WCFconsoleOutputBehavior);
            }
        }

        /// <summary>
        /// Creates a pipeline behaviour
        /// </summary>
        /// <returns>WCFconsoleOutputBehavior object</returns>
        protected override object CreateBehavior()
        {
            return new WCFconsoleOutputBehavior();
        }
    }
}

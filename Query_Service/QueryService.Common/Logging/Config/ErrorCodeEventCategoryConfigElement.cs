using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines the configuration element that maps to an entry inside "error code - event category configuration" XML node.
    /// </summary>
    public class ErrorCodeEventCategoryConfigElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [ConfigurationProperty(ErrorCodeEventCategoryConfigConstant.AttributeName.ErrorCode, IsKey = true, IsRequired = true)]
        public int ErrorCode
        {
            get
            {
                return (int)this[ErrorCodeEventCategoryConfigConstant.AttributeName.ErrorCode];
            }
            set
            {
                this[ErrorCodeEventCategoryConfigConstant.AttributeName.ErrorCode] = value;
            }

        }

        /// <summary>
        /// Gets or sets the category associated with an error code.
        /// </summary>
        [ConfigurationProperty(ErrorCodeEventCategoryConfigConstant.AttributeName.EventCategory, IsRequired=true)]
        public EventCategory Category
        {
            get
            {
                return (EventCategory)this[ErrorCodeEventCategoryConfigConstant.AttributeName.EventCategory];
            }
            set
            {
                this[ErrorCodeEventCategoryConfigConstant.AttributeName.EventCategory] = value;
            }
        }

        /// <summary>
        /// Gets or sets the severity associated with an error code.
        /// Possible values = "error"|"warning"|"information".
        /// </summary>
        [ConfigurationProperty(ErrorCodeEventCategoryConfigConstant.AttributeName.Severity, IsRequired = false)]
        public string Severity
        {
            get
            {
                return this[ErrorCodeEventCategoryConfigConstant.AttributeName.Severity] as string;
            }
            set
            {
                this[ErrorCodeEventCategoryConfigConstant.AttributeName.Severity] = value;
            }
        }

        /// <summary>
        /// Gets or sets the description associated with an error code.
        /// </summary>
        [ConfigurationProperty(ErrorCodeEventCategoryConfigConstant.AttributeName.Description, IsKey = true, IsRequired = false)]
        public string Description
        {
            get
            {
                return this[ErrorCodeEventCategoryConfigConstant.AttributeName.Description] as string;
            }
            set
            {
                this[ErrorCodeEventCategoryConfigConstant.AttributeName.Description] = value;
            }

        }
    }
}

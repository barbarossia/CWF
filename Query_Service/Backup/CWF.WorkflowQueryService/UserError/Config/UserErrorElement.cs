using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CWF.WorkflowQueryService.UserError.Config
{
    /// <summary>
    /// Custom configuration element class to represent the error code to user error message map.
    /// </summary>
    public class UserErrorElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [ConfigurationProperty(UserErrorConfigConstant.AttributeName.ErrorCode, IsKey = true, IsRequired = true)]
        public int ErrorCode
        {
            get
            {
                return (int)this[UserErrorConfigConstant.AttributeName.ErrorCode];
            }
            set
            {
                this[UserErrorConfigConstant.AttributeName.ErrorCode] = value;
            }
        }

        /// <summary>
        /// Gets or sets the user error message.
        /// </summary>
        [ConfigurationProperty(UserErrorConfigConstant.AttributeName.UserMessage, IsRequired = true)]
        public string UserMessage
        {
            get
            {
                return (string)this[UserErrorConfigConstant.AttributeName.UserMessage];
            }
            set
            {
                this[UserErrorConfigConstant.AttributeName.UserMessage] = value;
            }
        }

        /// <summary>
        /// Gets or sets the fault type associated with the error code.
        /// </summary>
        [ConfigurationProperty(UserErrorConfigConstant.AttributeName.FaultType, IsRequired = true)]
        public string FaultType
        {
            get
            {
                return (string)this[UserErrorConfigConstant.AttributeName.FaultType];
            }
            set
            {
                this[UserErrorConfigConstant.AttributeName.FaultType] = value;
            }
        }       
    }
}
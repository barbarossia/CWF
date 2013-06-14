using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines the configuration element that maps to an entry inside log settings configuration XML node.
    /// </summary>
    public class LogSettingConfigElement :ConfigurationElement
    {
        /// <summary>
        /// 
        /// Gets or sets the key of logging configuration item.
        /// </summary>
        [ConfigurationProperty(LogSettingConfigConstant.AttributeName.Key, IsKey = true, IsRequired = true)]
        public string Key
        {
            get
            {
                return (string)this[LogSettingConfigConstant.AttributeName.Key];
            }
            set
            {
                this[LogSettingConfigConstant.AttributeName.Key] = value;
            }
        }

        /// <summary>
        /// Gets or sets value of logging configuration item.
        /// </summary>
        [ConfigurationProperty(LogSettingConfigConstant.AttributeName.Value, IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)this[LogSettingConfigConstant.AttributeName.Value];
            }
            set
            {
                this[LogSettingConfigConstant.AttributeName.Value] = value;
            }
        }
    }
}
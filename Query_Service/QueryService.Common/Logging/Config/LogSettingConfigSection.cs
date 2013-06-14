using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines the configuration section that maps to log setting configuration XML node.
    /// </summary>
    public class LogSettingConfigSection : ConfigurationSection
    {
        /// <summary>
        /// Gets an instance of this configuration section.
        /// </summary>
        private static LogSettingConfigSection _current = ConfigurationManager.GetSection(LogSettingConfigConstant.NodeName.LoggingConfiguration) as LogSettingConfigSection;

        /// <summary>
        /// Gets the single instance of the in-memory representation of log setting configuration XML node.
        /// </summary>
        public static LogSettingConfigSection Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Gets the settings collection of log setting configuration.
        /// </summary>
        [ConfigurationProperty(LogSettingConfigConstant.NodeName.Settings)]
        [ConfigurationCollection(typeof(LogSettingConfigCollection))]
        public LogSettingConfigCollection Settings
        {
            get { return this[LogSettingConfigConstant.NodeName.Settings] as LogSettingConfigCollection; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Defines the configuration section that maps to "error code - event category configuration" XML node.
    /// </summary>
    public class ErrorCodeEventCategoryConfigSection : ConfigurationSection
    {
        /// <summary>
        /// Gets an instance of this configuration section.
        /// </summary>
        private static ErrorCodeEventCategoryConfigSection _current = ConfigurationManager.GetSection(ErrorCodeEventCategoryConfigConstant.NodeName.ErrorCodeEventCategoryConfiguration) as ErrorCodeEventCategoryConfigSection;

        /// <summary>
        /// Gets the single instance of the in-memory representation of "error code - event category configuration" XML node.
        /// </summary>
        public static ErrorCodeEventCategoryConfigSection Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Gets the error code mapping collection of "error code - event category configuration".
        /// </summary>
        [ConfigurationProperty(ErrorCodeEventCategoryConfigConstant.NodeName.ErrorCodes)]
        [ConfigurationCollection(typeof(ErrorCodeEventCategoryConfigCollection))]
        public ErrorCodeEventCategoryConfigCollection ErrorCodes
        {
            get { return this[ErrorCodeEventCategoryConfigConstant.NodeName.ErrorCodes] as ErrorCodeEventCategoryConfigCollection; }
        }
    }
}
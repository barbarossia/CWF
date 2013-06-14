using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Custom configuration collection class to represent log settings.
    /// </summary>
    public class LogSettingConfigCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public LogSettingConfigCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// Creates a new configuration Element.
        /// </summary>
        /// <returns>Returns a new configuration element of type LogSettingElement.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogSettingConfigElement();
        }

        /// <summary>
        /// Gets the element key for the configuration element.
        /// </summary>
        /// <param name="element">ConfigurationElement object.</param>
        /// <returns>The key with which the element is identified.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogSettingConfigElement)element).Key;
        }

        /// <summary>
        /// Gets or sets the LogSettingElement.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>An instance of LogSettingElement identified by key.</returns>
        public new LogSettingConfigElement this[string key]
        {
            get
            {
                return BaseGet((object)key) as LogSettingConfigElement;
            }
            set
            {
                if (BaseGet((object)key) == null)
                {
                    BaseAdd(value);
                }
            }
        }
    }
}

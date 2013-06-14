using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Microsoft.Support.Workflow.Service.Common.Logging.Config
{
    /// <summary>
    /// Custom configuration collection class to represent the error codes to event category mapping.
    /// </summary>
    public class ErrorCodeEventCategoryConfigCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ErrorCodeEventCategoryConfigCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {

        }

        /// <summary>
        /// Gets the type of collection.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// Creates a new configuration Element of ErrorCodeElement.
        /// </summary>
        /// <returns>Returns a new configuration Element of ErrorCodeElement.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ErrorCodeEventCategoryConfigElement();
        }

        /// <summary>
        /// Gets the element key for ErrorCodeElement.
        /// </summary>
        /// <param name="element">ConfigurationElement object.</param>
        /// <returns>Returns the key with which the element is identified.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ErrorCodeEventCategoryConfigElement)element).ErrorCode;
        }

        /// <summary>
        /// Gets or sets the ErrorCodeElement.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <returns>Returns an instance of ErrorCodeElement specified by the ErrorCode key.</returns>
        public ErrorCodeEventCategoryConfigElement this[int code]
        {
            get { return BaseGet((object)code) as ErrorCodeEventCategoryConfigElement; }
            set
            {
                if (BaseGet((object)code) == null)
                {
                    BaseAdd(value);
                }
            }
        }
    }
}

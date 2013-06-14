using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Service.Common.Logging.Config;
using Microsoft.Support.Workflow.Service.Common.Logging;

namespace Microsoft.Support.Workflow.Service.Test.Common
{


    /// <summary>
    /// Defines dynamic implementations to isolate the reading of LogSetting.config and to dynamically
    /// provide different configuration values to verify the behavior of code that reads from this configuration.
    /// </summary>
    public static class LogSettingConfigIsolator
    {
        private const string ConfigurationSectionGetSettingsMethod = "get_Settings";

        /// <summary>
        /// Gets a dynamic implementation that simulates empty log settings configuration.
        /// </summary>
        /// <returns>Dynamic implementation of type LogSettingConfigSection for empty log setting.</returns>
        public static ImplementationOfType GetEmptyLogSettingConfigurationMock()
        {
            ImplementationOfType impl = new ImplementationOfType(typeof(LogSettingConfigSection));
            impl.Register(typeof(LogSettingConfigSection).GetMethod(ConfigurationSectionGetSettingsMethod), null, null, null, RegistrationOptions.None,
                            delegate
                            {
                                LogSettingConfigCollection collection = new LogSettingConfigCollection();
                                return collection;
                            });
            return impl;
        }

         /// <summary>
        /// Gets a dynamic implementation that simulates a valid log settings configuration.
        /// </summary>
        /// <returns>Dynamic implementation of type LogSettingConfigSection for valid log setting.</returns>
        public static ImplementationOfType GetValidLogSettingConfigurationMock()
        {
            ImplementationOfType impl = new ImplementationOfType(typeof(LogSettingConfigSection));
            impl.Register(typeof(LogSettingConfigSection).GetMethod(ConfigurationSectionGetSettingsMethod), null, null, null, RegistrationOptions.None,
                            delegate
                            {
                               LogSettingConfigCollection collection = new LogSettingConfigCollection();
                                collection[LogSettingKey.LogWriter] = new LogSettingConfigElement { Key = LogSettingKey.LogWriter, Value = "Microsoft.Support.Workflow.Service.Common.Logging.EventLogWriter,QueryService.Common" };
                                collection[LogSettingKey.LogName] = new LogSettingConfigElement { Key = LogSettingKey.LogName, Value = LogSettingDefaultValue.LogName };
                                collection[LogSettingKey.GetLogLevelKey("default")] = new LogSettingConfigElement { Key = "default", Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.DatabaseError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey("default"), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.DataAccessLayerError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.DataAccessLayerError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.WebServiceLayerError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.WebServiceLayerError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.LogWriterError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.LogWriterError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerValidation.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerValidation.ToString().ToLower()), Value = "-1" };

                                return collection;
                            });
            return impl;
        }

        public static ImplementationOfType GetValidLogSettingConfigurationInstance()
        {
            ImplementationOfType impl = new ImplementationOfType(typeof(LogSettingConfigSection));
            LogSettingConfigSection log = new LogSettingConfigSection();
            impl.Register(() => LogSettingConfigSection.Current)
                .Return(log);
            impl.Register(typeof(LogSettingConfigSection).GetMethod(ConfigurationSectionGetSettingsMethod), null, null, null, RegistrationOptions.None,
                            delegate
                            {
                                LogSettingConfigCollection collection = new LogSettingConfigCollection();
                                collection[LogSettingKey.LogWriter] = new LogSettingConfigElement { Key = LogSettingKey.LogWriter, Value = "Microsoft.Support.Workflow.Service.Common.Logging.EventLogWriter,QueryService.Common" };
                                collection[LogSettingKey.LogName] = new LogSettingConfigElement { Key = LogSettingKey.LogName, Value = LogSettingDefaultValue.LogName };
                                collection[LogSettingKey.GetLogLevelKey("default")] = new LogSettingConfigElement { Key = "default", Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.DatabaseError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey("default"), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.DataAccessLayerError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.DataAccessLayerError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.WebServiceLayerError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.WebServiceLayerError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.LogWriterError.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.LogWriterError.ToString().ToLower()), Value = "2" };
                                collection[LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerValidation.ToString().ToLower())] = new LogSettingConfigElement { Key = LogSettingKey.GetLogLevelKey(EventSource.BusinessLayerValidation.ToString().ToLower()), Value = "-1" };

                                return collection;
                            });
            return impl;
        }
    }
}

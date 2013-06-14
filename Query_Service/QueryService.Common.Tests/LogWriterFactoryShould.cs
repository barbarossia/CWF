using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Service.Common.Logging;
using Microsoft.Support.Workflow.Service.Common.Logging.Config;
using Microsoft.Support.Workflow.Service.Test.Common;

namespace Microsoft.Support.Workflow.Service.Common.Tests
{
    /// <summary>
    /// Defines unit tests for LogWriterFactory class.
    /// </summary>
    [TestClass]
    public class LogWriterFactoryShould
    {

        private const string LogSettingsConfigSectionGetSettingsMethodName = "get_Settings";
        private TestContext testContextInstance;
        
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Used to initalize Variation test excution object
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Utility.CopyTestConfigs(Utility.TempLocation);
            Utility.CopyTestConfigs(testContextInstance.TestDeploymentDir);
        }

        [Description("Return log writer instance of configured type when get_Instance is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnLogWriterInstanceOfConfiguredTypeWhenGetInstanceIsCalled()
        {
            using (var isolator = new InstanceMethodCallIsolator<LogSettingConfigSection>(LogSettingsConfigSectionGetSettingsMethodName,
                delegate
                {
                    // Simulate LogWriter="Microsoft.Support.Workflow.Service.Common.Logging.EventLogWriter,QueryService.Common" value.
                    LogSettingConfigCollection collection = new LogSettingConfigCollection();
                    collection[LogSettingKey.LogWriter] = new LogSettingConfigElement { Key = LogSettingKey.LogWriter, Value = "Microsoft.Support.Workflow.Service.Common.Logging.EventLogWriter,QueryService.Common" };
                    collection[LogSettingKey.LogName] = new LogSettingConfigElement { Key = LogSettingKey.LogName, Value = LogSettingDefaultValue.LogName };
                    return collection;
                }))
            {
                ILogWriter writer = LogWriterFactory.LogWriter;
                Assert.IsTrue(writer is EventLogWriter);
            }
        }

        [Description("Return null if configured LogWriter value is empty when get_Instance is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnNullIfLogWriterConfigValueIsEmptyWhenGetInstanceIsCalled()
        {
            using (var isolator = new InstanceMethodCallIsolator<LogSettingConfigSection>(LogSettingsConfigSectionGetSettingsMethodName,
                delegate
                {
                    // Simulate LogWriter="" value.
                    LogSettingConfigCollection collection = new LogSettingConfigCollection();
                    collection[LogSettingKey.LogWriter] = new LogSettingConfigElement { Key = LogSettingKey.LogWriter, Value = String.Empty };
                    collection[LogSettingKey.LogName] = new LogSettingConfigElement { Key = LogSettingKey.LogName, Value = LogSettingDefaultValue.LogName };
                    return collection;
                }))
            {
                ILogWriter writer = LogWriterFactory.LogWriter;
                Assert.IsNull(writer);
            }
        }

        [Description("Return null if configured LogWriter value is invalid when get_Instance is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnNullIfLogWriterConfigValueIsNonExistingClassWhenGetInstanceIsCalled()
        {
            using (var isolator = new InstanceMethodCallIsolator<LogSettingConfigSection>(LogSettingsConfigSectionGetSettingsMethodName,
                delegate
                {
                    //Simulate a LogWriter value properly defined for a non-existing type.
                    LogSettingConfigCollection collection = new LogSettingConfigCollection();
                    collection[LogSettingKey.LogWriter] = new LogSettingConfigElement { Key = LogSettingKey.LogWriter, Value = "Microsoft.Support.Workflow.Service.Common.Logging.EventLogWriter2,QueryService.Common" };
                    collection[LogSettingKey.LogName] = new LogSettingConfigElement { Key = LogSettingKey.LogName, Value = LogSettingDefaultValue.LogName };
                    return collection;
                }))
            {
                ILogWriter writer = LogWriterFactory.LogWriter;
                Assert.IsNull(writer);
            }
        }

        [Description("Return null if configured LogWriter is not ILogWriter type when get_Instance is called.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnNullIfLogWriterConfigValueIsNotILogWriterWhenGetInstanceIsCalled()
        {
            using (var isolator = new InstanceMethodCallIsolator<LogSettingConfigSection>(LogSettingsConfigSectionGetSettingsMethodName,
                delegate
                {
                    // Simulate a log writer value properly defined for an existing type that does not implement ILogWriter.
                    LogSettingConfigCollection collection = new LogSettingConfigCollection();
                    collection[LogSettingKey.LogWriter] = new LogSettingConfigElement { Key = LogSettingKey.LogWriter, Value = "Microsoft.Support.Workflow.QueryService.Common.BaseException,QueryService.Common" };
                    collection[LogSettingKey.LogName] = new LogSettingConfigElement { Key = LogSettingKey.LogName, Value = LogSettingDefaultValue.LogName };
                    return collection;
                }))
            {
                ILogWriter writer = LogWriterFactory.LogWriter;
                Assert.IsNull(writer);
            }
        }
    }
}

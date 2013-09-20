using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Security;

namespace Microsoft.Support.Workflow.Authoring
{
    public enum OpenMode
    {
        Editing = 0,
        Readonly = 1
    }

    public enum SearchScope
    {
        SearchWholeWorkflow = 0,
        SearchCurrentWorkflow = 1
    }

    public static class DefaultValueSettings
    {
        public const string EnableDownloadDependeciesKey = "DownloadDependencies";
        public const string OpenForEditingModeKey = "OpenForEditing";
        public const string EnvKey = "Env";
        public const string SearchWholeWorkflowKey = "SearchWholeWorkflow";
        public const string EnableTaskAssignmentKey = "EnableTaskAssignment";
        public const string DefaultCategoryKey = "DefaultCategory";
        public static bool EnableDownloadDependecies = true;
        public static bool OpenForEditingMode = true;
        public static Env Environment = Env.Dev;
        public static bool SearchWholeWorkflow = true;//0:The Whole Workflow,1:Current Workflow
        public static bool EnableTaskAssignment = false;
        public static string DefaultCategory = string.Empty;

        static DefaultValueSettings()
        {
            RefreshConfigValues();
        }

        public static void RefreshConfigValues()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            EnableDownloadDependecies =Convert.ToBoolean( config.AppSettings.Settings[EnableDownloadDependeciesKey].Value);
            OpenForEditingMode = Convert.ToBoolean(config.AppSettings.Settings[OpenForEditingModeKey].Value);
            Environment = config.AppSettings.Settings[EnvKey].Value.ToEnv();
            SearchWholeWorkflow = Convert.ToBoolean(config.AppSettings.Settings[SearchWholeWorkflowKey].Value);
            EnableTaskAssignment = Convert.ToBoolean(config.AppSettings.Settings[EnableTaskAssignmentKey].Value);
            DefaultCategory = config.AppSettings.Settings[DefaultCategoryKey].Value;
        }

        public static void SetConfigValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}

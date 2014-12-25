using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using JT76.Common.ObjectExtensions;

namespace JT76.Common.Services
{
    public interface IConfigService
    {
        ConfigurationSectionGroup GetSectionGroup(string strSectionGroupName);
        string GetAppSetting(string strSettingName);
    }

    public class ConfigService : IConfigService
    {
        private const string StrUiConfigName = "Web.config";

        public enum JtAppSettings
        {
            PrimaryDeveloperEmail, GroupEmailSection
        }

        public ConfigurationSectionGroup GetSectionGroup(string strSectionGroupName)
        {
            var config = GetUiDirectoryConfig();

            var configSections = config.SectionGroups.Cast<ConfigurationSectionGroup>()
               .Where(s => s.SectionGroupName == strSectionGroupName);

            if (configSections == null)
                throw new ArgumentNullException();

            var configurationSectionGroups = configSections as IList<ConfigurationSectionGroup> ?? configSections.ToList();

            if (!configurationSectionGroups.Any())
                throw new ArgumentNullException();

            return configurationSectionGroups.First();
        }

        public string GetAppSetting(string strSettingName)
        {
            var config = GetUiDirectoryConfig();

            var strAppSetting = config.AppSettings.Settings[strSettingName].Value;

            if (string.IsNullOrEmpty(strAppSetting))
                throw new ArgumentNullException();

            return strAppSetting;
        }

        private Configuration GetUiDirectoryConfig()
        {
            var bTestDirectory = AppDomain.CurrentDomain.ToString().Contains("UnitTest");

            var fileMap = bTestDirectory ? new ExeConfigurationFileMap { ExeConfigFilename = StrUiConfigName } : new ExeConfigurationFileMap { ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + StrUiConfigName };

            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (!config.HasFile)
                throw new ArgumentNullException();

            return config;
        }
    }
}

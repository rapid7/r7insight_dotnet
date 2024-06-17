using System;
using System.Configuration;

namespace InsightCore.Net
{
    static class SettingsLookupFactory
    {
        public static SettingsLookup Create()
        {
            SettingsLookup settingsLookup = new SettingsLookup();
            settingsLookup.RegisterSettingStore("Environment Variable", CreateEnvironmentVariableLookup());
            settingsLookup.RegisterSettingStore("App Settings", CreateAppSettingsLookup());
            return settingsLookup;
        }

        static SettingsLookup.SettingLookupDelegate CreateEnvironmentVariableLookup()
        {
            return new SettingsLookup.SettingLookupDelegate((settingKey) => System.Environment.GetEnvironmentVariable(settingKey));
        }

        static SettingsLookup.SettingLookupDelegate CreateAppSettingsLookup()
        {
            return new SettingsLookup.SettingLookupDelegate((settingKey) => ConfigurationManager.AppSettings.Get(settingKey));
        }
    }
}

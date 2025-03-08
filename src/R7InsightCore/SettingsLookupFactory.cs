using System;

namespace InsightCore.Net
{
    static class SettingsLookupFactory
    {
        public static SettingsLookup Create()
        {
            SettingsLookup settingsLookup = new SettingsLookup();
            settingsLookup.RegisterSettingStore("Environment Variable", CreateEnvironmentVariableLookup());
#if NETFRAMEWORK || NETSTANDARD
            settingsLookup.RegisterSettingStore("App Settings", CreateAppSettingsLookup());
#endif
            return settingsLookup;
        }

        static SettingsLookup.SettingLookupDelegate CreateEnvironmentVariableLookup()
        {
            return new SettingsLookup.SettingLookupDelegate((settingKey) => System.Environment.GetEnvironmentVariable(settingKey));
        }

#if NETFRAMEWORK || NETSTANDARD
        static SettingsLookup.SettingLookupDelegate CreateAppSettingsLookup()
        {
            return new SettingsLookup.SettingLookupDelegate((settingKey) => System.Configuration.ConfigurationManager.AppSettings.Get(settingKey));
        }
#endif
    }
}

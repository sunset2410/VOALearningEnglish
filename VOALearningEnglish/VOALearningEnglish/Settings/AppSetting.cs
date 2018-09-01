using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOALearningEnglish.Settings
{
    public enum SettingKey
    {
        IsDictionnaryOn,
        IsListNewsOpen,
        IsListVideoOpen,
    }
    class AppSetting
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        public void WriteSettingString(string key, string value)
        {
            if (IsContainKey(key)) DeleteSettingString(key);
            localSettings.Values[key] = value;
        }

        public string ReadSettingString(string key)
        {
            Object value = localSettings.Values[key];
            return value.ToString();
        }

        public void DeleteSettingString(string key)
        {
            localSettings.DeleteContainer(key);
        }

        public void WriteSettingBool(string key, bool value)
        {
            if (IsContainKey(key)) DeleteSettingString(key);
            localSettings.Values[key] = value;
        }

        public bool ReadSettingBool(string key)
        {
            Object value = localSettings.Values[key];
            return (bool)value;
        }

        public void DeleteSettingBool(string key)
        {
            localSettings.DeleteContainer(key);
        }
        public bool IsContainKey(string key)
        {
            return localSettings.Values.ContainsKey(key);
        }
    }
}

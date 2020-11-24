// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Extensions;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration.AzureAppConfiguration
{
    internal class ConfigurationCacheManager
    {
        public ConcurrentDictionary<string, ConfigurationSetting> AllSettings { get; private set; }
        public ConcurrentDictionary<KeyLabelIdentifier, ConfigurationSetting> WatchedSettings { get; private set; }

        public ConfigurationCacheManager() { }

        public void UpdateSettingsCache(IDictionary<string, ConfigurationSetting> allData, IDictionary<KeyLabelIdentifier, ConfigurationSetting> watchedData)
        {
            AllSettings = allData as ConcurrentDictionary<string, ConfigurationSetting> ??
                 new ConcurrentDictionary<string, ConfigurationSetting>(allData, StringComparer.OrdinalIgnoreCase);

            WatchedSettings = watchedData as ConcurrentDictionary<KeyLabelIdentifier, ConfigurationSetting> ??
                new ConcurrentDictionary<KeyLabelIdentifier, ConfigurationSetting>(watchedData);
        }

        public void AddConfigurationSettingToCache(ConfigurationSetting configSetting)
        {
            AllSettings[configSetting.Key] = configSetting;
            WatchedSettings[new KeyLabelIdentifier(configSetting.Key, configSetting.Label)] = configSetting;
        }

        public void ProcessChangesUpdateWatchedSettings(IEnumerable<KeyValueChange> changes)
        {
            foreach (KeyValueChange change in changes)
            {
                string changedKey = change.Key;
                string changedLabel = change.Label;
                KeyLabelIdentifier changedKeyLabel = new KeyLabelIdentifier(changedKey, changedLabel);

                if (change.ChangeType == KeyValueChangeType.Deleted)
                {
                    WatchedSettings.TryRemove(changedKeyLabel, out ConfigurationSetting removedFromWatchedSettings);
                }
                else if (change.ChangeType == KeyValueChangeType.Modified)
                {
                    WatchedSettings[changedKeyLabel] = change.Current;
                }
            }
        }

        public void ProcessChangesUpdateAllSettings(IEnumerable<KeyValueChange> changes)
        {
            foreach (KeyValueChange change in changes)
            {
                string changedKey = change.Key;

                if (change.ChangeType == KeyValueChangeType.Deleted)
                {
                    AllSettings.TryRemove(changedKey, out ConfigurationSetting removedFromAllSettings);
                }
                else if (change.ChangeType == KeyValueChangeType.Modified)
                {
                    // Add or modify
                    AllSettings[changedKey] = change.Current;
                }
            }
        }


        //public void ProcessChanges(IEnumerable<KeyValueChange> changes)
        //{
        //    foreach (KeyValueChange change in changes)
        //    {
        //        string changedKey = change.Key;
        //        string changedLabel = change.Label;
        //        KeyLabelIdentifier changedKeyLabel = new KeyLabelIdentifier(changedKey, changedLabel);

        //        if (change.ChangeType == KeyValueChangeType.Deleted)
        //        {
        //            AllSettings.TryRemove(changedKey, out ConfigurationSetting removedFromAllSettings);
        //            WatchedSettings.TryRemove(changedKeyLabel, out ConfigurationSetting removedFromWatchedSettings);
        //        }
        //        else if (change.ChangeType == KeyValueChangeType.Modified)
        //        {
        //            if (AllSettings.ContainsKey(changedKey) && AllSettings[changedKey].Label == changedLabel.NormalizeNull())
        //            {
        //                AllSettings[changedKey] = change.Current;
        //            }

        //            if (WatchedSettings.TryGetValue(changedKeyLabel, out ConfigurationSetting watchedSetting))
        //            {
        //                WatchedSettings[changedKeyLabel] = change.Current;
        //            }
        //        }
        //    }
        //}
    }
}

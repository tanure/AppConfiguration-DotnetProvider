// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
using Azure;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Extensions;

namespace Microsoft.Extensions.Configuration.AzureAppConfiguration.Models
{
    internal class ClonedConfigurationSetting 
    {
        /// <summary>
        /// Original Key of the key-value in App Configuration.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Original Label of the key-value in App Configuration.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Original ETag indicating the state of the key-value within a configuration store.
        /// </summary>
        public ETag ETag { get; set; }

        /// <summary>
        /// Original value of the key-value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// A flag to indicate if the key-value has been mapped according to user defined mappers.
        /// </summary>
        public bool IsMapped { get; set; }

        /// <summary>
        /// The key-value which may or may not have been mapped.
        /// </summary>
        public ConfigurationSetting KeyValue { get; set; }

        public KeyValueData(ConfigurationSetting keyValue)
        {
            Key = keyValue.Key;
            // Label = keyValue.Label.NormalizeNull();
            ETag = keyValue.ETag;
            // Value = keyValue.Value;
            IsMapped = false;
            KeyValue = keyValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyValueData keyLabel)
            {
                return Key == keyLabel.Key && Label == keyLabel.Label;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Label != null ? Key.GetHashCode() ^ Label.GetHashCode() : Key.GetHashCode();
        }
    }
}

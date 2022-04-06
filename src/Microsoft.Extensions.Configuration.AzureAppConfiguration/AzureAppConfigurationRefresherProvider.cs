// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
//
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Configuration.AzureAppConfiguration
{
	internal class AzureAppConfigurationRefresherProvider : IConfigurationRefresherProvider
	{
		public IEnumerable<IConfigurationRefresher> Refreshers { get; }

		public AzureAppConfigurationRefresherProvider(IConfiguration configuration, ILoggerFactory _loggerFactory)
		{
			var refreshers = new List<IConfigurationRefresher>();

			if (configuration != null)
			{
				AddRefreshers(configuration, refreshers, _loggerFactory);
			}

			if (!refreshers.Any())
			{
				throw new InvalidOperationException("Unable to access the Azure App Configuration provider. Please ensure that it has been configured correctly.");
			}

			Refreshers = refreshers;
		}

		private void AddRefreshers(IConfiguration configuration, List<IConfigurationRefresher> refreshers, ILoggerFactory _loggerFactory)
		{
			var configurationRoot = configuration as IConfigurationRoot;
			if (configurationRoot != null)
			{
				foreach (IConfigurationProvider provider in configurationRoot.Providers)
				{
					if (provider is IConfigurationRefresher refresher)
					{
						// Use _loggerFactory only if LoggerFactory hasn't been set in AzureAppConfigurationOptions
						if (refresher.LoggerFactory == null)
						{
							refresher.LoggerFactory = _loggerFactory;
						}

						refreshers.Add(refresher);
					}
					else if (provider is ChainedConfigurationProvider chainedProvider)
					{
						AddRefreshers(chainedProvider.Configuration, refreshers);
					}
				}
			}
		}
	}
}

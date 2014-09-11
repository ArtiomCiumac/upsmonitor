namespace UpsMonitor
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using NLog;

    internal sealed class MonitoringConfig
    {
        internal readonly uint MinimumLevel;
        internal readonly uint MinimumRunTime;
        internal readonly TimeSpan CheckInterval;

        private MonitoringConfig(uint minimumLevel, uint minimumRunTime, TimeSpan checkInterval)
        {
            MinimumLevel = minimumLevel;
            MinimumRunTime = minimumRunTime;
            CheckInterval = checkInterval;
        }

        internal static MonitoringConfig ReadFromAppSettings()
        {
            var logger = LogManager.GetCurrentClassLogger();

            var settings = ConfigurationManager.AppSettings;

            var minimumLevel = TryParse(settings, Const.Config.MinimumBatteryLevel, Const.DefaultConfig.MinimumBatteryLevel, uint.TryParse, logger);
            var minimumRunTime = TryParse(settings, Const.Config.MinimumRunTime, Const.DefaultConfig.MinimumRunTime, uint.TryParse, logger);
            var checkInterval = TryParse(settings, Const.Config.BatteryCheckInterval, Const.DefaultConfig.BatteryCheckInterval, TimeSpan.TryParse, logger);

            logger.Info(Const.Messages.CurrentConfiguration,
                        Const.Config.MinimumBatteryLevel,
                        minimumLevel,
                        Const.Config.MinimumRunTime,
                        minimumRunTime,
                        Const.Config.BatteryCheckInterval,
                        checkInterval);

            return new MonitoringConfig(minimumLevel, minimumRunTime, checkInterval);
        }

        private static T TryParse<T>(NameValueCollection collection, string key, T defaultValue, ParseDelegate<T> parseDelegate, Logger logger)
        {
            T value;
            var textValue = collection[key];
            if (parseDelegate(textValue, out value))
                return value;

            logger.Error(Const.Messages.InvalidConfiguration, key, typeof(T), textValue, defaultValue);
            return defaultValue;
        }
    }

    internal delegate bool ParseDelegate<T>(string text, out T result);
}
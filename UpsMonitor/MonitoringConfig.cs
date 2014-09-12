namespace UpsMonitor
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using NLog;

    /// <summary>
    /// Contains the configuration data read from application config file.
    /// </summary>
    internal sealed class MonitoringConfig
    {
        /// <summary>
        /// Delegate used to represent a parse function (for example like <see cref="int.TryParse(string, out int)"/>).
        /// </summary>
        /// <typeparam name="T">The result data type.</typeparam>
        /// <param name="text">The text to parse.</param>
        /// <param name="result">The parse result.</param>
        /// <returns><c>true</c> - if parsing succeeded, <c>false</c> - otherwise.</returns>
        private delegate bool ParseDelegate<T>(string text, out T result);

        /// <summary>
        /// The minimum battery level in %.
        /// </summary>
        internal readonly uint MinimumLevel;

        /// <summary>
        /// The minimum estimated run time in minutes.
        /// </summary>
        internal readonly uint MinimumRunTime;

        /// <summary>
        /// The battery check interval.
        /// </summary>
        internal readonly TimeSpan CheckInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoringConfig"/> class with specified configuration values.
        /// </summary>
        /// <param name="minimumLevel">The minimum battery level in %.</param>
        /// <param name="minimumRunTime">he minimum estimated run time in minutes.</param>
        /// <param name="checkInterval">The battery check interval.</param>
        private MonitoringConfig(uint minimumLevel, uint minimumRunTime, TimeSpan checkInterval)
        {
            MinimumLevel = minimumLevel;
            MinimumRunTime = minimumRunTime;
            CheckInterval = checkInterval;
        }

        /// <summary>
        /// Reads the current configuration values from application config file.
        /// </summary>
        /// <remarks>
        /// Parameter names are read as in <see cref="Const.Config"/>,
        /// if parsing fails - uses default values from <see cref="Const.DefaultConfig"/>.
        /// </remarks>
        /// <returns>A new instance of <see cref="MonitoringConfig"/> containing the read parameters.</returns>
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

        /// <summary>
        /// Reads the field with specified name from provided <see cref="NameValueCollection"/> and tries to convert it using specified delegate.
        /// If conversion fails - writes a log message and returs the <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="T">The type of the field to convert to.</typeparam>
        /// <param name="collection">The collection of values to read from.</param>
        /// <param name="key">The field name to read.</param>
        /// <param name="defaultValue">The default fallback value.</param>
        /// <param name="parseDelegate">The convert delegate.</param>
        /// <param name="logger">Current logger.</param>
        /// <returns>The read value or <paramref name="defaultValue"/> if conversion from <c>string</c> to <typeparamref name="T"/> failed.</returns>
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
}
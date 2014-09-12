namespace UpsMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using NLog;

    /// <summary>
    /// Wrapper class responsible for enumerating batteries and comparing their property values with provided configuration.
    /// </summary>
    internal sealed class BatteryWrapper
    {
        /// <summary>
        /// Battery states when need to check their levels (e.g. there is no point to check if battery is low if it is on AC power).
        /// </summary>
        private readonly ushort[] _shutdownCodes = { Const.Status.IsDischarging };

        /// <summary>
        /// Queries WMI for battery information.
        /// </summary>
        private readonly ManagementObjectSearcher _batterySearcher;

        /// <summary>
        /// Translates numeric status codes to their textual explanation.
        /// </summary>
        private readonly Dictionary<ushort, string> _statusCodes;

        /// <summary>
        /// Minimum battery level after which computer needs to be shut down.
        /// </summary>
        private readonly uint _minimumLevel;

        /// <summary>
        /// Minimum estimated run time after which computer needs to be shut down.
        /// </summary>
        private readonly uint _minimumRunTime;

        /// <summary>
        /// Current class's logger.
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// Initializesa new instance of the <see cref="BatteryWrapper"/> class with provided minimum estimated level and run time.
        /// </summary>
        /// <param name="minimumLevel">Minimum battery level after which computer needs to be shut down.</param>
        /// <param name="minimumRunTime">Minimum estimated run time after which computer needs to be shut down.</param>
        public BatteryWrapper(uint minimumLevel, uint minimumRunTime)
        {
            _minimumLevel = minimumLevel;
            _minimumRunTime = minimumRunTime;

            _logger = LogManager.GetCurrentClassLogger();

            _batterySearcher = new ManagementObjectSearcher(Const.Wmi.SelectBatteryQuery);

            _statusCodes = new Dictionary<ushort, string>
                           {
                               {Const.Status.IsDischarging, Const.StatusMessages.IsDischarging},
                               {Const.Status.IsOnAcPower, Const.StatusMessages.IsOnAcPower},
                               {Const.Status.IsFullyCharged, Const.StatusMessages.IsFullyCharged},
                               {Const.Status.IsLow, Const.StatusMessages.IsLow},
                               {Const.Status.IsCritical, Const.StatusMessages.IsCritical},
                               {Const.Status.IsCharging, Const.StatusMessages.IsCharging},
                               {Const.Status.IsChargingAndHigh, Const.StatusMessages.IsChargingAndHigh},
                               {Const.Status.IsChargingAndLow, Const.StatusMessages.IsChargingAndLow},
                               {Const.Status.IsUnknown, Const.StatusMessages.IsUnknown},
                               {Const.Status.IsPartiallyCharged, Const.StatusMessages.IsPartiallyCharged}
                           };
        }

        /// <summary>
        /// Checks if any battery level or estimated run time is below supplied thresholds.
        /// </summary>
        /// <returns>Returns <c>true</c> if any battery level or estimated run time are below thresholds.</returns>
        internal bool IsShutdownNeeded()
        {
            foreach (ManagementObject mo in _batterySearcher.Get())
            {
                // try read battery properties
                var batteryName = TryGet<string>(mo, Const.Wmi.FieldName);
                var statusCode = TryGet<ushort>(mo, Const.Wmi.FieldBatteryStatus);
                var estimatedChargePercent = TryGet<ushort>(mo, Const.Wmi.FieldEstimatedChargeRemaining);
                var estimatedRunTime = TryGet<uint>(mo, Const.Wmi.FieldEstimatedRunTime);

                // write battery info message
                _logger.Trace(Const.Messages.BatteryInfo,
                                  batteryName,
                                  _statusCodes[statusCode],
                                  statusCode,
                                  estimatedChargePercent,
                                  estimatedRunTime);

                return _shutdownCodes.Contains(statusCode)
                                    && ((estimatedChargePercent > 0 && estimatedChargePercent < _minimumLevel)
                                        || (estimatedRunTime > 0 && estimatedRunTime < _minimumRunTime));
            }

            return false;
        }

        /// <summary>
        /// Helper class to read a field with specified name and type from the provided <see cref="ManagementBaseObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="mo">The <see cref="ManagementBaseObject"/> instance to read the field value from.</param>
        /// <param name="fieldName">The field name to read.</param>
        /// <returns>The field value or <c>default(T)</c> if it cannot be converted to <typeparamref name="T"/>.</returns>
        private T TryGet<T>(ManagementBaseObject mo, string fieldName)
            where T : IConvertible
        {
            var value = mo[fieldName];
            if (value != null)
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    _logger.Error(Const.Messages.CannotParseWmiValue, fieldName, value, typeof(T), ex.Message);
                }
            }

            return default(T);
        }
    }
}
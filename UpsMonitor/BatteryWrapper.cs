namespace UpsMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using NLog;

    internal sealed class BatteryWrapper
    {
        private readonly ushort[] _shutdownCodes = { Const.Status.IsDischarging };

        private readonly ManagementObjectSearcher _batterySearcher;
        private readonly Dictionary<ushort, string> _statusCodes;

        private readonly uint _minimumLevel;
        private readonly uint _minimumRunTime;

        private readonly Logger _logger;

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

        internal bool IsShutdownNeeded()
        {
            foreach (ManagementObject mo in _batterySearcher.Get())
            {
                var batteryName = TryGet<string>(mo, Const.Wmi.FieldName);
                var statusCode = TryGet<ushort>(mo, Const.Wmi.FieldBatteryStatus);

                var statusString = _statusCodes[statusCode];

                var estimatedChargePercent = TryGet<ushort>(mo, Const.Wmi.FieldEstimatedChargeRemaining);
                var estimatedRunTime = TryGet<uint>(mo, Const.Wmi.FieldEstimatedRunTime);

                _logger.Trace(Const.Messages.BatteryInfo,
                                  batteryName,
                                  statusString,
                                  statusCode,
                                  estimatedChargePercent,
                                  estimatedRunTime);

                return _shutdownCodes.Contains(statusCode)
                                    && ((estimatedChargePercent > 0 && estimatedChargePercent < _minimumLevel)
                                        || (estimatedRunTime > 0 && estimatedRunTime < _minimumRunTime));
            }

            return false;
        }

        internal T TryGet<T>(ManagementObject mo, string fieldName)
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
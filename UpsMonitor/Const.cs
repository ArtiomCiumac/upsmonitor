namespace UpsMonitor
{
    using System;

    internal static class Const
    {
        internal static class Wmi
        {
            internal const string SelectBatteryQuery = "select * from Win32_Battery";
            internal const string FieldName = "Name";
            internal const string FieldBatteryStatus = "BatteryStatus";
            internal const string FieldEstimatedChargeRemaining = "EstimatedChargeRemaining";
            internal const string FieldEstimatedRunTime = "EstimatedRunTime";
        }

        internal static class Messages
        {
            internal const string ServiceInitialized = "Service initialized";
            internal const string ServiceStartRequested = "Start requested";
            internal const string ServiceStopRequested = "Stop requested";
            internal const string ServicePauseRequested = "Pause requested";
            internal const string ServiceContinueRequested = "Continue requested";
            internal const string ServiceShutdownRequested = "Shutdown requested";

            internal const string StartingMonitoringTask = "Starting monitoring task";
            internal const string StoppingMonitoringTask = "Stopping monitoring task";

            internal const string MonitoringAlreadyRunning = "Monitoring is already running, ignoring start request.";
            internal const string MonitoringIsNotRunning = "Monitoring is not running, ignoring stop request.";
            internal const string CurrentConfiguration = "Configuration: {0}={1}, {2}={3}, {4}={5}";
            internal const string InvalidConfiguration = "Invalid configuration for '{0}' - cannot parse {1} from value {2}, assuming default value {3}";
            internal const string BatteryInfo = "Found battery '{0}' with status '{1}' ({2}), estimated charge: {3}, estimated run time: {4}.";
            internal const string CannotParseWmiValue = "Cannot read WMI value for field '{0}' from '{1}', {2} data type expected ({3}).";
            internal const string GotCommandLineArguments = "Got command line arguments: {0}";
            internal const string FailedToInstallService = "Failed to install service.";
            internal const string FailedToUninstallService = "Failed to uninstall service";
        }

        internal static class Status
        {
            internal const int IsDischarging = 1;
            internal const int IsOnAcPower = 2;
            internal const int IsFullyCharged = 3;
            internal const int IsLow = 4;
            internal const int IsCritical = 5;
            internal const int IsCharging = 6;
            internal const int IsChargingAndHigh = 7;
            internal const int IsChargingAndLow = 8;
            internal const int IsUnknown = 9;
            internal const int IsPartiallyCharged = 10;
        }

        internal static class StatusMessages
        {
            internal const string IsDischarging = "The battery is discharging";
            internal const string IsOnAcPower = "The system has access to AC and no battery is being discharged";
            internal const string IsFullyCharged = "Fully Charged";
            internal const string IsLow = "Low";
            internal const string IsCritical = "Critical";
            internal const string IsCharging = "Charging";
            internal const string IsChargingAndHigh = "Charging and High";
            internal const string IsChargingAndLow = "Charging and Low";
            internal const string IsUnknown = "Undefined";
            internal const string IsPartiallyCharged = "Partially Charged";
        }

        internal static class Config
        {
            internal const string MinimumBatteryLevel = "MinimumBatteryLevel";
            internal const string MinimumRunTime = "MinimumRunTime";
            internal const string BatteryCheckInterval = "BatteryCheckInterval";
        }

        internal static class DefaultConfig
        {
            internal const uint MinimumBatteryLevel = 30;
            internal const uint MinimumRunTime = 10;
            internal static readonly TimeSpan BatteryCheckInterval = TimeSpan.FromSeconds(30);
        }

        internal static class Command
        {
            internal const string Install = "install";
            internal const string Uninstall = "uninstall";
        }
    }
}
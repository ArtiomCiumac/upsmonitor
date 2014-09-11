namespace UpsMonitor
{
    using System;
    using System.Configuration.Install;
    using System.Reflection;
    using NLog;

    internal static class ServiceInstallerUtility
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static bool Install()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { _exePath });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
            return true;
        }

        public static bool Uninstall()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { "/u", _exePath });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
            return true;
        }
    }
}
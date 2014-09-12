namespace UpsMonitor
{
    using System;
    using System.Configuration.Install;
    using System.Reflection;
    using NLog;

    /// <summary>
    /// Helper class that encapsulates windows service installation and uninstallation functionality.
    /// </summary>
    internal static class ServiceInstallerUtility
    {
        /// <summary>
        /// The current executable location path.
        /// </summary>
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;

        /// <summary>
        /// The current logger.
        /// </summary>
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Installs the windows service.
        /// </summary>
        /// <returns><c>true</c> - if installation succeeded, <c>false</c> - otherwise.</returns>
        internal static bool Install()
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

        /// <summary>
        /// Uninstalls the windows service.
        /// </summary>
        /// <returns><c>true</c> - if uninstallation succeeded, <c>false</c> - otherwise.</returns>
        internal static bool Uninstall()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { Const.Command.UninstallSwitch, _exePath });
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
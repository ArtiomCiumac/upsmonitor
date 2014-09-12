namespace UpsMonitor
{
    using System.Diagnostics;

    /// <summary>
    /// Helper class to shutdown the computer.
    /// </summary>
    internal static class ShutdownHelper
    {
        /// <summary>
        /// Shuts down the computer.
        /// </summary>
        internal static void Shutdown()
        {
            Process.Start("shutdown", "/s /t 0");
        }
    }
}
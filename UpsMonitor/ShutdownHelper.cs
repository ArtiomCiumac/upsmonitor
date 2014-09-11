namespace UpsMonitor
{
    using System.Diagnostics;

    internal static class ShutdownHelper
    {
        internal static void Shutdown()
        {
            Process.Start("shutdown", "/s /t 0");
        }
    }
}
namespace UpsMonitor
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using NLog;

    /// <summary>
    /// Contains the main entry point of the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(params string[] args)
        {
            // check if we have any arguments passed
            if (args != null && args.Length == 1)
            {
                ExecuteProgramCommand(args);
            }
            else
            {
                // check if we are running as service or as a simple application
                if (Environment.UserInteractive)
                {
                    RunConsoleApplication();
                }
                else
                {
                    RunWindowsService();
                }
            }
        }

        /// <summary>
        /// Processes the provided application arguments and executes the associated commands.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        private static void ExecuteProgramCommand(IList<string> args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(Const.Messages.GotCommandLineArguments, args[0]);

            // check if we want to install or uninstall the service, otherwise just ignore it.
            switch (args[0].ToLowerInvariant())
            {
                case Const.Command.Install:
                    if (!ServiceInstallerUtility.Install())
                        logger.Error(Const.Messages.FailedToInstallService);
                    break;
                case Const.Command.Uninstall:
                    if (!ServiceInstallerUtility.Uninstall())
                        logger.Error(Const.Messages.FailedToUninstallService);
                    break;
                default:
                    Console.WriteLine(Const.Messages.UnknownCommand, args[0], Const.Command.Install, Const.Command.Uninstall);
                    break;
            }
        }

        /// <summary>
        /// Runs the application as a simple console application.
        /// </summary>
        private static void RunConsoleApplication()
        {
            Console.WriteLine(Const.Messages.ServiceStartRequested);

            var control = new ServiceControl();

            control.Start();

            Console.WriteLine(Const.Messages.PressAnyKeyToStop);
            Console.ReadKey();

            control.Stop();
        }

        /// <summary>
        /// Runs the application as a windows service.
        /// </summary>
        private static void RunWindowsService()
        {
            ServiceBase.Run(new UpsMonitorService());
        }
    }
}

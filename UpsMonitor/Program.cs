namespace UpsMonitor
{
    using System;
    using System.ServiceProcess;
    using NLog;

    static class Program
    {
        static void Main(params string[] args)
        {
            if (args != null && args.Length == 1)
            {
                ExecuteProgramCommand(args);
            }
            else
            {
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

        private static void ExecuteProgramCommand(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(Const.Messages.GotCommandLineArguments, args[0]);

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
            }
        }

        private static void RunConsoleApplication()
        {
            Console.WriteLine("Starting service.");

            var control = new ServiceControl();

            control.Start();    

            Console.WriteLine("Press any key to stop service.");
            Console.ReadKey();

            control.Stop();
        }

        private static void RunWindowsService()
        {
            ServiceBase.Run(new UpsMonitorService());
        }
    }
}

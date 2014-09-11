namespace UpsMonitor
{
    using System;
    using NLog;

    internal sealed class ServiceControl
    {
        private readonly Logger _logger;
        private readonly MonitoringTask _monitoringTask;

        internal ServiceControl()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _monitoringTask = new MonitoringTask(MonitoringConfig.ReadFromAppSettings());

            _logger.Info(Const.Messages.ServiceInitialized);
        }

        internal void Start()
        {
            ChangeServiceState(Const.Messages.ServiceStartRequested, x => x.Start());
        }

        internal void Stop()
        {
            ChangeServiceState(Const.Messages.ServiceStopRequested, x => x.Stop());
        }

        internal void Pause()
        {
            ChangeServiceState(Const.Messages.ServicePauseRequested, x => x.Stop());
        }

        internal void Continue()
        {
            ChangeServiceState(Const.Messages.ServiceContinueRequested, x => x.Start());
        }

        internal void Shutdown()
        {
            ChangeServiceState(Const.Messages.ServiceShutdownRequested, x => x.Stop());
        }

        private void ChangeServiceState(string logMessage, Action<MonitoringTask> changeStateAction)
        {
            if (logMessage == null) throw new ArgumentNullException("logMessage");
            if (changeStateAction == null) throw new ArgumentNullException("changeStateAction");

            _logger.Info(logMessage);

            changeStateAction(_monitoringTask);
        }
    }
}
namespace UpsMonitor
{
    using System;
    using NLog;

    /// <summary>
    /// Main service entry point that can be run either as a interactive application or as a windows service.
    /// </summary>
    internal sealed class ServiceControl
    {
        /// <summary>
        /// The current logger.
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// The current battery monitoring task.
        /// </summary>
        private readonly MonitoringTask _monitoringTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceControl"/> class.
        /// </summary>
        internal ServiceControl()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _monitoringTask = new MonitoringTask(MonitoringConfig.ReadFromAppSettings());

            _logger.Info(Const.Messages.ServiceInitialized);
        }

        /// <summary>
        /// Starts the monitoring process.
        /// </summary>
        internal void Start()
        {
            ChangeServiceState(Const.Messages.ServiceStartRequested, x => x.Start());
        }

        /// <summary>
        /// Stops the monitoring process.
        /// </summary>
        internal void Stop()
        {
            ChangeServiceState(Const.Messages.ServiceStopRequested, x => x.Stop());
        }

        /// <summary>
        /// Pauses the monitoring process.
        /// </summary>
        internal void Pause()
        {
            ChangeServiceState(Const.Messages.ServicePauseRequested, x => x.Stop());
        }

        /// <summary>
        /// Continues the monitoring process.
        /// </summary>
        internal void Continue()
        {
            ChangeServiceState(Const.Messages.ServiceContinueRequested, x => x.Start());
        }

        /// <summary>
        /// Shuts down the monitoring process.
        /// </summary>
        internal void Shutdown()
        {
            ChangeServiceState(Const.Messages.ServiceShutdownRequested, x => x.Stop());
        }

        /// <summary>
        /// Helper method to change the state of the monitoring task and write a corresponding message to log.
        /// </summary>
        /// <param name="logMessage">The log message to write.</param>
        /// <param name="changeStateAction">The changing state delegate.</param>
        private void ChangeServiceState(string logMessage, Action<MonitoringTask> changeStateAction)
        {
            if (logMessage == null) throw new ArgumentNullException("logMessage");
            if (changeStateAction == null) throw new ArgumentNullException("changeStateAction");

            _logger.Info(logMessage);

            changeStateAction(_monitoringTask);
        }
    }
}
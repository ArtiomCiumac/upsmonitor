namespace UpsMonitor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;

    /// <summary>
    /// Encapsulates an asyncronous battery monitoring task and provides functionality to start and stop it.
    /// </summary>
    /// <remarks>Public members of this class are thread safe.</remarks>
    internal sealed class MonitoringTask
    {
        /// <summary>
        /// Synchronization root to ensure that only one monitoring task is active at the same time.
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// The battery wrapper instance to check its state.
        /// </summary>
        private readonly BatteryWrapper _batteryWrapper;

        /// <summary>
        /// Current logger.
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// The battery status check interval.
        /// </summary>
        private readonly TimeSpan _checkInterval;

        /// <summary>
        /// The actual asynchronous monitoring task.
        /// </summary>
        private Task _monitorTask;

        /// <summary>
        /// The cancellation token source used to stop the <see cref="_monitorTask"/>.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoringTask"/> from the provided <see cref="MonitoringConfig"/>.
        /// </summary>
        /// <param name="configuration">The current monitoring configuration.</param>
        public MonitoringTask(MonitoringConfig configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _logger = LogManager.GetCurrentClassLogger();
            _batteryWrapper = new BatteryWrapper(configuration.MinimumLevel, configuration.MinimumRunTime);
            _checkInterval = configuration.CheckInterval;
        }

        /// <summary>
        /// Starts checking the battery. If the task is already started - does nothing.
        /// </summary>
        public void Start()
        {
            lock (_lockObject)
            {
                // make sure task is not running
                if (_monitorTask != null)
                {
                    _logger.Warn(Const.Messages.MonitoringAlreadyRunning);
                    return;
                }

                _logger.Info(Const.Messages.StartingMonitoringTask);

                _cancellationTokenSource = new CancellationTokenSource();

                _monitorTask = Task.Factory.StartNew(() => RunMonitoring(_cancellationTokenSource.Token),
                                                     _cancellationTokenSource.Token,
                                                     TaskCreationOptions.LongRunning,
                                                     TaskScheduler.Current);
            }
        }

        /// <summary>
        /// Stops checking the battery. If the task is already stopped - does nothing.
        /// </summary>
        public void Stop()
        {
            lock (_lockObject)
            {
                // make sure task is not stopped yet
                if (_monitorTask == null)
                {
                    _logger.Warn(Const.Messages.MonitoringIsNotRunning);
                    return;
                }

                _logger.Info(Const.Messages.StoppingMonitoringTask);

                _cancellationTokenSource.Cancel();

                _cancellationTokenSource = null;
                _monitorTask = null;
            }
        }

        /// <summary>
        /// Runs the monitoring process.
        /// </summary>
        private void RunMonitoring(CancellationToken token)
        {
            // run forever, unless cancelled
            while (true)
            {
                // task can be canceled at this point
                token.ThrowIfCancellationRequested();

                if (_batteryWrapper.IsShutdownNeeded())
                    ShutdownHelper.Shutdown();

                // wait the check interval, unless canceled
                try
                {
                    Task.Delay(_checkInterval, token).Wait(token);
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.Count != 1 || !(ex.InnerException is TaskCanceledException))
                        throw;
                }
            }
        }
    }
}
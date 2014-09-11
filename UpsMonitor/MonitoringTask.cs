namespace UpsMonitor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;

    internal sealed class MonitoringTask
    {
        private readonly object _lockObject = new object();
        private readonly BatteryWrapper _batteryWrapper;
        private readonly Logger _logger;
        private readonly TimeSpan _checkInterval;

        private Task _monitorTask;
        private CancellationTokenSource _cancellationTokenSource;

        public MonitoringTask(MonitoringConfig configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _logger = LogManager.GetCurrentClassLogger();
            _batteryWrapper = new BatteryWrapper(configuration.MinimumLevel, configuration.MinimumRunTime);
            _checkInterval = configuration.CheckInterval;
        }

        public void Start()
        {
            lock (_lockObject)
            {
                if (_monitorTask != null)
                {
                    _logger.Warn(Const.Messages.MonitoringAlreadyRunning);
                    return;
                }

                _logger.Info(Const.Messages.StartingMonitoringTask);

                _cancellationTokenSource = new CancellationTokenSource();

                _monitorTask = Task.Factory.StartNew(RunMonitoring,
                                                     _cancellationTokenSource.Token,
                                                     TaskCreationOptions.LongRunning,
                                                     TaskScheduler.Current);
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
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

        private void RunMonitoring()
        {
            while (true)
            {
                var source = _cancellationTokenSource;
                if (source == null) 
                    break;

                source.Token.ThrowIfCancellationRequested();

                if (_batteryWrapper.IsShutdownNeeded())
                    ShutdownHelper.Shutdown();

                try
                {
                    Task.Delay(_checkInterval, source.Token).Wait(source.Token);
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
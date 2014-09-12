namespace UpsMonitor
{
    using System.ServiceProcess;

    /// <summary>
    /// Encapsulates the Windows Service functionality.
    /// </summary>
    internal partial class UpsMonitorService : ServiceBase
    {
        /// <summary>
        /// The underlying service control instance.
        /// </summary>
        private readonly ServiceControl _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpsMonitorService"/> class.
        /// </summary>
        internal UpsMonitorService()
        {
            InitializeComponent();

            _service = new ServiceControl();
        }

        /// <summary>
        /// Handles the 'start' service control command.
        /// </summary>
        /// <param name="args">Ignored.</param>
        protected override void OnStart(string[] args)
        {
            _service.Start();
        }

        /// <summary>
        /// Handles the 'stop' service control command.
        /// </summary>
        protected override void OnStop()
        {
            _service.Stop();
        }

        /// <summary>
        /// Handles the 'pause' service control command.
        /// </summary>
        protected override void OnPause()
        {
            _service.Pause();
        }

        /// <summary>
        /// Handles the 'continue' service control command.
        /// </summary>
        protected override void OnContinue()
        {
            _service.Continue();
        }

        /// <summary>
        /// Handles the 'shutdown' service control command.
        /// </summary>
        protected override void OnShutdown()
        {
            _service.Shutdown();
        }
    }
}

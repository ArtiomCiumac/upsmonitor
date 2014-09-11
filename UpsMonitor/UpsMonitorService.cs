namespace UpsMonitor
{
    using System.ServiceProcess;

    internal partial class UpsMonitorService : ServiceBase
    {
        private readonly ServiceControl _service;

        internal UpsMonitorService()
        {
            InitializeComponent();

            CanPauseAndContinue = true;
            CanShutdown = true;

            _service = new ServiceControl();
        }

        protected override void OnStart(string[] args)
        {
            _service.Start();
        }

        protected override void OnStop()
        {
            _service.Stop();
        }

        protected override void OnPause()
        {
            _service.Pause();
        }

        protected override void OnContinue()
        {
            _service.Continue();
        }

        protected override void OnShutdown()
        {
            _service.Shutdown();
        }
    }
}

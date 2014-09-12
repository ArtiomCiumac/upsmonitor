namespace UpsMonitor
{
    using System.ComponentModel;
    using System.Configuration.Install;

    /// <summary>
    /// Holds all configuration related to Windows Service installation.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInstaller"/> class.
        /// </summary>
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}

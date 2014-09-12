UPS Monitor
==========

Simple windows service to monitor locally attached UPS and shutdown the computer if remaining time or battery charge percent becomes less than configured threshold.

The service uses NLog and must have write access to `Logs` folder.

To read the battery information the service is using WMI commands, so the running account must have correct access rights to query WMI information.


Service installation
--------------------

Run the following command as administrator from command line: `UpsMonitor.exe install`


Service uninstallation
----------------------

Run the following command as administrator from command line: `UpsMonitor.exe uninstall`


Running service as command line application
-------------------------------------------

Simply run the following command: `UpsMonitor.exe`

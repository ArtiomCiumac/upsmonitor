namespace UpsMonitor
{
    using System;
    using System.Globalization;
    using NLog;
    using NLog.Common;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    [Target("DiscardDuplicates", IsWrapper = true, IsCompound = false)]
    public class DiscardDuplicatestWrapperTarget : WrapperTargetBase
    {
        private LogEventInfo _previousLogEvent;
        private int _previousCount;

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            if (IsDifferentFromPrevious(logEvent.LogEvent))
            {
                if (_previousLogEvent != null && _previousCount > 0)
                {
                    var e = LogEventInfo.Create(_previousLogEvent.Level,
                                                _previousLogEvent.LoggerName,
                                                CultureInfo.CurrentCulture,
                                                "{0} more of previous message.",
                                                new object[] { _previousCount });

                    WrappedTarget.WriteAsyncLogEvent(new AsyncLogEventInfo(e, ex => { }));
                }

                _previousLogEvent = logEvent.LogEvent;
                _previousCount = 0;

                WrappedTarget.WriteAsyncLogEvent(logEvent);
            }
            else
            {
                _previousCount++;
            }
        }

        private bool IsDifferentFromPrevious(LogEventInfo eventInfo)
        {
            if (eventInfo == null) throw new ArgumentNullException("eventInfo");

            if (_previousLogEvent == null) return true;

            return _previousLogEvent.Level != eventInfo.Level
                   || _previousLogEvent.LoggerName != eventInfo.LoggerName
                   || _previousLogEvent.FormattedMessage != eventInfo.FormattedMessage;
        }

        private void WriteEvent(AsyncLogEventInfo logEvent)
        {

        }
    }
}
namespace UpsMonitor
{
    using System;
    using System.Globalization;
    using NLog;
    using NLog.Common;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    /// <summary>
    /// NLog custom wrapper target that discards messages that are duplicate in a row.
    /// </summary>
    [Target("DiscardDuplicates", IsWrapper = true, IsCompound = false)]
    public class DiscardDuplicatestWrapperTarget : WrapperTargetBase
    {
        /// <summary>
        /// Previous log event that needs to be compared with the current one.
        /// </summary>
        private LogEventInfo _previousLogEvent;

        /// <summary>
        /// The number of same events as the previous one.
        /// </summary>
        private int _previousCount;

        /// <summary>
        /// Writes the provided <see cref="AsyncLogEventInfo"/> to the underlying target.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            // check if current event is different from previous
            if (IsDifferentFromPrevious(logEvent.LogEvent))
            {
                // current event is different - we need to write the count of same as previous messages
                if (_previousLogEvent != null && _previousCount > 0)
                {
                    var e = LogEventInfo.Create(_previousLogEvent.Level,
                                                _previousLogEvent.LoggerName,
                                                CultureInfo.CurrentCulture,
                                                "{0} more of previous message.",
                                                new object[] { _previousCount });

                    WrappedTarget.WriteAsyncLogEvent(new AsyncLogEventInfo(e, ex => { }));
                }

                // reset counters
                _previousLogEvent = logEvent.LogEvent;
                _previousCount = 0;

                // write current event
                WrappedTarget.WriteAsyncLogEvent(logEvent);
            }
            else
            {
                // if current event is same as previous - simply increase the count and don't write it to the wrapped target
                _previousCount++;
            }
        }

        /// <summary>
        /// Checks if the provided <see cref="LogEventInfo"/> is different from the saved previous one.
        /// </summary>
        /// <param name="eventInfo">The new event.</param>
        /// <returns><c>true</c> if the provided event is different and needs to be written immediately, <c>false</c> - otherwise.</returns>
        private bool IsDifferentFromPrevious(LogEventInfo eventInfo)
        {
            if (eventInfo == null) throw new ArgumentNullException("eventInfo");

            if (_previousLogEvent == null) return true;

            return _previousLogEvent.Level != eventInfo.Level
                   || _previousLogEvent.LoggerName != eventInfo.LoggerName
                   || _previousLogEvent.FormattedMessage != eventInfo.FormattedMessage;
        }
    }
}
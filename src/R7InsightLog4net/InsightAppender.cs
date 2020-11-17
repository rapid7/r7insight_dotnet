using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;

using InsightCore.Net;

namespace log4net.Appender
{
    public class InsightAppender : AppenderSkeleton, IAsyncLoggerConfig
    {
        class Log4netAsyncLogger : AsyncLogger
        {
            protected override void WriteDebugMessages(string message, Exception ex)
            {
                base.WriteDebugMessages(message, ex);
                log4net.Util.LogLog.Warn(GetType(), message, ex);
            }

            public override bool LoadCredentials()
            {
                bool success = base.LoadCredentials();
                if (!success)
                {
                    log4net.Util.LogLog.Warn(GetType(), "Failed to load credentials");
                }
                return success;
            }
        }

        private Log4netAsyncLogger insightAsync;

        public InsightAppender()
        {
            insightAsync = new Log4netAsyncLogger();
        }

        /// <inheritdoc />
        public string Token
        {
            get
            {
                return insightAsync.getToken();
            }
            set
            {
                insightAsync.setToken(value);
            }
        }

        /// <inheritdoc />
        public bool Debug
        {
            get
            {
                return insightAsync.getDebug();
            }
            set
            {
                insightAsync.setDebug(value);
            }
        }

        /// <inheritdoc />
        public bool UseSsl
        {
            get
            {
                return insightAsync.getUseSsl();
            }
            set
            {
                insightAsync.setUseSsl(value);
            }
        }

        /// <inheritdoc />
        public bool IsUsingDataHub
        {
            get
            {
                return insightAsync.getIsUsingDataHab();
            }
            set
            {
                insightAsync.setIsUsingDataHub(value);
            }
        }

        /// <inheritdoc />
        public string DataHubAddress
        {
            get
            {
                return insightAsync.getDataHubAddr();
            }
            set
            {
                insightAsync.setDataHubAddr(value);
            }
        }

        /// <inheritdoc />
        public int DataHubPort
        {
            get
            {
                return insightAsync.getDataHubPort();
            }
            set
            {
                insightAsync.setDataHubPort(value);
            }
        }

        /// <inheritdoc />
        public bool LogHostname
        {
            get
            {
                return insightAsync.getUseHostName();
            }
            set
            {
                insightAsync.setUseHostName(value);
            }
        }

        /// <inheritdoc />
        public String HostName
        {
            get
            {
                return insightAsync.getHostName();
            }
            set
            {
                insightAsync.setHostName(value);
            }
        }

        /// <inheritdoc />
        public String LogID
        {
            get
            {
                return insightAsync.getLogID();
            }
            set
            {
                insightAsync.setLogID(value);
            }
        }

        /// <inheritdoc />
        public String Region
        {
            get
            {
                return insightAsync.getRegion();
            }
            set
            {
                insightAsync.setRegion(value);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var renderedEvent = RenderLoggingEvent(loggingEvent);
            insightAsync.AddLine(renderedEvent);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            foreach (var logEvent in loggingEvents)
            {
                this.Append(logEvent);
            }
        }

        protected override bool RequiresLayout
        {
            get
            {
                return true;
            }
        }

        public override bool Flush(int millisecondsTimeout)
        {
            return insightAsync.FlushQueue(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        protected override void OnClose()
        {
            insightAsync.interruptWorker();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

using InsightCore.Net;

namespace NLog.Targets
{
    [Target("Insight")]
    public sealed class InsightTarget : TargetWithLayout, IAsyncLoggerConfig
    {
        class NLogAsyncLogger : AsyncLogger
        {
            protected override void WriteDebugMessages(string message, Exception ex)
            {
                base.WriteDebugMessages(message, ex);
                InternalLogger.Warn(string.Concat(message, " Exception: ", ex.ToString()));
            }

            public override bool LoadCredentials()
            {
                bool success = base.LoadCredentials();
                if (!success)
                {
                    InternalLogger.Warn("Failed to load credentials");
                }
                return success;
            }
        }

        private NLogAsyncLogger insightAsync;

        public InsightTarget()
        {
            insightAsync = new NLogAsyncLogger();
        }

        /// <inheritdoc />
        public bool Debug 
        {
            get { return insightAsync.getDebug(); }
            set { insightAsync.setDebug(value); } 
        }

        /// <inheritdoc />
        public bool IsUsingDataHub
        {
            get { return insightAsync.getIsUsingDataHab(); }
            set { insightAsync.setIsUsingDataHub(value); }
        }

        /// <inheritdoc />
        public string DataHubAddress
        {
            get { return insightAsync.getDataHubAddr(); }
            set { insightAsync.setDataHubAddr(value); }
        }

        /// <inheritdoc />
        public int DataHubPort
        {
            get { return insightAsync.getDataHubPort(); }
            set { insightAsync.setDataHubPort(value); }
        }

        /// <inheritdoc />
        public bool UseSsl
        {
            get { return insightAsync.getUseSsl(); }
            set { insightAsync.setUseSsl(value); }
        }

        /// <inheritdoc />
        public string Token
        {
            get { return insightAsync.getToken(); }
            set { insightAsync.setToken(value); }
        }

        /// <inheritdoc />
        public bool LogHostname
        {
            get { return insightAsync.getUseHostName(); }
            set { insightAsync.setUseHostName(value); }
        }

        /// <inheritdoc />
        public string HostName
        {
            get { return insightAsync.getHostName(); }
            set { insightAsync.setHostName(value); }
        }

        /// <inheritdoc />
        public string LogID
        {
            get { return insightAsync.getLogID(); }
            set { insightAsync.setLogID(value); }
        }

        /// <inheritdoc />
        public string Region
        {
            get { return insightAsync.getRegion(); }
            set { insightAsync.setRegion(value); }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            //Render message content
            String renderedEvent = this.Layout.Render(logEvent);
            insightAsync.AddLine(renderedEvent);
        }

        protected override void CloseTarget()
        {
            base.CloseTarget();
            insightAsync.interruptWorker();
        }

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            if (!insightAsync.FlushQueue(TimeSpan.FromMilliseconds(50)))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        InternalLogger.Trace("Waiting for AsyncLogger queue flush");
                        if (insightAsync.FlushQueue(TimeSpan.FromSeconds(5)))
                        {
                            InternalLogger.Trace("Completed AsyncLogger queue flush");
                            asyncContinuation(null);
                            return;
                        }
                    }
                    InternalLogger.Warn("Timeout while waiting for AsyncLogger queue flush");
                    asyncContinuation(new TimeoutException("AsyncLogger queues are not empty"));
                }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskCreationOptions.None, System.Threading.Tasks.TaskScheduler.Default);
            }
            else
            {
                asyncContinuation(null);
            }
        }
    }
}

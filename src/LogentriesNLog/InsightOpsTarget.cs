using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

using InsightOpsCore.Net;

namespace NLog.Targets
{
    [Target("InsightOps")]
    public sealed class InsightOpsTarget : TargetWithLayout
    {
        private AsyncLogger insightOpsAsync;

        public InsightOpsTarget()
        {
            insightOpsAsync = new AsyncLogger();
        }

        
        /** Debug flag. */
        public bool Debug 
        {
            get { return insightOpsAsync.getDebug(); }
            set { insightOpsAsync.setDebug(value); } 
        }

        /** Is using DataHub parameter flag. - ste to true if it is needed to send messages to DataHub instance. */
        public bool IsUsingDataHub
        {
            get { return insightOpsAsync.getIsUsingDataHab(); }
            set { insightOpsAsync.setIsUsingDataHub(value); }
        }

        /** DataHub server address */
        public String DataHubAddr
        {
            get { return insightOpsAsync.getDataHubAddr(); }
            set { insightOpsAsync.setDataHubAddr(value); }
        }

        /** DataHub server port */
        public int DataHubPort
        {
            get { return insightOpsAsync.getDataHubPort(); }
            set { insightOpsAsync.setDataHubPort(value); }
        }

        /** Option to set Token programmatically or in Appender Definition */
        public string Token
        {
            get { return insightOpsAsync.getToken(); }
            set { insightOpsAsync.setToken(value); }
        }

        /** SSL/TLS parameter flag */
        public bool Ssl
        {
            get { return insightOpsAsync.getUseSsl(); }
            set { insightOpsAsync.setUseSsl(value); }
        }

        /** ACCOUNT_KEY parameter for HTTP PUT logging */
        public String Key
        {
            get { return insightOpsAsync.getAccountKey(); }
            set { insightOpsAsync.setAccountKey(value); }
        }

        /** LOCATION parameter for HTTP PUT logging */
        public String Location
        {
            get { return insightOpsAsync.getLocation(); }
            set { insightOpsAsync.setLocation(value); }
        }

        /* LogHostname - switch that defines whether add host name to the log message */
        public bool LogHostname
        {
            get { return insightOpsAsync.getUseHostName(); }
            set { insightOpsAsync.setUseHostName(value); }
        }

        /* HostName - user-defined host name. If empty the library will try to obtain it automatically */
        public String HostName
        {
            get { return insightOpsAsync.getHostName(); }
            set { insightOpsAsync.setHostName(value); }
        }

        /* User-defined log message ID */
        public String LogID
        {
            get { return insightOpsAsync.getLogID(); }
            set { insightOpsAsync.setLogID(value); }
        }

		/* User-defined log message ID */
		public String Region
		{
            get { return insightOpsAsync.getRegion(); }
			set { insightOpsAsync.setRegion(value); }
		}

        public bool KeepConnection { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            //Render message content
            String renderedEvent = this.Layout.Render(logEvent);

            insightOpsAsync.AddLine(renderedEvent);
        }

        protected override void CloseTarget()
        {
            base.CloseTarget();

            insightOpsAsync.interruptWorker();
        }
    }
}

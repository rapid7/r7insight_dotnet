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
    public sealed class InsightTarget : TargetWithLayout
    {
        private AsyncLogger insightAsync;

        public InsightTarget()
        {
            insightAsync = new AsyncLogger();
        }

        
        /** Debug flag. */
        public bool Debug 
        {
            get { return insightAsync.getDebug(); }
            set { insightAsync.setDebug(value); } 
        }

        /** Is using DataHub parameter flag. - ste to true if it is needed to send messages to DataHub instance. */
        public bool IsUsingDataHub
        {
            get { return insightAsync.getIsUsingDataHab(); }
            set { insightAsync.setIsUsingDataHub(value); }
        }

        /** DataHub server address */
        public String DataHubAddr
        {
            get { return insightAsync.getDataHubAddr(); }
            set { insightAsync.setDataHubAddr(value); }
        }

        /** DataHub server port */
        public int DataHubPort
        {
            get { return insightAsync.getDataHubPort(); }
            set { insightAsync.setDataHubPort(value); }
        }

        /** Option to set Token programmatically or in Appender Definition */
        public string Token
        {
            get { return insightAsync.getToken(); }
            set { insightAsync.setToken(value); }
        }

        /** SSL/TLS parameter flag */
        public bool Ssl
        {
            get { return insightAsync.getUseSsl(); }
            set { insightAsync.setUseSsl(value); }
        }

        /** ACCOUNT_KEY parameter for HTTP PUT logging */
        public String Key
        {
            get { return insightAsync.getAccountKey(); }
            set { insightAsync.setAccountKey(value); }
        }

        /** LOCATION parameter for HTTP PUT logging */
        public String Location
        {
            get { return insightAsync.getLocation(); }
            set { insightAsync.setLocation(value); }
        }

        /* LogHostname - switch that defines whether add host name to the log message */
        public bool LogHostname
        {
            get { return insightAsync.getUseHostName(); }
            set { insightAsync.setUseHostName(value); }
        }

        /* HostName - user-defined host name. If empty the library will try to obtain it automatically */
        public String HostName
        {
            get { return insightAsync.getHostName(); }
            set { insightAsync.setHostName(value); }
        }

        /* User-defined log message ID */
        public String LogID
        {
            get { return insightAsync.getLogID(); }
            set { insightAsync.setLogID(value); }
        }

        /* User-defined log message ID */
        public String Region
        {
            get { return insightAsync.getRegion(); }
            set { insightAsync.setRegion(value); }
        }

        public bool KeepConnection { get; set; }

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
    }
}

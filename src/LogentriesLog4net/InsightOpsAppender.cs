using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;

using InsightOpsCore.Net;

namespace log4net.Appender
{
    public class InsightOpsAppender : AppenderSkeleton
    {
        private AsyncLogger insightOpsAsync;

        public InsightOpsAppender()
        {
            insightOpsAsync = new AsyncLogger();
        }

        #region attributeMethods

        /* Option to set LOGENTRIES_TOKEN programmatically or in appender definition. */
        public string Token
        {
            get
            {
                return insightOpsAsync.getToken();
            }
            set
            {
                insightOpsAsync.setToken(value);
            }
        }

        /* Option to set LOGENTRIES_ACCOUNT_KEY programmatically or in appender definition. */
        public String AccountKey
        {
            get
            {
                return insightOpsAsync.getAccountKey();
            }
            set
            {
                insightOpsAsync.setAccountKey(value);
            }
        }

        /* Option to set LOGENTRIES_LOCATION programmatically or in appender definition. */
        public String Location
        {
            get
            {
                return insightOpsAsync.getLocation();
            }
            set
            {
                insightOpsAsync.setLocation(value);
            }
        }

        /* Set to true to always flush the TCP stream after every written entry. */
        public bool ImmediateFlush
        {
            get
            {
                return insightOpsAsync.getImmediateFlush();
            }
            set
            {
                insightOpsAsync.setImmediateFlush(value);
            }
        }

        /* Debug flag. */
        public bool Debug
        {
            get
            {
                return insightOpsAsync.getDebug();
            }
            set
            {
                insightOpsAsync.setDebug(value);
            }
        }

        /* Set to true to use SSL with HTTP PUT logging. */
        public bool UseSsl
        {
            get
            {
                return insightOpsAsync.getUseSsl();
            }
            set
            {
                insightOpsAsync.setUseSsl(value);
            }
        }

        /* Is using DataHub parameter flag. - set to true to use DataHub server */
        public bool IsUsingDataHub
        {
            get 
            { 
                return insightOpsAsync.getIsUsingDataHab(); 
            }
            set 
            { 
                insightOpsAsync.setIsUsingDataHub(value); 
            }
        }

        /* DataHub server address */
        public String DataHubAddr
        {
            get 
            { 
                return insightOpsAsync.getDataHubAddr(); 
            }
            set 
            { 
                insightOpsAsync.setDataHubAddr(value); 
            }
        }

        /* DataHub server port */
        public int DataHubPort
        {
            get 
            { 
                return insightOpsAsync.getDataHubPort(); 
            }
            set 
            { 
                insightOpsAsync.setDataHubPort(value); 
            }
        }

        /* Switch that defines whether add host name to the log message */
        public bool LogHostname
        {
            get
            {
                return insightOpsAsync.getUseHostName();
            }
            set
            {
                insightOpsAsync.setUseHostName(value);
            }
        }

        /* User-defined host name. If empty the library will try to obtain it automatically */
        public String HostName
        {
            get
            {
                return insightOpsAsync.getHostName();
            }
            set
            {
                insightOpsAsync.setHostName(value);
            }
        }

        /* User-defined log message ID */
        public String LogID
        {
            get
            {
                return insightOpsAsync.getLogID();
            }
            set
            {
                insightOpsAsync.setLogID(value);
            }
        }

        /* This property exists for backward compatibility with older configuration XML. */
        [Obsolete("Use the UseSsl property instead.")]
        public bool Ssl
        {
            get
            {
                return insightOpsAsync.getUseSsl();
            }
            set
            {
                insightOpsAsync.setUseSsl(value);
            }
        }

		/* User-defined region */
		public String Region
		{
			get
			{
                return insightOpsAsync.getRegion();
			}
			set
			{
                insightOpsAsync.setRegion(value);
			}
		}

        #endregion

        protected override void Append(LoggingEvent loggingEvent)
        {
            var renderedEvent = RenderLoggingEvent(loggingEvent);
            insightOpsAsync.AddLine(renderedEvent);
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

        protected override void OnClose()
        {
            insightOpsAsync.interruptWorker();
        }
    }
}

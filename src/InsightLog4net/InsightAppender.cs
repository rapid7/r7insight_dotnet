using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;

using InsightCore.Net;

namespace log4net.Appender
{
    public class InsightAppender : AppenderSkeleton
    {
        private AsyncLogger insightAsync;

        public InsightAppender()
        {
            insightAsync = new AsyncLogger();
        }

        #region attributeMethods

        /* Option to set LOGENTRIES_TOKEN programmatically or in appender definition. */
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

        /* Option to set LOGENTRIES_ACCOUNT_KEY programmatically or in appender definition. */
        public String AccountKey
        {
            get
            {
                return insightAsync.getAccountKey();
            }
            set
            {
                insightAsync.setAccountKey(value);
            }
        }

        /* Option to set LOGENTRIES_LOCATION programmatically or in appender definition. */
        public String Location
        {
            get
            {
                return insightAsync.getLocation();
            }
            set
            {
                insightAsync.setLocation(value);
            }
        }

        /* Set to true to always flush the TCP stream after every written entry. */
        public bool ImmediateFlush
        {
            get
            {
                return insightAsync.getImmediateFlush();
            }
            set
            {
                insightAsync.setImmediateFlush(value);
            }
        }

        /* Debug flag. */
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

        /* Set to true to use SSL with HTTP PUT logging. */
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

        /* Is using DataHub parameter flag. - set to true to use DataHub server */
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

        /* DataHub server address */
        public String DataHubAddr
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

        /* DataHub server port */
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

        /* Switch that defines whether add host name to the log message */
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

        /* User-defined host name. If empty the library will try to obtain it automatically */
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

        /* User-defined log message ID */
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

        /* This property exists for backward compatibility with older configuration XML. */
        [Obsolete("Use the UseSsl property instead.")]
        public bool Ssl
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

        /* User-defined region */
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

        #endregion

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

        protected override void OnClose()
        {
            insightAsync.interruptWorker();
        }
    }
}

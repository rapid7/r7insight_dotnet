using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace InsightCore.Net
{
    class InsightClient
    {
        // Logentries API server address. 
        protected const String LeDataUrl = "{0}.data.logs.insight.rapid7.com";

        // Port number for logging on Logentries DATA server. 
        protected const int LeUnsecurePort = 80;

        // Port number for SSL logging on Logentries DATA server. 
        protected const int LeSecurePort = 443;

        // Creates LeClient instance. If do not define useServerUrl and/or useOverrideProt during call
        // LeClient will be configured to work with api.logentries.com server; otherwise - with
        // defined server on defined port.
        public InsightClient(bool useSsl, bool useDataHub, String serverAddr, int port, String region)
        {
           
            // Override port number and server address to send logs to DataHub instance.
            if (useDataHub)
            {
                m_UseSsl = false; // DataHub does not support receiving log messages over SSL for now.
                m_TcpPort = port;
                m_ServerAddr = serverAddr;
            }
            else
            {
                m_UseSsl = useSsl;

                if (!m_UseSsl)
                    m_TcpPort = LeUnsecurePort;
                else
                    m_TcpPort = LeSecurePort;

                m_ServerAddr = String.Format(LeDataUrl, region); // By default m_ServerAddr points to api.logentries.com if useDataHub is not set to true.
            }
        }

        private bool m_UseSsl = false;
        private int m_TcpPort;
        private TcpClient m_Client = null;
        private Stream m_Stream = null;
        private SslStream m_SslStream = null;
        private String m_ServerAddr = null;

        private Stream ActiveStream
        {
            get
            {
                return m_UseSsl ? m_SslStream : m_Stream;
            }
        }

        public void SetSocketKeepAliveValues(TcpClient tcpc, int KeepAliveTime, int KeepAliveInterval)
        {
            //KeepAliveTime: default value is 2hr
            //KeepAliveInterval: default value is 1s and Detect 5 times

            uint dummy = 0; //lenth = 4
            byte[] inOptionValues = new byte[System.Runtime.InteropServices.Marshal.SizeOf(dummy) * 3]; //size = lenth * 3 = 12
            bool OnOff = true;

            BitConverter.GetBytes((uint)(OnOff ? 1 : 0)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)KeepAliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)KeepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            // of course there are other ways to marshal up this byte array, this is just one way
            // call WSAIoctl via IOControl

            // .net 1.1 type
            //int SIO_KEEPALIVE_VALS = -1744830460; //(or 0x98000004)
            //socket.IOControl(SIO_KEEPALIVE_VALS, inOptionValues, null); 

            // .net 3.5 type
            tcpc.Client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        public void Connect()
        {
            m_Client = new TcpClient(m_ServerAddr, m_TcpPort);
            m_Client.NoDelay = true;

            // on Azure sockets will be closed after some minutes idle.
            // which for some reason messes up this library causing it to lose data.
            
            // turn on keepalive, to keep the sockets open.
            // I don't really understand why this helps the problem, since the socket already has NoDelay set
            // so data should be sent immediately. And indeed it does appear to be sent promptly when it works.
            m_Client.Client.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // set timeouts to 10 seconds idle before keepalive, 1 second between repeats, 
            SetSocketKeepAliveValues(m_Client, 10 *1000, 1000);
            
            m_Stream = m_Client.GetStream();

            if (m_UseSsl)
            {
                m_SslStream = new SslStream(m_Stream);
                m_SslStream.AuthenticateAsClient(m_ServerAddr);

            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            ActiveStream.Write(buffer, offset, count);
        }

        public void Flush()
        {
            ActiveStream.Flush();
        }

        public void Close()
        {
            if (m_Client != null)
            {
                try
                {
                    m_Client.Close();
                }
                catch
                {
                }
            }
        }
    }
}

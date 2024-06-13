using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace InsightCore.Net
{
    class InsightClient
    {
        // Rapid7 Insight API server address. 
        protected const string LeDataUrl = "{0}.data.logs.insight.rapid7.com";

        // Port number for logging on Rapid7 Insight DATA server. 
        protected const int LeUnsecurePort = 80;

        // Port number for SSL logging on Rapid7 Insight DATA server. 
        protected const int LeSecurePort = 443;

        public InsightClient(bool useSsl, bool useDataHub, string serverAddr, int port, string region)
        {
            if (useDataHub)
            {
                m_UseSsl = false; // DataHub does not support receiving log messages over SSL for now.
                TcpPort = port;
                ServerAddr = serverAddr;
            }
            else
            {
                m_UseSsl = useSsl;
                TcpPort = m_UseSsl ? LeSecurePort : LeUnsecurePort;
                ServerAddr = string.Format(LeDataUrl, region);
            }
        }

        private bool m_UseSsl = false;
        public int TcpPort { get; private set; }
        private TcpClient m_Client = null;
        private Stream m_Stream = null;
        private SslStream m_SslStream = null;
        public string ServerAddr { get; private set; }

        private Stream ActiveStream => m_UseSsl ? m_SslStream : m_Stream;

        public void SetSocketKeepAliveValues(TcpClient tcpc, int keepAliveTime, int keepAliveInterval)
        {
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            bool onOff = true;

            BitConverter.GetBytes((uint)(onOff ? 1 : 0)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)keepAliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)keepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

            tcpc.Client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        public void Connect()
        {
            m_Client = new TcpClient();
            m_Client.Connect(ServerAddr, TcpPort);
            m_Client.NoDelay = true;

            m_Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            try
            {
                SetSocketKeepAliveValues(m_Client, 10 * 1000, 1000);
            }
            catch (PlatformNotSupportedException)
            {
                // .NET on Linux does not support modification of that settings at the moment. Defaults applied.
            }

            m_Stream = m_Client.GetStream();

            if (m_UseSsl)
            {
                m_SslStream = new SslStream(m_Stream);
                m_SslStream.AuthenticateAsClient(ServerAddr);
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            ActiveStream.Write(buffer, offset, count);
            ActiveStream.Flush();
        }

        public void Close()
        {
            if (m_Client != null)
            {
                try
                {
                    m_Client.Dispose();
                }
                catch
                {
                }
                finally
                {
                    m_Client = null;
                    m_Stream = null;
                    m_SslStream = null;
                }
            }
        }
    }
}

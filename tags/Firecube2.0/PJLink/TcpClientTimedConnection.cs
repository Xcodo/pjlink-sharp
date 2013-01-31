using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace rv
{
    /// <summary>
    /// A custom TcpClient class that cancels connection attempts after the specified timeout.
    /// </summary>
    public class TcpClientTimedConnection : TcpClient
    {
        /// <summary>
        /// Connects to a remote host.
        /// </summary>
        /// <param name="hostname">The hostname of the host to connect to.</param>
        /// <param name="port">The port of the host to connect to.</param>
        /// <param name="connectTimeout">
        /// The timeout after which to cancel the connection attempt. 
        /// A <see cref="SocketException"/> is thrown if the connection cannot be established in the specified timeout.
        /// </param>
        public void Connect(string hostname, int port, int connectTimeout)
        {
            IPAddress address;
            if (!IPAddress.TryParse(hostname, out address))
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
                if (hostEntry.AddressList.Length > 0)
                    Connect(hostEntry.AddressList[0], port, connectTimeout);
                else
                    // 11004 = WSANO_DATA
                    throw new SocketException(11004);
            }
            else
                Connect(address, port, connectTimeout); 
        }

        /// <summary>
        /// Connects to a remote host.
        /// </summary>
        /// <param name="address">The address of the host to connect to.</param>
        /// <param name="port">The port of the host to connect to.</param>
        /// <param name="connectTimeout">
        /// The timeout after which to cancel the connection attempt. 
        /// A <see cref="SocketException"/> is thrown if the connection cannot be established in the specified timeout.
        /// </param>
        public void Connect(IPAddress address, int port, int connectTimeout)
        {
            IAsyncResult result = this.BeginConnect(address, port, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(connectTimeout);
            if (!success || !this.Client.Connected)
            {
                try
                {
                    this.Client.Close();
                }
                catch (Exception)
                {
                }

                // 10060 = WSAETIMEDOUT
                throw new SocketException(10060);
            }
        }
    }
}

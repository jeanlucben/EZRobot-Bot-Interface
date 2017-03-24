///    Synbot Plugin Plugin for EZB Builder Software intefacing With Synthetic Intelligence Network SYNBOT Framework
///    Copyright(C) 2016  Jean-Luc BENARD
///    This program is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or any later version.
///    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
///    GNU General Public License for more details.
///    You should have received a copy of the GNU General Public License along with this program.If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clienttcp
{
    class Clienttcp
    {
        private string adress;
        private string portTCP;
        private TcpClient _tcpClient = null;

        public Clienttcp(string hostadress , string hostport)
        {
            adress = hostadress;
            if (adress == "") adress = GetIPs();
            portTCP = hostport;  
        }
        /// <summary>
        ///  Get Local Host IP adress
        /// </summary>
        public string GetIPs()
        {
            string Iphost = "";
            System.Net.IPHostEntry _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork") Iphost = _IPAddress.ToString();
            }
            return Iphost;
        }
        private void connect()
        {
            int port = Convert.ToInt32(portTCP);
            string IPadr = adress;
            if (IPadr == "") IPadr = GetIPs();
            _tcpClient = new TcpClient();
            IAsyncResult ar = _tcpClient.BeginConnect(IPadr, port, null, null);
            System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
            try
            {
                if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3), false))
                {
                    _tcpClient.Close();
                    throw new TimeoutException();
                }
                _tcpClient.EndConnect(ar);
            }
            finally
            {
                wh.Close();
            }

            _tcpClient.NoDelay = true;
            _tcpClient.ReceiveTimeout = 2000;
            _tcpClient.SendTimeout = 2000;
        }
        private void disconnect()
        {
            if (_tcpClient != null)
                _tcpClient.Close();

            _tcpClient = null;
        }
        /// <summary>
        /// Send command string to the Synbot server connection.
        /// The input buffer will be cleared, then it will send the command, finally it reads the response.
        /// There is always a response from the EZ-Builder Script Interface:
        ///  - If you are expecting to receive data, the response will be the received data.
        ///  - If you are not expecting to receive data, the response will be "OK"
        /// </summary>
        private string sendCommand(string cmd)
        {
            try
            {
                clearInputBuffer();
                _tcpClient.Client.Send(System.Text.Encoding.ASCII.GetBytes(cmd + Environment.NewLine));
                return readResponseLine();
            }
            catch (Exception ex)
            {
                //EZ_Builder.Invokers.SetAppendText(botConsole, true, "Communication with Synbot Server Error {0} ", ex);
                disconnect();
            }
            return string.Empty;
        }
        /// Clears any data in the tcp incoming buffer by reading the buffer into an empty byte array.
        private void clearInputBuffer()
        {

            if (_tcpClient.Available > 0)
                _tcpClient.GetStream().Read(new byte[_tcpClient.Available], 0, _tcpClient.Available);
        }
        /// Blocks and waits for a string of data to be sent. The string is terminated with a \r\n
        private string readResponseLine()
        {
            string str = string.Empty;
            do
            {
                byte[] tmpBuffer = new byte[1024];
                _tcpClient.GetStream().Read(tmpBuffer, 0, tmpBuffer.Length);
                str += System.Text.Encoding.ASCII.GetString(tmpBuffer);
            } while (!str.Contains(Environment.NewLine));
            // Return only the first line if multiple lines were received
            return str.Substring(0, str.IndexOf(Environment.NewLine));
        }
    }
}

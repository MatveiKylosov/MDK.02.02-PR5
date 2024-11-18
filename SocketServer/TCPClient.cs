using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public class TCPClient
    {
        public readonly TcpClient Client;
        private string token = null;
        public bool RequestToDisconnect = false;
        public string Token { 
            get 
            {
                if (token == null)
                {
                    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                    var random = new Random();
                    var token = new StringBuilder(16);

                    for (int i = 0; i < 16; i++)
                    {
                        token.Append(chars[random.Next(chars.Length)]);
                    }

                    return token.ToString();
                }

                return token;
            } 
        }

        private string ipAddress;
        public string IPAddress { get 
            {
                if (ipAddress == null)
                {
                    IPEndPoint ep = Client.Client.RemoteEndPoint as IPEndPoint;
                    return ep.Address.ToString();
                }
                return ipAddress; 
            } 
        }

        public TCPClient(TcpClient Client)
        {
            this.Client = Client;
        }
    }
}

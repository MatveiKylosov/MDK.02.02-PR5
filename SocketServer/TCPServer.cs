using MySqlX.XDevAPI;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    public class TCPServer
    {
        private readonly TcpListener TCPListener;
        private bool IsRunning;
        private readonly ConcurrentBag<TCPClient> Clients = new ConcurrentBag<TCPClient>();
        private int QuntityClients;
        private int TokenLifetime;

        public TCPServer(string ip = "", int port = 1337, int quantityClients = 1, int tokenLifetime = 60)
        {
            var ipServer = ip == "" ? IPAddress.Any : IPAddress.Parse(ip);
            TCPListener = new TcpListener(ipServer, port);
            IsRunning = false;

            QuntityClients = quantityClients;
            TokenLifetime = tokenLifetime;
        }

        public async Task<bool> InitServer()
        {
            try
            {
                TCPListener.Start();
                IsRunning = true;
                Console.WriteLine("Сервер запущен...");

                while (IsRunning)
                {
                    if (Clients.Count() == QuntityClients)
                    {
                        continue;
                    }

                    try
                    {
                        var client = await TCPListener.AcceptTcpClientAsync();
                        var tcpClient = new TCPClient(client);
                        Clients.Add(tcpClient);

                        await HandleClientAsync(tcpClient);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка подключения клиента: {ex.Message}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка запуска сервера: {ex.Message}");
                return false;
            }
        }

        private async Task HandleClientAsync(TCPClient client)
        {
            using (var stream = client.Client.GetStream())
            {
                bool tokenExpired = false;
                Task timerTask = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(TokenLifetime));
                    tokenExpired = true; Console.WriteLine("Время жизни токена истекло.");
                });

                try
                {
                    while (IsRunning && !tokenExpired)
                    {
                        try
                        {
                            //...
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Console.WriteLine($"Ошибка обработки клиента: {ex.Message}");
#endif
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
#endif
                }
                finally
                {
                    if (tokenExpired)
                    {
                        Disconnect(client);
                    }

                    await timerTask;
                }
            }
        }


        public void Disconnect(TCPClient client)
        {   //Под сомнением.
            TCPClient removedClient;
            client.Client.Close();
            Clients.TryTake(out removedClient);
        }

        ~TCPServer()
        {
            IsRunning = false;
            TCPListener.Stop();

            foreach (var client in Clients)
            {
                client.Client.Close();
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    public class TCPServer
    {
        private readonly TcpListener TCPListener;
        private bool IsRunning;
        private readonly ConcurrentBag<TcpClient> Clients = new ConcurrentBag<TcpClient>();
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

                // Основной цикл прослушивания
                while (IsRunning)
                {
                    if(Clients.Count() == QuntityClients)
                    {
                        //Давать отворот клиентам
                        continue;
                    }    

                    try
                    {
                        var client = await TCPListener.AcceptTcpClientAsync();
                        Clients.Add(client); // Добавляем клиента в список
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

        public void Stop()
        {
            IsRunning = false;
            TCPListener.Stop();

            // Закрываем все подключения
            foreach (var client in Clients)
            {
                client.Close();
            }
        }
    }
}

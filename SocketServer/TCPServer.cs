﻿
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        private ConcurrentBag<TCPClient> Clients = new ConcurrentBag<TCPClient>();
        private int QuntityClients;
        private int TokenLifetime;

        private async Task AcceptClientAsync()
        {
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
        }

        private async Task HandleClientAsync(TCPClient client)
        {
            using (var stream = client.Client.GetStream())
            {
                bool tokenExpired = false;

                using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                {
                    await writer.WriteLineAsync(client.Token);
                    Console.WriteLine($"Токен отправлен клиенту: {client.Token}");
                }

                Task timerTask = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(TokenLifetime));
                    tokenExpired = true;
                    client.duration++;
                });

                try
                {
                    while (IsRunning && !tokenExpired && client.Client.Connected)
                    {
                        if (!client.Client.Connected)
                        {
                            Console.WriteLine($"Клиент {client.IPAddress} с токеном {client.Token} отключился.");
                            Clients.TryTake(out var removedClient);  // Удаляем клиента из списка
                            break;  // Прерываем цикл, если соединение закрыто
                        }

                        if (client.RequestToDisconnect)
                        {
                            Console.WriteLine($"Клиент {client.IPAddress} был принудительно отключён.");
                            Disconnect(client);
                            break;  // Выход из цикла, если клиент принудительно отключен
                        }

                        await Task.Delay(500);  // небольшой таймаут, чтобы избежать излишней загрузки процессора
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
                }
                finally
                {
                    if (tokenExpired)
                    {
                        Console.WriteLine($"Время жизни токена {client.Token} истекло у клиента {client.IPAddress}.");
                        Disconnect(client);
                    }

                    await timerTask;  // Убедимся, что таймер завершился
                }
            }
        }

        private void Disconnect(TCPClient client)
        {
            try
            {
                client.Client.Close();  // Закрываем соединение
                Clients.TryTake(out var removedClient);  // Удаляем клиента из списка
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отключении клиента: {ex.Message}");
            }
        }



        public TCPServer(string ip = "", int port = 1337, int quantityClients = 1, int tokenLifetime = 60)
        {
            var ipServer = ip == "" ? IPAddress.Any : IPAddress.Parse(ip);
            TCPListener = new TcpListener(ipServer, port);
            IsRunning = false;

            QuntityClients = quantityClients;
            TokenLifetime = tokenLifetime;
        }

        public bool Start()
        {
            try
            {
                TCPListener.Start();
                IsRunning = true;
                _ = AcceptClientAsync();
                Console.WriteLine("Сервер запущен...");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка запуска сервера: {ex.Message}");
                return false;
            }
        }

        public void Disconnect(string token)
        {
            var client = Clients.First(x => x.Token == token);
            client.RequestToDisconnect = true;
        }

        public List<string> GetAllTokens()
        {
            var list = new List<string>();
            list.AddRange(Clients.Select(x => $"Клиент: {x.Token}, время подключения: {1}, продолжительность подключения {x.duration}").ToArray());
            return list;
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

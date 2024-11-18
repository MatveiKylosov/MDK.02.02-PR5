using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    public class TCPClient
    {
        private readonly string serverIp;
        private readonly int serverPort;
        private readonly TcpClient tcpClient;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;
        private CancellationTokenSource cts;

        public TCPClient(string ip, int port)
        {
            serverIp = ip;
            serverPort = port;
            tcpClient = new TcpClient();
            cts = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            try
            {
                await tcpClient.ConnectAsync(serverIp, serverPort);
                Console.WriteLine($"Подключено к серверу {serverIp}:{serverPort}");

                stream = tcpClient.GetStream();
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                reader = new StreamReader(stream, Encoding.UTF8);

                // Получаем токен от сервера
                string token = await reader.ReadLineAsync();
                Console.WriteLine($"Получен токен: {token}");

                // Обработка команд пользователя
                await ProcessCommandsAsync(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подключении или получении данных: {ex.Message}");
            }
        }

        private async Task ProcessCommandsAsync(string token)
        {
            string input;
            while (!cts.Token.IsCancellationRequested)
            {
                Console.WriteLine("Введите команду (exit для выхода):");
                input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    cts.Cancel();
                    Console.WriteLine("Выход из клиента...");
                    break;
                }

                // Отправляем команду на сервер
                try
                {
                    await writer.WriteLineAsync(input);
                    Console.WriteLine($"Команда отправлена: {input}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке команды: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            cts.Cancel();
            tcpClient.Close();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string serverIp = "127.0.0.1";
            int serverPort = 1337;

            var client = new TCPClient(serverIp, serverPort);
            await client.StartAsync();

            // Ожидаем завершения работы клиента
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
            client.Stop();
        }
    }
}

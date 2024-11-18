using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClientExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Адрес сервера
            string serverAddress = "127.0.0.1";
            int serverPort = 1337;

            try
            {
                // Подключаемся к серверу
                using (var client = new TcpClient(serverAddress, serverPort))
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Читаем строку от сервера
                    string serverResponse = await reader.ReadLineAsync();
                    Console.WriteLine("Ответ от сервера: " + serverResponse);
                     serverResponse = await reader.ReadLineAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подключении к серверу: {ex.Message}");
            }
        }
    }
}

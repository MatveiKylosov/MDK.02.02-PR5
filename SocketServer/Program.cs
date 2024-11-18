using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocketServer
{
    internal class Program
    {

        static private void PrintLogo()
        {
            Console.WriteLine(@"           /$$                 /$$                                 ");
            Console.WriteLine(@"          | $$                | $$                                 ");
            Console.WriteLine(@"  /$$$$$$ | $$$$$$$   /$$$$$$ | $$$$$$$   /$$$$$$                  ");
            Console.WriteLine(@" |____  $$| $$__  $$ /$$__  $$| $$__  $$ |____  $$                 ");
            Console.WriteLine(@"  /$$$$$$$| $$  \ $$| $$  \ $$| $$  \ $$  /$$$$$$$                 ");
            Console.WriteLine(@" /$$__  $$| $$  | $$| $$  | $$| $$  | $$ /$$__  $$                 ");
            Console.WriteLine(@"|  $$$$$$$| $$$$$$$/|  $$$$$$/| $$$$$$$/|  $$$$$$$                 ");
            Console.WriteLine(@" \_______/|_______/  \______/ |_______/  \_______/                 ");
            Console.WriteLine(@"                                                                   ");
            Console.WriteLine(@"                                                                   ");
            Console.WriteLine(@"                                                                   ");
            Console.WriteLine(@"                                           /$$                     ");
            Console.WriteLine(@"                                          | $$                     ");
            Console.WriteLine(@"                                      /$$$$$$$  /$$$$$$  /$$    /$$");
            Console.WriteLine(@"                                     /$$__  $$ /$$__  $$|  $$  /$$/");
            Console.WriteLine(@"                                    | $$  | $$| $$$$$$$$ \  $$/$$/ ");
            Console.WriteLine(@"                                    | $$  | $$| $$_____/  \  $$$/  ");
            Console.WriteLine(@"                                    |  $$$$$$$|  $$$$$$$   \  $/   ");
            Console.WriteLine(@"                                     \_______/ \_______/    \_/    ");
        }

        /*
         Func<string, T> parse
         Первый параметр отвечает за то, что будет параметром в делегате (называется он in)
         Второй, что получается на выходе (называется он TResult)
         */

        static T GetInput<T>(string prompt, Func<string, T> parse, Func<T, bool> validate)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                var parsedValue = parse(input);
                if (validate(parsedValue))
                {
                    return parsedValue;
                }
            }
        }
        static async Task Main()
        {
            TCPServer tcpServer= new TCPServer();
            PrintLogo();
#if false
            var regex = new Regex(@"^(\d{1,3}\.){3}\d{1,3}$");

            var ip = GetInput("Укажите IP адрес сервера - ", 
                               s => s, 
                               s => regex.IsMatch(s));

            var port = GetInput("Укажите порт сервера - ", 
                                s => int.TryParse(s, out int p) ? p : -1, 
                                p => p > 1025 && p < 65536);

            var quantityClients = GetInput("Укажите максимальное количество клиентов - ",
                                            s => int.TryParse(s, out int q) ? q : -1,
                                            q => q > 0);

            var tokenLifetime = GetInput("Укажите сколько будет активен токен в секундах (60 или более) - ", 
                                          s => int.TryParse(s, out int t) ? t : -1, 
                                          t => t > 60);


            Console.WriteLine($"Итоговые настройки:");
            Console.WriteLine($"IP адрес: {ip}");
            Console.WriteLine($"Порт: {port}");
            Console.WriteLine($"Максимальное количество клиентов: {quantityClients}");
            Console.WriteLine($"Время жизни токена: {tokenLifetime} секунд");
            Console.WriteLine($"\nдля просмотра всех команд используйте команду /help");
#endif
            if(tcpServer.Start())


            while (true)
            {
                var command = Console.ReadLine();
                 
                if(command == "/help")
                {
                    Console.WriteLine($"Список доступных комманд:");
                    Console.WriteLine($"/clear\t\tотчищает консоль.");
                    Console.WriteLine($"/config\t\tобновить настройки сервера.");
                    Console.WriteLine($"/status\t\tсписок подключённых клиентов к серверу.");
                    Console.WriteLine($"/disconnect\tотключить клиента от сервера.");
                    Console.WriteLine($"/help\t\tпросмотр всех команд.");
                }

                else if(command == "/clear")
                {
                    PrintLogo();
                    Console.Clear();
                }

                else if (command == "/config")
                {

                }

                else if (command == "/status")
                {
                    foreach(var x in tcpServer.GetAllTokens()) { 
                        Console.WriteLine(x);
                    }
                }

                else if (command == "/disconnect")
                {

                }
            }
        }
    }
}

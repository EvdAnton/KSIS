using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TCP_WhoIS
{
    class Program
    {
        private const int port = 43;
        private const string server = "whois.networksolutions.com";

        static void Main(string[] args)
        {
            try
            {
                Console.Write("Enter Domen_name: ");
                string domen = Console.ReadLine();

                TcpClient client = new TcpClient();
                client.Connect(server, port);

                byte[] data = Encoding.ASCII.GetBytes(domen  + "\r\n");

                var stream = client.GetStream();
                stream.Write(data, 0, domen.Length);

                using (var reader = new StreamReader(client.GetStream(), Encoding.ASCII))
                    Console.WriteLine(reader.ReadToEnd());

                stream.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            Console.WriteLine("Запрос завершен...");
            Console.Read();
        }
    }
}
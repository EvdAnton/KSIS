using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SocketTcpClient
{
    class Program
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // подключаемся к удаленному хосту
                    socket.Connect(ipPoint);


                    //string message = Console.ReadLine();
                    // преобразуем строку в байты
                    //byte[] data = Encoding.Unicode.GetBytes(message);

                    Console.Write("Введите путь к данным:");
                    string path = Console.ReadLine();

                    var file = new FileInfo(path);
                    Console.WriteLine("Размер до отправки: " + file.Length + " байт");

                    using (FileStream fstream = File.OpenRead(path))
                    {
                        
                        byte[] data = new byte[fstream.Length];
                        fstream.Read(data, 0, data.Length);

                        socket.Send(data);
                        Console.WriteLine(DateTime.Now.ToLongTimeString() + " : отправка сообщения");

                        // получаем ответ
                        data = new byte[256]; // буфер для ответа
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байт

                        do
                        {
                            bytes = socket.Receive(data, data.Length, 0);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (socket.Available > 0);

                        Console.WriteLine(DateTime.Now.ToLongTimeString() + " ответ сервера: " + builder.ToString());
                    }




                    // закрываем сокет
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+ "\n");
                }
            }
        }
    }
}
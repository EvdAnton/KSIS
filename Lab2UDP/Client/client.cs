using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SocketUdpClient
{
    class Program
    {
        static int localPort; // порт приема сообщений
        static int remotePort; // порт для отправки сообщений
        static Socket listeningSocket;

        static void Main(string[] args)
        {
            Console.Write("Введите порт для приема сообщений: ");
            localPort = Int32.Parse(Console.ReadLine());
            Console.Write("Введите порт для отправки сообщений: ");
            remotePort = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Введите путь к файлу");
            //Console.WriteLine();

            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Task listeningTask = new Task(Listen);
                listeningTask.Start();

                // отправка сообщений на разные порты
                while (true)
                {
                    string path = Console.ReadLine();

                    var file = new FileInfo(path);
                    

                    using (FileStream fstream = File.OpenRead(path))
                    {

                        byte[] data = new byte[fstream.Length];
                        fstream.Read(data, 0, data.Length);

                        EndPoint remotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), remotePort);
                        listeningSocket.SendTo(data, remotePoint);
                    }
                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " : отправка сообщения");
                    Console.WriteLine("Размер до отправки: " + file.Length + " байт");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        // поток для приема подключений
        private static void Listen()
        {
            try
            {
                //Прослушиваем по адресу
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), localPort);
                listeningSocket.Bind(localIP);

                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    //адрес, с которого пришли данные
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    do
                    {
                        bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (listeningSocket.Available > 0);

                    using (var fstream = new FileStream("new1.txt", FileMode.Create))
                    {
                        byte[] input = Encoding.Unicode.GetBytes(builder.ToString());
                        fstream.Write(input, 0, input.Length);
                        Console.WriteLine("Размер после принятия: " + input.Length + " байт");
                    }
                    // получаем данные о подключении
                    IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

                    // выводим сообщение
                    //Console.WriteLine("{0}:{1} - {2}", remoteFullIp.Address.ToString(),
                    //    remoteFullIp.Port, builder.ToString());
                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " : отправка сообщения");
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        // закрытие сокета
        private static void Close()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
        }
    }
}
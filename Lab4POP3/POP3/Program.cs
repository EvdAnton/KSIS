using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POP3
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
        
    }

    public class Client
    {
        private int _Port = 110;
        private string _Host = String.Empty;
        private string _UserName = String.Empty;
        private string _Password = String.Empty;
        private Socket _Socket = null;
        Result _ServerResponse;
        public int MessageCount = 0;
        public int MessagesSize = 0;

        public Client(string host, int port, string userName, string password)
        {
            
            if (String.IsNullOrEmpty(host))
                throw new Exception("Необходимо указать адрес pop3-сервера.");
            if (String.IsNullOrEmpty(userName))
                throw new Exception("Необходимо указать логин пользователя.");
            if (String.IsNullOrEmpty(password))
                throw new Exception("Необходимо указать пароль пользователя.");
            this._Host = host;
            this._Password = password;
            this._Port = port;
            this._UserName = userName;

            Connect();
            Command(String.Format("USER {0}", _UserName));
            ReadLine();

            Command(String.Format("PASS {0}", _Password));
            _ServerResponse = ReadLine();

            if (_ServerResponse.IsError)
                throw new Exception(_ServerResponse.ServerMessage);

            Command("STAT");
            _ServerResponse = ReadLine();
            if (_ServerResponse.IsError)
                throw new Exception(_ServerResponse.ServerMessage);
            _ServerResponse.ParseStat(out this.MessageCount, out this.MessagesSize);
        }

        public void Connect()
        {
            IPHostEntry myIPHostEntry = Dns.GetHostEntry(_Host);
            if (myIPHostEntry == null || myIPHostEntry.AddressList == null || myIPHostEntry.AddressList.Length <= 0)
                throw new Exception("Не удалось определить IP-адрес по хосту.");
            var myIPEndPoint = new IPEndPoint(myIPHostEntry.AddressList[0], _Port);
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Socket.ReceiveBufferSize = 512;
            _Socket.Connect(myIPEndPoint);
        }

        public void Command(string cmd)
        {
            if (_Socket == null)
                throw new Exception("Соединение с сервером не установлено. Откройте соединение методом Connect");
            byte[] buf = Encoding.ASCII.GetBytes(String.Format("{0}\r\n", cmd));
            if (_Socket.Send(buf, buf.Length, SocketFlags.None) != buf.Length)
                throw new Exception("При отправке данных удаленному серверу произошла ошибка...");
        }

        public string ReadLine()
        {
            byte[] buf = new byte[_Socket.ReceiveBufferSize];
            var result = new StringBuilder(_Socket.ReceiveBufferSize);
            int s = 0;
            while ((s = _Socket.Receive(buf, _Socket.ReceiveBufferSize, SocketFlags.None)) > 0)
                result.Append(Encoding.ASCII.GetChars(buf, 0, s));
            return result.ToString().TrimEnd("\r\n".ToCharArray());
        }

        public string ReadToEnd()
        {
            byte[] buf = new byte[_Socket.ReceiveBufferSize];
            var result = new StringBuilder(_Socket.ReceiveBufferSize);
            int s = 0;
            while ((s = _Socket.Receive(buf, _Socket.ReceiveBufferSize, SocketFlags.None)) > 0)
                result.Append(Encoding.ASCII.GetChars(buf, 0, s));
            return result.ToString();
        }

        public class Result
        {
            public string Source { get; set; }
            public bool IsError { get; set; }
            public string ServerMessage { get; set; }
            public string Body { get; set; }
            public Result() { }
            public Result(string source)
            {
                this.Source = source;
                this.IsError = source.StartsWith("-ERR");
                Regex myReg = new Regex(@"(\+OK|\-ERR)\s{1}(?<msg>.*)?", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (myReg.IsMatch(source))
                    this.ServerMessage = myReg.Match(source).Groups["msg"].Value;
                else
                    this.ServerMessage = source;
                if (source.IndexOf("\r\n") != -1)
                    this.Body = source.Substring(source.IndexOf("\r\n") + 2, source.Length - source.IndexOf("\r\n") - 2);
                if (this.Body.IndexOf("\r\n\r\n.\r\n") != -1)
                    this.Body = this.Body.Substring(0, this.Body.IndexOf("\r\n\r\n.\r\n"));
            }

            public static implicit operator Result(string value)
            {
                return new Result(value);
            }

            public void ParseStat(out int messagesCount, out int messagesSize)
            {
                var myReg = new Regex(@"(?<count>\d+)\s+(?<size>\d+)");
                Match m = myReg.Match(this.Source);
                int.TryParse(m.Groups["count"].Value, out messagesCount);
                int.TryParse(m.Groups["size"].Value, out messagesSize);
            }
        }
    }
}

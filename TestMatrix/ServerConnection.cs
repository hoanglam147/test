using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;


namespace ServerConnection
{
    using TestMatrix;

    public class UDPServer
    {

        public IPAddress ServerAddr;
        public int Port;

        public string GoodreadPattern
        {
            get
            {
                return goodreadPattern;
            }
            set
            {
                goodreadPattern = WildcardToRegex(value);
                goodReadRGX = new Regex(goodreadPattern, RegexOptions.IgnoreCase);
            }
        }
        public string NoreadPattern
        {
            get
            {
                return noreadPattern;
            }
            set
            {
                noreadPattern = WildcardToRegex(value);
                noReadRGX = new Regex(noreadPattern, RegexOptions.IgnoreCase);
            }
        }
        public string PartialreadPattern
        {
            get
            {
                return partialreadPattern;
            }
            set
            {
                partialreadPattern = WildcardToRegex(value);
                partialReadRGX = new Regex(partialreadPattern, RegexOptions.IgnoreCase);
            }
        }
        public string MultiplereadPattern
        {
            get
            {
                return multiplereadPattern;
            }
            set
            {
                multiplereadPattern = WildcardToRegex(value);
                multipleReadRGX = new Regex(multiplereadPattern, RegexOptions.IgnoreCase);
            }
        }

        public string HeartBeatPattern
        {
            get
            {
                return heartbeatPattern;
            }
            set
            {
                heartbeatPattern = WildcardToRegex(value);
                heartbeatRGX = new Regex(heartbeatPattern, RegexOptions.IgnoreCase);
            }
        }

        public event EventHandler GoodReadPatternFound;
        public event EventHandler NoReadPatternFound;
        public event EventHandler PartialReadPatternFound;
        public event EventHandler MultipleReadPatternFound;
        public event EventHandler HeartBeatPatternFound;

        public UDPServer(IPAddress localaddr, int port, Logger logger)
        {
            //IPAddress.Any
            ServerAddr = localaddr;
            Port = port;
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
            this.logger = logger;
            portLogString = " - UDPServer(p." + Convert.ToString(port) + ") - ";
        }

        public void Open()
        {
            listeningForClients = true;
        }

        public void Close()
        {
            listeningForClients = false;
            listenThread.Join();
            listenThread = null;
        }

        private bool listeningForClients = false;
        private Thread listenThread;
        private Regex goodReadRGX, noReadRGX, partialReadRGX, multipleReadRGX, heartbeatRGX;
        private string goodreadPattern, noreadPattern, partialreadPattern, multiplereadPattern, heartbeatPattern;
        private Logger logger;
        private string portLogString;

        private void ListenForClients()
        {
            Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint localEndPoint = new IPEndPoint(ServerAddr, Port);
            soUdp.Bind(localEndPoint);
            listeningForClients = true;

            while (listeningForClients)
            {
                int bytesReceived = 0;
                Byte[] received = new Byte[4096];
                IPEndPoint tmpEndPoint = new IPEndPoint(ServerAddr, Port);
                EndPoint remoteEP = (tmpEndPoint);
                if (soUdp.Available > 0)
                {
                    try
                    {
                        // blocking???
                        bytesReceived = soUdp.ReceiveFrom(received, ref remoteEP);
                    }
                    catch (System.Exception)
                    {
                        //a socket error has occurred or socket closed server side
                        break;
                    }
                    if (bytesReceived == 0)
                    {
                        //nothing to receive
                        break;
                    }

                    if (bytesReceived != 0)
                    {
                        ASCIIEncoding encoder = new ASCIIEncoding();
                        string receivedMsg = encoder.GetString(received, 0, bytesReceived);
                        CheckAnswer(receivedMsg);
                        if (logger != null)
                        {
                            logger.Log(receivedMsg, portLogString);
                        }
                    }
                }
            }
            soUdp.Close();
        }

        private void HandleClientComm(object client)
        {
        }

        private void CheckAnswer(string answer)
        {
            if (goodReadRGX != null && goodReadRGX.IsMatch(answer))
            {
                if (GoodReadPatternFound != null)
                    GoodReadPatternFound(this, EventArgs.Empty);
            }
            if (noReadRGX != null && noReadRGX.IsMatch(answer))
            {
                if (NoReadPatternFound != null)
                    NoReadPatternFound(this, EventArgs.Empty);
            }
            if (partialReadRGX != null && partialReadRGX.IsMatch(answer))
            {
                if (PartialReadPatternFound != null)
                    PartialReadPatternFound(this, EventArgs.Empty);
            }
            if (multipleReadRGX != null && multipleReadRGX.IsMatch(answer))
            {
                if (MultipleReadPatternFound != null)
                    MultipleReadPatternFound(this, EventArgs.Empty);
            }
            if (heartbeatRGX != null && heartbeatRGX.IsMatch(answer))
            {
                if (HeartBeatPatternFound != null)
                    HeartBeatPatternFound(this, EventArgs.Empty);
            }
        }

        private string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

    }

    public class TCPServer
    {
        public IPAddress ServerAddr;
        public int Port;

        public string GoodreadPattern
        {
            get
            {
                return goodreadPattern;
            }
            set
            {
                goodreadPattern = WildcardToRegex(value);
                goodReadRGX = new Regex(goodreadPattern, RegexOptions.IgnoreCase);
            }
        }
        public string NoreadPattern
        {
            get
            {
                return noreadPattern;
            }
            set
            {
                noreadPattern = WildcardToRegex(value);
                noReadRGX = new Regex(noreadPattern, RegexOptions.IgnoreCase);
            }
        }
        public string PartialreadPattern
        {
            get
            {
                return partialreadPattern;
            }
            set
            {
                partialreadPattern = WildcardToRegex(value);
                partialReadRGX = new Regex(partialreadPattern, RegexOptions.IgnoreCase);
            }
        }
        public string MultiplereadPattern
        {
            get
            {
                return multiplereadPattern;
            }
            set
            {
                multiplereadPattern = WildcardToRegex(value);
                multipleReadRGX = new Regex(multiplereadPattern, RegexOptions.IgnoreCase);
            }
        }

        public string HeartBeatPattern
        {
            get
            {
                return heartbeatPattern;
            }
            set
            {
                heartbeatPattern = WildcardToRegex(value);
                heartbeatRGX = new Regex(heartbeatPattern, RegexOptions.IgnoreCase);
            }
        }

        public event EventHandler GoodReadPatternFound;
        public event EventHandler NoReadPatternFound;
        public event EventHandler PartialReadPatternFound;
        public event EventHandler MultipleReadPatternFound;
        public event EventHandler HeartBeatPatternFound;

        public TCPServer(IPAddress localaddr, int port, Logger logger)
        {
            //IPAddress.Any
            ServerAddr = localaddr;
            Port = port;
            tcpListener = new TcpListener(ServerAddr, Port);
            listenThread = new Thread(new ThreadStart(ListenForClients));
            this.logger = logger;
            portLogString = " - TCPServer(p." + Convert.ToString(port) + ") - ";
        }

        public void Open()
        {
            listenThread.Start();
        }

        public void Close()
        {
            foreach (TcpClient client in clientList)
            {
                client.Close();
            }
            clientList.Clear();

            listeningForClients = false;
            listenThread.Join();
            listenThread = null;
        }

        private bool listeningForClients = false;
        private TcpListener tcpListener;
        private Thread listenThread;
        private Regex goodReadRGX, noReadRGX, partialReadRGX, multipleReadRGX, heartbeatRGX;
        private string goodreadPattern, noreadPattern, partialreadPattern, multiplereadPattern, heartbeatPattern;
        private NetworkStream clientStream;
        private List<TcpClient> clientList;
        private Logger logger;
        private string portLogString;

        private void ListenForClients()
        {
            try
            {
                tcpListener.Start();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (clientList == null)
                clientList = new List<TcpClient>();

            listeningForClients = true;
            while (listeningForClients)
            {
                // iterate until a client has tried to connect to the server
                if (tcpListener.Pending())
                {
                    // blocks until a client has connected to the server
                    TcpClient client = tcpListener.AcceptTcpClient();
                    clientList.Add(client);

                    // create a thread to handle communication with connected client
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                }
            }
            tcpListener.Stop();
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (System.Exception)
                {
                    //a socket error has occurred or socket closed server side
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                if (bytesRead != 0)
                {
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    string receivedMsg = encoder.GetString(message, 0, bytesRead);
                    CheckAnswer(receivedMsg);
                    if (logger != null)
                    {
                        logger.Log(receivedMsg, portLogString);
                    }
                }
            }
            tcpClient.Close();
        }

        private void CheckAnswer(string answer)
        {
            if (goodReadRGX != null && goodReadRGX.IsMatch(answer))
            {
                if (GoodReadPatternFound != null)
                    GoodReadPatternFound(this, EventArgs.Empty);
            }
            if (noReadRGX != null && noReadRGX.IsMatch(answer))
            {
                if (NoReadPatternFound != null)
                    NoReadPatternFound(this, EventArgs.Empty);
            }
            if (partialReadRGX != null && partialReadRGX.IsMatch(answer))
            {
                if (PartialReadPatternFound != null)
                    PartialReadPatternFound(this, EventArgs.Empty);
            }
            if (multipleReadRGX != null && multipleReadRGX.IsMatch(answer))
            {
                if (MultipleReadPatternFound != null)
                    MultipleReadPatternFound(this, EventArgs.Empty);
            }
            if (heartbeatRGX != null && heartbeatRGX.IsMatch(answer))
            {
                if (HeartBeatPatternFound != null)
                    HeartBeatPatternFound(this, EventArgs.Empty);
            }
        }

        private string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

    }
}

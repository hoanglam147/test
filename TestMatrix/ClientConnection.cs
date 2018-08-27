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


namespace ClientConnection
{
    using TestMatrix;

    public class SerialPortClient : IDisposable
    {
        public string portName;
        public int baudRate;
        public Parity parity;
        public int dataBits;
        public StopBits stopBits;
        public Handshake handshake;

        public string goodreadPattern;
        public string noreadPattern;
        public string partialreadPattern;
        public string multiplereadPattern;
        public string phaseIdSeparator;

        public List<Int32> missingPhaseIdxList;
        public List<Int32> outoforderPhaseIdxList;

        public event EventHandler GoodReadPatternFound;
        public event EventHandler NoReadPatternFound;
        public event EventHandler PartialReadPatternFound;
        public event EventHandler MultipleReadPatternFound;
        public event EventHandler OutOfOrderFound;
        public event EventHandler MissingFound;

        public SerialPortClient(string portname,
                                    int baudrate,
                                    Parity parityval,
                                    int databits,
                                    StopBits stopbits,
                                    Handshake flowcontrol,
                                    Logger logger)
        {
            serialReceive = false;
            portName = portname;
            baudRate = baudrate;
            parity = parityval;
            dataBits = databits;
            stopBits = stopbits;
            handshake = flowcontrol;
            missingPhaseIdxList = new List<Int32>();
            outoforderPhaseIdxList = new List<Int32>();
            this.logger = logger;
            portLogString = " - Serial " + portName + ") - ";
        }

        public void SetPatterns(string goodreadpatt,
                                string noreadpatt,
                                string partialreadpatt,
                                string multiplereadpatt,
                                string phaseanalysispatt)
        {
            goodreadPattern = WildcardToRegex(goodreadpatt);
            noreadPattern = WildcardToRegex(noreadpatt);
            partialreadPattern = WildcardToRegex(partialreadpatt);
            multiplereadPattern = WildcardToRegex(multiplereadpatt);
            phaseIdSeparator = phaseanalysispatt;

            goodReadRGX = new Regex(goodreadPattern, RegexOptions.IgnoreCase);
            noReadRGX = new Regex(noreadPattern, RegexOptions.IgnoreCase);
            partialReadRGX = new Regex(partialreadPattern, RegexOptions.IgnoreCase);
            multipleReadRGX = new Regex(multiplereadPattern, RegexOptions.IgnoreCase);
        }

        public bool Open()
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            try
            {
                serialPort.Open();
                serialReceive = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return serialReceive;
        }

        public void Close()
        {
            serialReceive = false;
            if (serialPort != null)
                serialPort.Close();
        }

        public int Send(string message)
        {
            int rc = 0;
            if (serialPort != null)
                serialPort.Write(message);
            return rc;
        }

        public void ManageSerialPort()
        {
            Open();
            while (serialReceive)
            {
                try
                {
                    string message = serialPort.ReadLine();
                    if (logger != null)
                    {
                        logger.Log(message, portLogString);
                    }
                    if (goodReadRGX.IsMatch(message))
                    {
                        GoodReadPatternFound(this, EventArgs.Empty);
                    }
                    if (noReadRGX.IsMatch(message))
                    {
                        NoReadPatternFound(this, EventArgs.Empty);
                    }
                    if (partialReadRGX.IsMatch(message))
                    {
                        PartialReadPatternFound(this, EventArgs.Empty);
                    }
                    if (multipleReadRGX.IsMatch(message))
                    {
                        MultipleReadPatternFound(this, EventArgs.Empty);
                    }
                    // Split string on separator.
                    // ... This will separate all the words.
                    string[] fields = message.Split(phaseIdSeparator.ToCharArray());
                    if (fields.Length > 1)
                    {
                        int phaseId = Convert.ToInt32(fields[0]);
                        if (isFirstPhase)
                        {
                            isFirstPhase = false;
                        }
                        else
                        {
                            if (phaseId > expectedPhaseIdx)
                            {
                                // this is a idx hole - perhaps it will arrive later on
                                for ( ; phaseId == expectedPhaseIdx; expectedPhaseIdx++)
                                {
                                    missingPhaseIdxList.Add(expectedPhaseIdx);
                                    MissingFound(this, EventArgs.Empty);
                                }
                            }
                            if (phaseId < expectedPhaseIdx)
                            {
                                // this is an out-of-order situation
                                // remove this PhaseId from missing list and add it to the out-of-order list
                                for (int i = 0; i < missingPhaseIdxList.Count; i++)
                                {
                                    if (missingPhaseIdxList[i] == phaseId)
                                    {
                                        missingPhaseIdxList.Remove(i);
                                        break;
                                    }
                                }
                                outoforderPhaseIdxList.Add(phaseId);
                                OutOfOrderFound(this, EventArgs.Empty);
                            }
                        }
                        expectedPhaseIdx = phaseId + 1;
                    }
                }
                catch (TimeoutException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception)
                {
                    // channel closed
                }
            }
            Close();
        }

        private SerialPort serialPort;
        private Queue<byte> receivedData = new Queue<byte>();
        private Regex goodReadRGX, noReadRGX, partialReadRGX, multipleReadRGX;
        private bool serialReceive;
        private Int32 expectedPhaseIdx = 1;
        private bool isFirstPhase = true;
        private Logger logger;
        private string portLogString;

        private string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

        public void Dispose()
        {
            if (serialPort != null)
                serialPort.Dispose();
        }
    }

    public class TCPClient
    {
        public IPAddress HostAddr;
        public int HostPort;
        private bool searchForPhaseIdx = false;
        private Int32 expectedPhaseIdx = 1;
        private bool isFirstPhase = true;
        private string phaseIdSeparator;

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
        public event EventHandler OutOfOrderPhaseFound;
        public event EventHandler MissingPhaseFound;

        public TCPClient(IPAddress hostaddr, Int32 hostport, Logger logger)
        {
            HostAddr = hostaddr;
            HostPort = hostport;
            clientThread = new Thread(new ThreadStart(DoConnection));
            missingPhaseIdxList = new List<Int32>();
            outoforderPhaseIdxList = new List<Int32>();
            this.logger = logger;
            portLogString = " - TCPClient(p." + Convert.ToString(HostPort) + ") - ";
        }

        ~TCPClient()
        {
            Close();
        }

        public void Open()
        {
            clientThread.Start();
        }

        public void Close()
        {
            //CloseConnection();

            connectedToServer = false;
            if (clientThread != null)
            {
                clientThread.Join();
                clientThread = null;
            }
        }

        public List<Int32> missingPhaseIdxList;
        public List<Int32> outoforderPhaseIdxList;

        public void SearchForPhaseIndex(string separatorPattern)
        {
            searchForPhaseIdx = true;
            phaseIdSeparator = separatorPattern;
        }

        private ClientConnection host;
        private bool connectedToServer = false;
        private Thread clientThread;

        private Regex goodReadRGX, noReadRGX, partialReadRGX, multipleReadRGX, heartbeatRGX;
        private string goodreadPattern, noreadPattern, partialreadPattern, multiplereadPattern, heartbeatPattern;

        private Logger logger;
        private string portLogString;

        private void DoConnection()
        {
            host = new ClientConnection(HostAddr.ToString(), HostPort);
            if (host.Open())
            {
                connectedToServer = true;
            }
            else
            {
                host = null;
            }

            while (connectedToServer)
            {
                string answer;

                // receive from host
                answer = host.Receive();
                if (answer.Length != 0)
                {
                    CheckAnswer(answer);
                    if (logger != null)
                    {
                        logger.Log(answer, portLogString);
                    }
                }
            }
            CloseConnection();
        }

        public void CloseConnection()
        {
            if (host != null)
            {
                host.Close();
            }
            connectedToServer = false;
        }

        public bool IsConnected()
        {
            return connectedToServer;
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
            if (searchForPhaseIdx)
            {
                // Split string on separator.
                // ... This will separate all the words.
                string[] fields = answer.Split(phaseIdSeparator.ToCharArray());
                if (fields.Length > 1)
                {
                    int phaseId = 0;
                    bool result = false;
                    string phaseField = fields[0];
                    while (!result && phaseField.Length > 0)
                    {
                        result = Int32.TryParse(phaseField, out phaseId);
                        if (!result)
                        {
                            if (phaseField.Length > 1)
                                phaseField = phaseField.Substring(1);
                        }
                    }

                    if (isFirstPhase)
                    {
                        isFirstPhase = false;
                    }
                    else
                    {
                        if (phaseId > expectedPhaseIdx)
                        {
                            // this is a idx hole - perhaps it will arrive later on
                            for (; expectedPhaseIdx < phaseId; expectedPhaseIdx++)
                            {
                                missingPhaseIdxList.Add(expectedPhaseIdx);
                                MissingPhaseFound(this, EventArgs.Empty);
                            }
                        }
                        else if (phaseId < expectedPhaseIdx)
                        {
                            // this is an out-of-order situation
                            // remove this PhaseId from missing list and add it to the out-of-order list
                            for (int i = 0; i < missingPhaseIdxList.Count; i++)
                            {
                                if (missingPhaseIdxList[i] == phaseId)
                                {
                                    missingPhaseIdxList.Remove(i);
                                    break;
                                }
                            }
                            outoforderPhaseIdxList.Add(phaseId);
                            OutOfOrderPhaseFound(this, EventArgs.Empty);
                        }
                    }
                    expectedPhaseIdx = phaseId + 1;
                }
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

    class ClientConnection
    {
        private TcpClient tcpClient;
        private NetworkStream tcpStream;
        private Int32 peerPort = 51236;
        private string peerAddr = "127.0.0.1";

        public ClientConnection(string ipAddr, Int32 port)
        {
            peerPort = port;
            peerAddr = ipAddr;
        }

        public bool Open()
        {
            bool rc = true;
            if (tcpClient == null)
                tcpClient = new TcpClient();
            int count = 0;
            while (tcpClient.Connected == false && count < 20)
            {
                count++;
                try
                {
                    tcpClient.Connect(peerAddr, peerPort);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (tcpClient == null)
                    return false;
                if (tcpClient.Connected)
                    break;
                System.Threading.Thread.Sleep(500);
            }
            if (tcpClient.Connected == false)
                return false;

            // Get a client stream for reading and writing.
            tcpStream = tcpClient.GetStream();
            return rc;
        }

        public void Close()
        {
            if (tcpStream != null)
                tcpStream.Close();
            tcpStream = null;
            if (tcpClient != null)
                tcpClient.Close();
            tcpClient = null;
        }

        public bool Send(string msg2send)
        {
            if (tcpClient == null /*|| tcpClient.Connected == false*/)
            {
                if (Open() == false)
                {
                    MessageBox.Show("Error opening TCP connection!");
                    return false;
                }
            }

            if (tcpStream.CanWrite)
            {
                Byte[] sendBytes = Encoding.UTF8.GetBytes(msg2send);
                try
                {
                    tcpStream.Write(sendBytes, 0, sendBytes.Length);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("You cannot write data to this stream.");
                return false;
            }
            return true;
        }

        public string Receive()
        {
            if (tcpStream == null)
                return string.Empty;

            string recv = string.Empty;

            if (tcpStream.CanRead)
            {
                int recvlen = 0;

                // Reads NetworkStream into a byte buffer.
                byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                try
                {
                    // Read can return anything from 0 to numBytesToRead. 
                    // This method blocks until at least one byte is read.
                    // Or timeout (500mSec) elapses.
                    tcpStream.ReadTimeout = 50;
                    recvlen = tcpStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);
                }
                catch (System.IO.IOException) { }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return string.Empty;
                }

                if (recvlen > 0)
                {
                    recv = Encoding.UTF8.GetString(bytes);
                    int len = recv.IndexOf('\0');
                    recv = recv.Substring(0, len);
                }
                return recv;
            }
            else
            {
                //MessageBox.Show("You cannot read data from this stream.");
            }

            return string.Empty;
        }

    }
}

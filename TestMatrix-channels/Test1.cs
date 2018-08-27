using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Net;

namespace Test1
{
    using TestMatrix_channels;
    using ClientConnection;
    using ServerConnection;

    public class SerialPortItem
    {
        public string portName;
        public int baud;
        public Parity parity;
        public int databits;
        public StopBits stopbits;
        public Handshake hanshake;
    }


    public class Test1LoggingConfigurationParameters
    {
        public bool EnableLogging = false;
        public string LoggingFolder = Environment.CurrentDirectory;
        public bool AppendLogs = false;
        public bool SplitLogFile = false;
        public Int32 LogFileMaxSize = 10;
        public string LogFileName = "TestMatrixLog.txt";
    }

    public class Test1ConfigurationParameters
    {
        public IPAddress DeviceAddr;
        public IPAddress ServerAddr;

        public List<SerialPortItem> serialportArray;

        public void AddSerialPort(string portName, int baud, Parity parity, int databits, StopBits stopbits, Handshake hanshake)
        {
            SerialPortItem item = new SerialPortItem();
            item.portName = portName;
            item.baud = baud;
            item.parity = parity;
            item.databits = databits;
            item.stopbits = stopbits;
            item.hanshake = hanshake;
            if (serialportArray == null)
                serialportArray = new List<SerialPortItem>();
            serialportArray.Add(item);
        }

        public bool TestGoodRead;
        public String GoodRead_pattern;
        public Int32 GoodRead_TCPClient_hostPort;
        public Int32 GoodRead_TCPServer_hostPort;
        public Int32 GoodRead_UDPServer_hostPort;
        public string GoodRead_SerialMain_comPort;
        public string GoodRead_SerialAUX_comPort;

        public bool TestNoRead;
        public String NoRead_pattern;
        public Int32 NoRead_TCPClient_hostPort;
        public Int32 NoRead_TCPServer_hostPort;
        public Int32 NoRead_UDPServer_hostPort;
        public string NoRead_SerialMain_comPort;
        public string NoRead_SerialAUX_comPort;

        public bool TestPartialRead;
        public String PartialRead_pattern;
        public Int32 PartialRead_TCPClient_hostPort;
        public Int32 PartialRead_TCPServer_hostPort;
        public Int32 PartialRead_UDPServer_hostPort;
        public string PartialRead_SerialMain_comPort;
        public string PartialRead_SerialAUX_comPort;

        public bool TestMultipleRead;
        public String MultipleRead_pattern;
        public Int32 MultipleRead_TCPClient_hostPort;
        public Int32 MultipleRead_TCPServer_hostPort;
        public Int32 MultipleRead_UDPServer_hostPort;
        public string MultipleRead_SerialMain_comPort;
        public string MultipleRead_SerialAUX_comPort;

        public bool TestHeartBeat;
        public String HeartBeat_pattern;
        public Int32 HeartBeat_TCPClient_hostPort;
        public Int32 HeartBeat_TCPServer_hostPort;
        public Int32 HeartBeat_UDPServer_hostPort;
        public string HeartBeat_SerialMain_comPort;
        public string HeartBeat_SerialAUX_comPort;

        public bool TestPhaseAnalysis;
        public String PhaseAnalysis_pattern;
        public Int32 PhaseAnalysis_TCPClient_hostPort;
        public Int32 PhaseAnalysis_TCPServer_hostPort;
        public Int32 PhaseAnalysis_UDPServer_hostPort;
        public string PhaseAnalysis_SerialMain_comPort;
        public string PhaseAnalysis_SerialAUX_comPort;
    }

    public class Test1Class
    {
        public event EventHandler Ended;
        public event EventHandler WorkInProgress;

        public event EventHandler CountersIncremented;

        public enum ChannelType
        {
            tcpClient = 0,
            tcpServer,
            udpServer,
            mainPort,
            auxPort,
            MAX_ChannelType
        }

        public Int32 out1Count;
        public Int32 out2Count;
        public Int32 out3Count;
        public Int32 in1Count;
        public Int32 in2Count;
        public Int32[] goodreadCount;
        public Int32[] noreadCount;
        public Int32[] partialreadCount;
        public Int32[] multiplereadCount;
        public Int32[] heartbeatCount;
        public Int32[] totalMessageCount;
        public Int32[] outOfOrderPhaseCount;
        public Int32[] missingPhaseCount;

        public Test1Class(object formmain, Test1ConfigurationParameters config, Test1LoggingConfigurationParameters logcfg)
        {
            formMain = (MainForm)formmain;
            configParams = config;
            logConfig = logcfg;
            goodreadCount = new Int32[(int)ChannelType.MAX_ChannelType];
            noreadCount = new Int32[(int)ChannelType.MAX_ChannelType];
            partialreadCount = new Int32[(int)ChannelType.MAX_ChannelType];
            multiplereadCount = new Int32[(int)ChannelType.MAX_ChannelType];
            heartbeatCount = new Int32[(int)ChannelType.MAX_ChannelType];
            totalMessageCount = new Int32[(int)ChannelType.MAX_ChannelType];
            outOfOrderPhaseCount = new Int32[(int)ChannelType.MAX_ChannelType];
            missingPhaseCount = new Int32[(int)ChannelType.MAX_ChannelType];

            tcpClientArray = new TCPClient[10] { null, null, null, null, null, null, null, null, null, null };
            tcpServerArray = new TCPServer[10] { null, null, null, null, null, null, null, null, null, null };
            udpServerArray = new UDPServer[10] { null, null, null, null, null, null, null, null, null, null };

            ResetCounters();
        }

        ~Test1Class()
        {
            CloseTest();
        }

        public List<Int32> MissingPhaseIndex
        {
            get
            {
                List<Int32> missing = new List<Int32>();
                if (tcpClientPhaseAnalysis != null)
                    missing = tcpClientPhaseAnalysis.missingPhaseIdxList;
                return missing;
            }
        }

        public List<Int32> OutOfOrderPhaseIndex
        {
            get
            {
                List<Int32> outoforder = new List<Int32>();
                if (tcpClientPhaseAnalysis != null)
                    outoforder = tcpClientPhaseAnalysis.outoforderPhaseIdxList;
                return outoforder;
            }
        }

        // private members
        private MainForm formMain;

        private SerialPortListener serialPortMain;
        private Thread serialPortMainThread;

        private SerialPortListener serialPortAUX;
        private Thread serialPortAUXThread;

        private TCPClient[] tcpClientArray;
        private TCPServer[] tcpServerArray;
        private UDPServer[] udpServerArray;

        private TCPClient tcpClientPhaseAnalysis;
        private Thread tcpClientPhaseAnalysisThread;


        private Test1ConfigurationParameters configParams;
        private Test1LoggingConfigurationParameters logConfig;

        private StreamWriter logger = null;

        // Volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        private volatile bool _shouldStop;

        public void DoTest()
        {
            _shouldStop = false;

            OnWorkInProgress(EventArgs.Empty);

            PrepareTest();

            while (!_shouldStop)
            {
                // do nothing...
                System.Threading.Thread.Sleep(10);
            }
            CloseTest();
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        // Invoke the Ended event:
        protected virtual void OnEnded(EventArgs e)
        {
            if (Ended != null)
                Ended(this, e);
        }

        // Invoke the WorkInProgress event:
        protected virtual void OnWorkInProgress(EventArgs e)
        {
            if (WorkInProgress != null)
                WorkInProgress(this, e);
        }

        private void releaseObject(object obj)
        {
            try
            {
                if (obj != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void ResetCounters()
        {
            out1Count = 0;
            out2Count = 0;
            out3Count = 0;
            in1Count = 0;
            in2Count = 0;
            for (int i = 0; i < goodreadCount.Length; i++)
                goodreadCount[i] = 0;
            for (int i = 0; i < noreadCount.Length; i++)
                noreadCount[i] = 0;
            for (int i = 0; i < partialreadCount.Length; i++)
                partialreadCount[i] = 0;
            for (int i = 0; i < multiplereadCount.Length; i++)
                multiplereadCount[i] = 0;
            for (int i = 0; i < heartbeatCount.Length; i++)
                heartbeatCount[i] = 0;
            for (int i = 0; i < totalMessageCount.Length; i++)
                totalMessageCount[i] = 0;
            for (int i = 0; i < outOfOrderPhaseCount.Length; i++)
                outOfOrderPhaseCount[i] = 0;
            for (int i = 0; i < missingPhaseCount.Length; i++)
                missingPhaseCount[i] = 0;
        }

        private int GetTCPClient(int port)
        {
            bool found = false;
            for (int i = 0; i < tcpClientArray.Length; i++)
            {
                if (tcpClientArray[i] != null)
                {
                    if (tcpClientArray[i].HostPort == port)
                    {
                        return i;
                    }
                }
            }
            if (!found)
            {
                for (int i = 0; i < tcpClientArray.Length; i++)
                {
                    if (tcpClientArray[i] == null)
                    {
                        tcpClientArray[i] = new TCPClient(configParams.DeviceAddr, port, logger);
                        return i;
                    }
                }
            }
            return -1;
        }

        private int GetTCPServer(int port)
        {
            bool found = false;
            for (int i = 0; i < tcpServerArray.Length; i++)
            {
                if (tcpServerArray[i] != null)
                {
                    if (tcpServerArray[i].Port == port)
                    {
                        return i;
                    }
                }
            }
            if (!found)
            {
                for (int i = 0; i < tcpServerArray.Length; i++)
                {
                    if (tcpServerArray[i] == null)
                    {
                        tcpServerArray[i] = new TCPServer(configParams.ServerAddr, port, logger);
                        return i;
                    }
                }
            }
            return -1;
        }

        private int GetUDPServer(int port)
        {
            bool found = false;
            for (int i = 0; i < udpServerArray.Length; i++)
            {
                if (udpServerArray[i] != null)
                {
                    if (udpServerArray[i].Port == port)
                    {
                        return i;
                    }
                }
            }
            if (!found)
            {
                for (int i = 0; i < udpServerArray.Length; i++)
                {
                    if (udpServerArray[i] == null)
                    {
                        udpServerArray[i] = new UDPServer(configParams.ServerAddr, port, logger);
                        return i;
                    }
                }
            }
            return -1;
        }

        private bool PrepareTest()
        {
            SerialPortItem serialportMain = null;
            SerialPortItem serialportAUX = null;
            string goodreadpatt = "";
            string noreadpatt = "";
            string partialreadpatt = "";
            string multiplereadpatt = "";
            string phaseanalysispatt = "";

            // setup log files
            if (logConfig.EnableLogging)
            {
                logger = new StreamWriter(logConfig.LoggingFolder + "\\" + logConfig.LogFileName, logConfig.AppendLogs);
            }

            if (configParams.TestGoodRead)
            {
                if (configParams.GoodRead_SerialMain_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportMain = configParams.serialportArray.Find(x => x.portName == configParams.GoodRead_SerialMain_comPort);
                    goodreadpatt = configParams.GoodRead_pattern;
                }
                if (configParams.GoodRead_SerialAUX_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportAUX = configParams.serialportArray.Find(x => x.portName == configParams.GoodRead_SerialAUX_comPort);
                    goodreadpatt = configParams.GoodRead_pattern;
                }
            }
            if (configParams.TestNoRead)
            {
                if (configParams.NoRead_SerialMain_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportMain = configParams.serialportArray.Find(x => x.portName == configParams.NoRead_SerialMain_comPort);
                    noreadpatt = configParams.NoRead_pattern;
                }
                if (configParams.NoRead_SerialAUX_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportAUX = configParams.serialportArray.Find(x => x.portName == configParams.NoRead_SerialAUX_comPort);
                    noreadpatt = configParams.NoRead_pattern;
                }
            }
            if (configParams.TestPartialRead)
            {
                if (configParams.PartialRead_SerialMain_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportMain = configParams.serialportArray.Find(x => x.portName == configParams.PartialRead_SerialMain_comPort);
                    partialreadpatt = configParams.PartialRead_pattern;
                }
                if (configParams.PartialRead_SerialAUX_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportAUX = configParams.serialportArray.Find(x => x.portName == configParams.PartialRead_SerialAUX_comPort);
                    partialreadpatt = configParams.PartialRead_pattern;
                }
            }
            if (configParams.TestMultipleRead)
            {
                if (configParams.MultipleRead_SerialMain_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportMain = configParams.serialportArray.Find(x => x.portName == configParams.MultipleRead_SerialMain_comPort);
                    multiplereadpatt = configParams.MultipleRead_pattern;
                }
                if (configParams.MultipleRead_SerialAUX_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportAUX = configParams.serialportArray.Find(x => x.portName == configParams.MultipleRead_SerialAUX_comPort);
                    multiplereadpatt = configParams.MultipleRead_pattern;
                }
            }
            if (configParams.TestPhaseAnalysis)
            {
                if (configParams.PhaseAnalysis_SerialMain_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportMain = configParams.serialportArray.Find(x => x.portName == configParams.PhaseAnalysis_SerialMain_comPort);
                    phaseanalysispatt = configParams.PhaseAnalysis_pattern;
                }
                if (configParams.PhaseAnalysis_SerialAUX_comPort != "Not used")
                {
                    if (configParams.serialportArray != null)
                        serialportAUX = configParams.serialportArray.Find(x => x.portName == configParams.PhaseAnalysis_SerialAUX_comPort);
                    phaseanalysispatt = configParams.PhaseAnalysis_pattern;
                }
            }

            if (serialportMain != null)
            {
                serialPortMain = new SerialPortListener(serialportMain.portName, serialportMain.baud, serialportMain.parity, serialportMain.databits, serialportMain.stopbits, serialportMain.hanshake, logger);
                serialPortMain.SetPatterns(goodreadpatt, noreadpatt, partialreadpatt, multiplereadpatt, phaseanalysispatt);
                serialPortMain.GoodReadPatternFound += new EventHandler(OnSerialMainGoodReadPatternFound);
                serialPortMain.NoReadPatternFound += new EventHandler(OnSerialMainNoReadPatternFound);
                serialPortMain.PartialReadPatternFound += new EventHandler(OnSerialMainPartialReadPatternFound);
                serialPortMain.MultipleReadPatternFound += new EventHandler(OnSerialMainMultipleReadPatternFound);
                serialPortMain.OutOfOrderFound += new EventHandler(OnSerialMainOutOfOrderPhaseFound);
                serialPortMain.MissingFound += new EventHandler(OnSerialMainMissingPhaseFound);
                serialPortMainThread = new Thread(serialPortMain.ManageSerialPort);
                serialPortMainThread.Name = "SerialPortMainThread";
            }

            if (serialportAUX != null)
            {
                serialPortAUX = new SerialPortListener(serialportAUX.portName, serialportAUX.baud, serialportAUX.parity, serialportAUX.databits, serialportAUX.stopbits, serialportAUX.hanshake, logger);
                serialPortAUX.SetPatterns(goodreadpatt, noreadpatt, partialreadpatt, multiplereadpatt, phaseanalysispatt);
                serialPortAUX.GoodReadPatternFound += new EventHandler(OnSerialAuxGoodReadPatternFound);
                serialPortAUX.NoReadPatternFound += new EventHandler(OnSerialAuxNoReadPatternFound);
                serialPortAUX.PartialReadPatternFound += new EventHandler(OnSerialAuxPartialReadPatternFound);
                serialPortAUX.MultipleReadPatternFound += new EventHandler(OnSerialAuxMultipleReadPatternFound);
                serialPortMain.OutOfOrderFound += new EventHandler(OnSerialAuxOutOfOrderPhaseFound);
                serialPortMain.MissingFound += new EventHandler(OnSerialAuxMissingPhaseFound);
                serialPortAUXThread = new Thread(serialPortAUX.ManageSerialPort);
                serialPortAUXThread.Name = "SerialPortAUXThread";
            }

            if (configParams.TestGoodRead)
            {
                if (configParams.GoodRead_TCPClient_hostPort != 0)
                {
                    int idx = GetTCPClient(configParams.GoodRead_TCPClient_hostPort);
                    if (idx != -1)
                    {
                        tcpClientArray[idx].GoodreadPattern = configParams.GoodRead_pattern;
                        tcpClientArray[idx].GoodReadPatternFound += new EventHandler(OnTCPClientGoodReadPatternFound);
                    }
                }
                if (configParams.GoodRead_TCPServer_hostPort != 0)
                {
                    int idx = GetTCPServer(configParams.GoodRead_TCPServer_hostPort);
                    if (idx != -1)
                    {
                        tcpServerArray[idx].GoodreadPattern = configParams.GoodRead_pattern;
                        tcpServerArray[idx].GoodReadPatternFound += new EventHandler(OnTCPServerGoodReadPatternFound);
                    }
                }
                if (configParams.GoodRead_UDPServer_hostPort != 0)
                {
                    int idx = GetUDPServer(configParams.GoodRead_UDPServer_hostPort);
                    if (idx != -1)
                    {
                        udpServerArray[idx].GoodreadPattern = configParams.GoodRead_pattern;
                        udpServerArray[idx].GoodReadPatternFound += new EventHandler(OnUDPServerGoodReadPatternFound);
                    }
                }
            }
            if (configParams.TestNoRead)
            {
                if (configParams.NoRead_TCPClient_hostPort != 0)
                {
                    int idx = GetTCPClient(configParams.NoRead_TCPClient_hostPort);
                    if (idx != -1)
                    {
                        tcpClientArray[idx].NoreadPattern = configParams.NoRead_pattern;
                        tcpClientArray[idx].NoReadPatternFound += new EventHandler(OnTCPClientNoReadPatternFound);
                    }
                }
                if (configParams.NoRead_TCPServer_hostPort != 0)
                {
                    int idx = GetTCPServer(configParams.NoRead_TCPServer_hostPort);
                    if (idx != -1)
                    {
                        tcpServerArray[idx].NoreadPattern = configParams.NoRead_pattern;
                        tcpServerArray[idx].NoReadPatternFound += new EventHandler(OnTCPServerNoReadPatternFound);
                    }
                }
                if (configParams.NoRead_UDPServer_hostPort != 0)
                {
                    int idx = GetUDPServer(configParams.NoRead_UDPServer_hostPort);
                    if (idx != -1)
                    {
                        udpServerArray[idx].NoreadPattern = configParams.NoRead_pattern;
                        udpServerArray[idx].NoReadPatternFound += new EventHandler(OnUDPServerNoReadPatternFound);
                    }
                }
            }
            if (configParams.TestPartialRead)
            {
                if (configParams.PartialRead_TCPClient_hostPort != 0)
                {
                    int idx = GetTCPClient(configParams.PartialRead_TCPClient_hostPort);
                    if (idx != -1)
                    {
                        tcpClientArray[idx].PartialreadPattern = configParams.PartialRead_pattern;
                        tcpClientArray[idx].PartialReadPatternFound += new EventHandler(OnTCPClientPartialReadPatternFound);
                    }
                }
                if (configParams.PartialRead_TCPServer_hostPort != 0)
                {
                    int idx = GetTCPServer(configParams.PartialRead_TCPServer_hostPort);
                    if (idx != -1)
                    {
                        tcpServerArray[idx].PartialreadPattern = configParams.PartialRead_pattern;
                        tcpServerArray[idx].PartialReadPatternFound += new EventHandler(OnTCPServerPartialReadPatternFound);
                    }
                }
                if (configParams.PartialRead_UDPServer_hostPort != 0)
                {
                    int idx = GetUDPServer(configParams.PartialRead_UDPServer_hostPort);
                    if (idx != -1)
                    {
                        udpServerArray[idx].PartialreadPattern = configParams.PartialRead_pattern;
                        udpServerArray[idx].PartialReadPatternFound += new EventHandler(OnUDPServerPartialReadPatternFound);
                    }
                }
            }
            if (configParams.TestMultipleRead)
            {
                if (configParams.MultipleRead_TCPClient_hostPort != 0)
                {
                    int idx = GetTCPClient(configParams.MultipleRead_TCPClient_hostPort);
                    if (idx != -1)
                    {
                        tcpClientArray[idx].MultiplereadPattern = configParams.MultipleRead_pattern;
                        tcpClientArray[idx].MultipleReadPatternFound += new EventHandler(OnTCPClientMultipleReadPatternFound);
                    }
                }
                if (configParams.MultipleRead_TCPServer_hostPort != 0)
                {
                    int idx = GetTCPServer(configParams.MultipleRead_TCPServer_hostPort);
                    if (idx != -1)
                    {
                        tcpServerArray[idx].MultiplereadPattern = configParams.MultipleRead_pattern;
                        tcpServerArray[idx].MultipleReadPatternFound += new EventHandler(OnTCPServerMultipleReadPatternFound);
                    }
                }
                if (configParams.MultipleRead_UDPServer_hostPort != 0)
                {
                    int idx = GetUDPServer(configParams.MultipleRead_UDPServer_hostPort);
                    if (idx != -1)
                    {
                        udpServerArray[idx].MultiplereadPattern = configParams.MultipleRead_pattern;
                        udpServerArray[idx].MultipleReadPatternFound += new EventHandler(OnUDPServerMultipleReadPatternFound);
                    }
                }

            }

            if (configParams.TestHeartBeat)
            {
                if (configParams.HeartBeat_TCPClient_hostPort != 0)
                {
                    int idx = GetTCPClient(configParams.HeartBeat_TCPClient_hostPort);
                    if (idx != -1)
                    {
                        tcpClientArray[idx].HeartBeatPattern = configParams.HeartBeat_pattern;
                        tcpClientArray[idx].HeartBeatPatternFound += new EventHandler(OnTCPClientHeartBeatPatternFound);
                    }
                }
                if (configParams.HeartBeat_TCPServer_hostPort != 0)
                {
                    int idx = GetTCPServer(configParams.HeartBeat_TCPServer_hostPort);
                    if (idx != -1)
                    {
                        tcpServerArray[idx].HeartBeatPattern = configParams.HeartBeat_pattern;
                        tcpServerArray[idx].HeartBeatPatternFound += new EventHandler(OnTCPServerHeartBeatPatternFound);
                    }
                }
                if (configParams.HeartBeat_UDPServer_hostPort != 0)
                {
                    int idx = GetUDPServer(configParams.HeartBeat_UDPServer_hostPort);
                    if (idx != -1)
                    {
                        udpServerArray[idx].HeartBeatPattern = configParams.HeartBeat_pattern;
                        udpServerArray[idx].HeartBeatPatternFound += new EventHandler(OnUDPServerHeartBeatPatternFound);
                    }
                }

            }
            
            if (configParams.TestPhaseAnalysis)
            {
                if (configParams.PhaseAnalysis_TCPClient_hostPort != 0)
                {
                    int idx = GetTCPClient(configParams.PhaseAnalysis_TCPClient_hostPort);
                    if (idx != -1)
                    {
                        tcpClientArray[idx].GoodreadPattern = configParams.PhaseAnalysis_pattern;
                        tcpClientArray[idx].SearchForPhaseIndex(configParams.PhaseAnalysis_pattern);
                        tcpClientArray[idx].OutOfOrderPhaseFound += OnTCPClientOutOfOrderPhaseFound;
                        tcpClientArray[idx].MissingPhaseFound += OnTCPClientMissingPhaseFound;
                    }
                }
            }

            if (serialPortMainThread != null)
                serialPortMainThread.Start();
            if (serialPortAUXThread != null)
                serialPortAUXThread.Start();

            if (tcpClientPhaseAnalysisThread != null)
                tcpClientPhaseAnalysisThread.Start();

            for (int i = 0; i < tcpServerArray.Length; i++)
            {
                if (tcpServerArray[i] != null)
                    tcpServerArray[i].Open();
            }
            for (int i = 0; i < udpServerArray.Length; i++)
            {
                if (udpServerArray[i] != null)
                    udpServerArray[i].Open();
            }

            return true;
        }


        private void OnTCPClientGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.tcpClient]++;
            totalMessageCount[(int)ChannelType.tcpClient]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPServerGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.tcpServer]++;
            totalMessageCount[(int)ChannelType.tcpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnUDPServerGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.udpServer]++;
            totalMessageCount[(int)ChannelType.udpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.mainPort]++;
            totalMessageCount[(int)ChannelType.mainPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.auxPort]++;
            totalMessageCount[(int)ChannelType.auxPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }


        private void OnTCPClientNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.tcpClient]++;
            totalMessageCount[(int)ChannelType.tcpClient]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPServerNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.tcpServer]++;
            totalMessageCount[(int)ChannelType.tcpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnUDPServerNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.udpServer]++;
            totalMessageCount[(int)ChannelType.udpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.mainPort]++;
            totalMessageCount[(int)ChannelType.mainPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.auxPort]++;
            totalMessageCount[(int)ChannelType.auxPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }


        private void OnTCPClientPartialReadPatternFound(object sender, EventArgs e)
        {
            partialreadCount[(int)ChannelType.tcpClient]++;
            totalMessageCount[(int)ChannelType.tcpClient]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPServerPartialReadPatternFound(object sender, EventArgs e)
        {
            partialreadCount[(int)ChannelType.tcpServer]++;
            totalMessageCount[(int)ChannelType.tcpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnUDPServerPartialReadPatternFound(object sender, EventArgs e)
        {
            partialreadCount[(int)ChannelType.udpServer]++;
            totalMessageCount[(int)ChannelType.udpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainPartialReadPatternFound(object sender, EventArgs e)
        {
            partialreadCount[(int)ChannelType.mainPort]++;
            totalMessageCount[(int)ChannelType.mainPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxPartialReadPatternFound(object sender, EventArgs e)
        {
            partialreadCount[(int)ChannelType.auxPort]++;
            totalMessageCount[(int)ChannelType.auxPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }


        private void OnTCPClientMultipleReadPatternFound(object sender, EventArgs e)
        {
            multiplereadCount[(int)ChannelType.tcpClient]++;
            totalMessageCount[(int)ChannelType.tcpClient]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPServerMultipleReadPatternFound(object sender, EventArgs e)
        {
            multiplereadCount[(int)ChannelType.tcpServer]++;
            totalMessageCount[(int)ChannelType.tcpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnUDPServerMultipleReadPatternFound(object sender, EventArgs e)
        {
            multiplereadCount[(int)ChannelType.udpServer]++;
            totalMessageCount[(int)ChannelType.udpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainMultipleReadPatternFound(object sender, EventArgs e)
        {
            multiplereadCount[(int)ChannelType.mainPort]++;
            totalMessageCount[(int)ChannelType.mainPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxMultipleReadPatternFound(object sender, EventArgs e)
        {
            multiplereadCount[(int)ChannelType.auxPort]++;
            totalMessageCount[(int)ChannelType.auxPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPClientHeartBeatPatternFound(object sender, EventArgs e)
        {
            heartbeatCount[(int)ChannelType.tcpClient]++;
            totalMessageCount[(int)ChannelType.tcpClient]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPServerHeartBeatPatternFound(object sender, EventArgs e)
        {
            heartbeatCount[(int)ChannelType.tcpServer]++;
            totalMessageCount[(int)ChannelType.tcpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnUDPServerHeartBeatPatternFound(object sender, EventArgs e)
        {
            heartbeatCount[(int)ChannelType.udpServer]++;
            totalMessageCount[(int)ChannelType.udpServer]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainHeartBeatPatternFound(object sender, EventArgs e)
        {
            heartbeatCount[(int)ChannelType.mainPort]++;
            totalMessageCount[(int)ChannelType.mainPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxHeartBeatPatternFound(object sender, EventArgs e)
        {
            heartbeatCount[(int)ChannelType.auxPort]++;
            totalMessageCount[(int)ChannelType.auxPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPClientOutOfOrderPhaseFound(object sender, EventArgs e)
        {
            missingPhaseCount[(int)ChannelType.tcpClient]--;
            outOfOrderPhaseCount[(int)ChannelType.tcpClient]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPClientMissingPhaseFound(object sender, EventArgs e)
        {
            missingPhaseCount[(int)ChannelType.tcpClient]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainOutOfOrderPhaseFound(object sender, EventArgs e)
        {
            missingPhaseCount[(int)ChannelType.mainPort]--;
            outOfOrderPhaseCount[(int)ChannelType.mainPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxOutOfOrderPhaseFound(object sender, EventArgs e)
        {
            missingPhaseCount[(int)ChannelType.auxPort]--;
            outOfOrderPhaseCount[(int)ChannelType.auxPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainMissingPhaseFound(object sender, EventArgs e)
        {
            missingPhaseCount[(int)ChannelType.mainPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxMissingPhaseFound(object sender, EventArgs e)
        {
            missingPhaseCount[(int)ChannelType.auxPort]++;
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void CloseTest()
        {
            //close host threads
            for (int i = 0; i < tcpClientArray.Length; i++)
            {
                if (tcpClientArray[i] != null)
                {
                    tcpClientArray[i].Close();
                    tcpClientArray[i].GoodReadPatternFound -= OnTCPClientGoodReadPatternFound;
                    tcpClientArray[i].NoReadPatternFound -= OnTCPClientNoReadPatternFound;
                    tcpClientArray[i].PartialReadPatternFound -= OnTCPClientPartialReadPatternFound;
                    tcpClientArray[i].MultipleReadPatternFound -= OnTCPClientMultipleReadPatternFound;
                    tcpClientArray[i].HeartBeatPatternFound -= OnTCPClientHeartBeatPatternFound;
                    tcpClientArray[i].OutOfOrderPhaseFound -= OnTCPClientOutOfOrderPhaseFound;
                    tcpClientArray[i].MissingPhaseFound -= OnTCPClientMissingPhaseFound;
                    tcpClientArray[i] = null;
                }
            }
            for (int i = 0; i < tcpServerArray.Length; i++)
            {
                if (tcpServerArray[i] != null)
                {
                    tcpServerArray[i].Close();
                    tcpServerArray[i].GoodReadPatternFound -= OnTCPServerGoodReadPatternFound;
                    tcpServerArray[i].NoReadPatternFound -= OnTCPServerNoReadPatternFound;
                    tcpServerArray[i].PartialReadPatternFound -= OnTCPServerPartialReadPatternFound;
                    tcpServerArray[i].MultipleReadPatternFound -= OnTCPServerMultipleReadPatternFound;
                    tcpServerArray[i].HeartBeatPatternFound -= OnTCPServerHeartBeatPatternFound;
                    tcpServerArray[i] = null;
                }
            }
            for (int i = 0; i < udpServerArray.Length; i++)
            {
                if (udpServerArray[i] != null)
                {
                    udpServerArray[i].Close();
                    udpServerArray[i].GoodReadPatternFound -= OnUDPServerGoodReadPatternFound;
                    udpServerArray[i].NoReadPatternFound -= OnUDPServerNoReadPatternFound;
                    udpServerArray[i].PartialReadPatternFound -= OnUDPServerPartialReadPatternFound;
                    udpServerArray[i].MultipleReadPatternFound -= OnUDPServerMultipleReadPatternFound;
                    udpServerArray[i].HeartBeatPatternFound -= OnUDPServerHeartBeatPatternFound;
                    udpServerArray[i] = null;
                }
            }

            System.Threading.Thread.Sleep(500);

            for (int i = 0; i < tcpClientArray.Length; i++)
            {
                if (tcpClientArray[i] != null)
                {
                    tcpClientArray[i] = null;
                }
            }
            for (int i = 0; i < tcpServerArray.Length; i++)
            {
                if (tcpServerArray[i] != null)
                {
                    tcpServerArray[i] = null;
                }
            }
            for (int i = 0; i < udpServerArray.Length; i++)
            {
                if (udpServerArray[i] != null)
                {
                    udpServerArray[i] = null;
                }
            }

            if (serialPortMain != null)
            {
                serialPortMain.Close();
                System.Threading.Thread.Sleep(100);
                serialPortMainThread.Join();
                serialPortMainThread = null;
                serialPortMain.Dispose();
                serialPortMain = null;
            }
            if (serialPortAUX != null)
            {
                serialPortAUX.Close();
                System.Threading.Thread.Sleep(100);
                serialPortAUXThread.Join();
                serialPortAUXThread = null;
                serialPortAUX.Dispose();
                serialPortAUX = null;
            }

            // setup log files
            if (logConfig.EnableLogging)
            {
                if (logger != null)
                {
                    logger.Close();
                    logger.Dispose();
                    logger = null;
                }
            }

            OnEnded(EventArgs.Empty);
        }

    }

}

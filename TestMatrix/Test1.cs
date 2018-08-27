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
using NationalInstruments.DAQmx;

namespace Test1
{
    using TestMatrix;
    using ClientConnection;
    using ServerConnection;
    using WaveformGenerator;

    public enum EncoderGeneration
    {
        BasedOnFrequency,
        BasedOnConveyorSpeed
    }

    public class SerialPortItem
    {
        public string portName;
        public int baud;
        public Parity parity;
        public int databits;
        public StopBits stopbits;
        public Handshake handshake;

        public SerialPortItem()
        {
            fillDafault();
        }

        public SerialPortItem(string portname)
        {
            fillDafault();
            portName = portname;
        }

        private void fillDafault()
        {
            portName = "";
            baud = 115200;
            parity = Parity.None;
            databits = 8;
            stopbits = StopBits.One;
            handshake = Handshake.None;
        }
    }

    public class Test1ConfigurationParameters
    {
        public IPAddress DeviceAddr;
        public IPAddress ServerAddr;

        public List<SerialPortItem> serialportArray = new List<SerialPortItem>();

        public void AddSerialPort(string portName, int baud, Parity parity, int databits, StopBits stopbits, Handshake hanshake)
        {
            SerialPortItem item = new SerialPortItem();
            item.portName = portName;
            item.baud = baud;
            item.parity = parity;
            item.databits = databits;
            item.stopbits = stopbits;
            item.handshake = hanshake;
            if (serialportArray == null)
                serialportArray = new List<SerialPortItem>();
            serialportArray.Add(item);
        }

        public bool TestDigitalLines;
        public String Out1_DigitalLine;
        public String Out2_DigitalLine;
        public String Out3_DigitalLine;
        public String In1_DigitalLine;
        public String In2_DigitalLine;

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

        public bool GeneratePhase;
        public bool GeneratePeriodicPhase;
        public bool GeneratePhaseOnEncoder;
        public Int32 PhaseOnDuration;
        public Int32 PhaseOffDuration;
        public Int32 PhaseOnRandMinDuration;
        public Int32 PhaseOnRandMaxDuration;
        public Int32 PhaseOffRandMinDuration;
        public Int32 PhaseOffRandMaxDuration;
        public bool GeneratePhaseIO;
        public String PhaseDigitalLine;
        public DigitalLineActiveState PhaseOnActiveState;
        public bool GeneratePhaseTCP;
        public Int32 PhaseTCPPort;
        public string PhaseTCPPhaseonMsg;
        public string PhaseTCPPhaseoffMsg;
        public bool GeneratePhaseSerial;
        public string PhaseSerialPort;
        public string PhaseSerialPhaseonMsg;
        public string PhaseSerialPhaseoffMsg;

        public bool GenerateTrigger;
        public Int32 TriggerDuration;
        public Int32 TriggerPeriod;
        public bool GenerateTriggerIO;
        public String TriggerDigitalLine;
        public DigitalLineActiveState TriggerActiveState;
        public bool TriggerOnPhaseOff;
        public bool GenerateTriggerTCP;
        public Int32 TriggerTCPPort;
        public string TriggerTCPTriggerMsg;
        public bool GenerateTriggerSerial;
        public string TriggerSerialPort;
        public string TriggerSerialTriggerMsg;

        public bool GenerateEncoder;
        public String EncoderDigitalLine;
        public EncoderGeneration EncoderGenerated;
        public double EncoderFrequency;
        public Int32 ConveyorSpeed;
        public double EncoderSteps;
        public bool GenerateConveyorStartStop;
        public Int32 ConveyorStartStopTransition;
        public Int32 ConveyorOFFMaxDuration;
        public Int32 ConveyorOFFMinDuration;
        public Int32 ConveyorONMaxDuration;
        public Int32 ConveyorONMinDuration;

        public bool GenerateProtocolIndex;
        public String ProtocolIndexMessage;
        public Int32 ProtocolIndexCounter;
        public bool GenerateFixedProtocolIndex;
        public Int32 ProtocolIndexFixedDelay;
        public Int32 ProtocolIndexRandMinDelay;
        public Int32 ProtocolIndexRandMaxDelay;
        public bool GenerateProtocolIndexTCP;
        public Int32 ProtocolIndexTCPPort;
        public bool GenerateProtocolIndexSerial;
        public string ProtocolIndexSerialPort;

        public bool GeneratePassthrough;
        public string PassthroughHeader, PassthroughTerminator, PassThroughInputChannel;
        public string PassThroughFillingMode, PassthroughLength, PassthroughFillingPattern, PassthroughFieldJustification;
        public string PassThroughOutputChannel;
        public string PassthroughInputMessage, PassthroughOutputMessage;
        public Int32 PassthroughInputCounter;
        public Int32 PassthroughOutputConter;
        public Int32 PassthroughCycle;
        public bool PassthroughRamdonCycle;
        public Int32 PassthroughMinCycle, PassthroughMaxCycle;

        public Int32 PassthroughIndexTCPPort;


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

        public Int32 phaseCount;
        public Int32 triggerCount;
        public Int32 encoderStartStopCount;
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
        public string[] availableChannels;
        public string[] availableDevices;

        public Test1Class(object formmain, Test1ConfigurationParameters config, LoggerConfiguration logcfg)
        {
            formMain = (MainForm)formmain;
            configParams = config;
            logConfig = logcfg;
            chanVal = new bool[16];
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

            availableChannels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine | PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External);
            availableDevices = DaqSystem.Local.Devices; // returns "dev1" e "dev2"

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
//                 if (tcpClientPhaseAnalysis != null)
//                     missing = tcpClientPhaseAnalysis.missingPhaseIdxList;
                return missing;
            }
        }

        public List<Int32> OutOfOrderPhaseIndex
        {
            get
            {
                List<Int32> outoforder = new List<Int32>();
//                 if (tcpClientPhaseAnalysis != null)
//                     outoforder = tcpClientPhaseAnalysis.outoforderPhaseIdxList;
                return outoforder;
            }
        }

        // private members
        private MainForm formMain;

        private SerialPortClient serialPortMain;
        private Thread serialPortMainThread;

        private SerialPortClient serialPortAUX;
        private Thread serialPortAUXThread;

        private TCPClient[] tcpClientArray;
        private TCPServer[] tcpServerArray;
        private UDPServer[] udpServerArray;

        private Task DAQTask;
        private DigitalMultiChannelReader DAQDigitalReaderMultiChannel;
        private bool[] chanVal;

        private Digital_WaveformGenerator waveformPhase = null;
        private Digital_WaveformGenerator waveformTrigger = null;
        private Digital_WaveformGenerator waveformEncoder = null;

        private Test1ConfigurationParameters configParams;
        private LoggerConfiguration logConfig;

        private Logger logger = null;

        private System.Timers.Timer startstopEncoderTimer;
        private bool startstopEncoder_started;

        private System.Timers.Timer startProtocolIndexTimer;
        //private bool startProtocolIndex_started;

        private SerialPortClient serialProtocolIndex;
        private ClientConnection tcpProtocolIndex;
        

        // Volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        private volatile bool _shouldStop;

        public void DoTest()
        {
            _shouldStop = false;

            OnWorkInProgress(EventArgs.Empty);

            bool rc = PrepareTest();

            if (rc)
            {
                StartTest();
                while (!_shouldStop)
                {
                    // do nothing...
                    System.Threading.Thread.Sleep(100);
                }
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
            phaseCount = 0;
            triggerCount = 0;
            encoderStartStopCount = 0;
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
                logger = new Logger(logConfig);
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
                serialPortMain = new SerialPortClient(serialportMain.portName, serialportMain.baud, serialportMain.parity, serialportMain.databits, serialportMain.stopbits, serialportMain.handshake, logger);
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
                serialPortAUX = new SerialPortClient(serialportAUX.portName, serialportAUX.baud, serialportAUX.parity, serialportAUX.databits, serialportAUX.stopbits, serialportAUX.handshake, logger);
                serialPortAUX.SetPatterns(goodreadpatt, noreadpatt, partialreadpatt, multiplereadpatt, phaseanalysispatt);
                serialPortAUX.GoodReadPatternFound += new EventHandler(OnSerialAuxGoodReadPatternFound);
                serialPortAUX.NoReadPatternFound += new EventHandler(OnSerialAuxNoReadPatternFound);
                serialPortAUX.PartialReadPatternFound += new EventHandler(OnSerialAuxPartialReadPatternFound);
                serialPortAUX.MultipleReadPatternFound += new EventHandler(OnSerialAuxMultipleReadPatternFound);
                serialPortAUX.OutOfOrderFound += new EventHandler(OnSerialAuxOutOfOrderPhaseFound);
                serialPortAUX.MissingFound += new EventHandler(OnSerialAuxMissingPhaseFound);
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

            if (configParams.TestDigitalLines)
            {
                try
                {
                    // NationalInstruments DAQmx management
                    DAQTask = new Task();
                    // Create channels
                    DAQTask.DIChannels.CreateChannel(configParams.Out1_DigitalLine, "", ChannelLineGrouping.OneChannelForEachLine);
                    DAQTask.DIChannels.CreateChannel(configParams.Out2_DigitalLine, "", ChannelLineGrouping.OneChannelForEachLine);
                    DAQTask.DIChannels.CreateChannel(configParams.Out3_DigitalLine, "", ChannelLineGrouping.OneChannelForEachLine);
                    DAQTask.DIChannels.CreateChannel(configParams.In1_DigitalLine, "", ChannelLineGrouping.OneChannelForEachLine);
                    DAQTask.DIChannels.CreateChannel(configParams.In2_DigitalLine, "", ChannelLineGrouping.OneChannelForEachLine);

                    // Configure digital change detection timing
                    //DAQTask.Timing.ConfigureChangeDetection("Dev2/port0/line0:7", "Dev2/port0/line0:7", SampleQuantityMode.ContinuousSamples, 1000);
//                     DAQTask.Timing.ConfigureChangeDetection(configParams.Out1_DigitalLine, configParams.Out1_DigitalLine, SampleQuantityMode.ContinuousSamples, 1000);
//                     DAQTask.Timing.ConfigureChangeDetection(configParams.Out2_DigitalLine, configParams.Out2_DigitalLine, SampleQuantityMode.ContinuousSamples, 1000);
//                     DAQTask.Timing.ConfigureChangeDetection(configParams.Out3_DigitalLine, configParams.Out3_DigitalLine, SampleQuantityMode.ContinuousSamples, 1000);
//                     DAQTask.Timing.ConfigureChangeDetection(configParams.In1_DigitalLine, configParams.In1_DigitalLine, SampleQuantityMode.ContinuousSamples, 1000);
//                     DAQTask.Timing.ConfigureChangeDetection(configParams.In2_DigitalLine, configParams.In2_DigitalLine, SampleQuantityMode.ContinuousSamples, 1000);

                    // Add the digital change detection event handler
                    // Use SynchronizeCallbacks to specify that the object marshals callbacks across threads appropriately.
                    DAQTask.SynchronizeCallbacks = true;

                    //DAQTask.DigitalChangeDetection += new DigitalChangeDetectionEventHandler(DAQTask_DigitalChangeDetection);

                    // Create the reader
                    DAQDigitalReaderMultiChannel = new DigitalMultiChannelReader(DAQTask.Stream);
                }
                catch (DaqException ex)
                {
                    CloseTest();

                    MessageBox.Show(ex.Message);
                    return false;
                }
                catch (SystemException ex)
                {
                    CloseTest();

                    MessageBox.Show(ex.Message);
                    return false;
                }
            }

            if (configParams.GenerateEncoder)
            {
                waveformEncoder = new Digital_WaveformGenerator(WaveformType.DigitalIO, configParams.DeviceAddr, configParams.EncoderDigitalLine, true);
                if (configParams.EncoderGenerated == EncoderGeneration.BasedOnFrequency)
                {
                    waveformEncoder.Frequency = configParams.EncoderFrequency;
                    waveformEncoder.DutyCycle = 50;
                }
                if (configParams.EncoderGenerated == EncoderGeneration.BasedOnConveyorSpeed)
                {
                    waveformEncoder.Frequency = (double)configParams.ConveyorSpeed / configParams.EncoderSteps;
                    waveformEncoder.DutyCycle = 50;
                }

                if (configParams.GenerateConveyorStartStop)
                {
                    waveformEncoder.SmoothStartStop = (configParams.ConveyorStartStopTransition != 0);
                    waveformEncoder.StartStopTransitionTime = configParams.ConveyorStartStopTransition;
                }
                waveformEncoder.WaveformStateChange += new Digital_WaveformGenerator.WaveformStateEventHandler(waveformEncoder_WaveformOnOff);

            }

            if (configParams.GeneratePhaseIO)
            {
                waveformPhase = new Digital_WaveformGenerator(WaveformType.DigitalIO, configParams.DeviceAddr, configParams.PhaseDigitalLine, configParams.GeneratePeriodicPhase)
                {
                    OnDuration = 1000 * configParams.PhaseOnDuration,
                    OffDuration = 1000 * configParams.PhaseOffDuration,
                    OnMinDuration = 1000 * configParams.PhaseOnRandMinDuration,
                    OnMaxDuration = 1000 * configParams.PhaseOnRandMaxDuration,
                    OffMinDuration = 1000 * configParams.PhaseOffRandMinDuration,
                    OffMaxDuration = 1000 * configParams.PhaseOffRandMaxDuration,
                    ActiveState = configParams.PhaseOnActiveState
                };
                waveformPhase.WaveformTransition += new Digital_WaveformGenerator.WaveformTransitionEventHandler(waveformPhase_WaveformTransition);
            }

            if (configParams.GeneratePhaseTCP)
            {
                waveformPhase = new Digital_WaveformGenerator(WaveformType.TCP, configParams.DeviceAddr, Convert.ToString(configParams.PhaseTCPPort), configParams.GeneratePeriodicPhase)
                {
                    OnDuration = 1000 * configParams.PhaseOnDuration,
                    OffDuration = 1000 * configParams.PhaseOffDuration,
                    OnMinDuration = 1000 * configParams.PhaseOnRandMinDuration,
                    OnMaxDuration = 1000 * configParams.PhaseOnRandMaxDuration,
                    OffMinDuration = 1000 * configParams.PhaseOffRandMinDuration,
                    OffMaxDuration = 1000 * configParams.PhaseOffRandMaxDuration,
                    ActiveState = configParams.PhaseOnActiveState,
                    SignalOnMsg = configParams.PhaseTCPPhaseonMsg,
                    SignalOffMsg = configParams.PhaseTCPPhaseoffMsg
                };
                waveformPhase.WaveformTransition += new Digital_WaveformGenerator.WaveformTransitionEventHandler(waveformPhase_WaveformTransition);
            }

            if (configParams.GeneratePhaseSerial)
            {
                waveformPhase = new Digital_WaveformGenerator(WaveformType.Serial, configParams.DeviceAddr, configParams.PhaseSerialPort, configParams.GeneratePeriodicPhase)
                {
                    OnDuration = 1000 * configParams.PhaseOnDuration,
                    OffDuration = 1000 * configParams.PhaseOffDuration,
                    OnMinDuration = 1000 * configParams.PhaseOnRandMinDuration,
                    OnMaxDuration = 1000 * configParams.PhaseOnRandMaxDuration,
                    OffMinDuration = 1000 * configParams.PhaseOffRandMinDuration,
                    OffMaxDuration = 1000 * configParams.PhaseOffRandMaxDuration,
                    ActiveState = configParams.PhaseOnActiveState
                };
                waveformPhase.WaveformTransition += new Digital_WaveformGenerator.WaveformTransitionEventHandler(waveformPhase_WaveformTransition);
            }

            if (configParams.GenerateTriggerIO)
            {
                waveformTrigger = new Digital_WaveformGenerator(WaveformType.DigitalIO, configParams.DeviceAddr, configParams.TriggerDigitalLine, true)
                {
                    OnDuration = 1000 * configParams.TriggerDuration,
                    OffDuration = 1000 * (configParams.TriggerPeriod - configParams.TriggerDuration),
                    ActiveState = configParams.TriggerActiveState
                };
                waveformTrigger.WaveformTransition += new Digital_WaveformGenerator.WaveformTransitionEventHandler(waveformTrigger_WaveformTransition);
            }

            if (configParams.GenerateTriggerTCP)
            {
                waveformTrigger = new Digital_WaveformGenerator(WaveformType.TCP, configParams.DeviceAddr, Convert.ToString(configParams.TriggerTCPPort), true)
                {
                    OnDuration = 1000 * configParams.TriggerDuration,
                    OffDuration = 1000 * (configParams.TriggerPeriod - configParams.TriggerDuration),
                    ActiveState = configParams.TriggerActiveState,
                    SignalOnMsg = configParams.TriggerTCPTriggerMsg,
                    SignalOffMsg = ""
                };
                waveformTrigger.WaveformTransition += new Digital_WaveformGenerator.WaveformTransitionEventHandler(waveformTrigger_WaveformTransition);
            }

            if (configParams.GenerateTriggerSerial)
            {
                waveformTrigger = new Digital_WaveformGenerator(WaveformType.Serial, configParams.DeviceAddr, configParams.TriggerSerialPort, true)
                {
                    OnDuration = 1000 * configParams.TriggerDuration,
                    OffDuration = 1000 * (configParams.TriggerPeriod - configParams.TriggerDuration),
                    ActiveState = configParams.TriggerActiveState
                };
                waveformTrigger.WaveformTransition += new Digital_WaveformGenerator.WaveformTransitionEventHandler(waveformTrigger_WaveformTransition);
            }

            if (configParams.GenerateProtocolIndex)
            {
                startProtocolIndexTimer = new System.Timers.Timer()
                {
                    Enabled = false,
                    AutoReset = false
                };
                startProtocolIndexTimer.Elapsed += OnProtocolIndexTimedEvent;

                if (configParams.GenerateProtocolIndexTCP)
                {
                    tcpProtocolIndex = new ClientConnection(configParams.DeviceAddr.ToString(), configParams.ProtocolIndexTCPPort);
                    tcpProtocolIndex.Open();
                }
                if (configParams.GenerateProtocolIndexSerial)
                {
                    SerialPortItem serialport = null;
                    if (configParams.serialportArray != null)
                        serialport = configParams.serialportArray.Find(x => x.portName == configParams.ProtocolIndexSerialPort);
                    serialProtocolIndex = new SerialPortClient(serialport.portName, serialport.baud, serialport.parity, serialport.databits, serialport.stopbits, serialport.handshake, logger);
                    serialProtocolIndex.Open();
                }

            }
            return true;
        }

        void waveformEncoder_WaveformOnOff(object sender, Digital_WaveformGenerator.WaveformEventArgs a)
        {
            if (sender == waveformEncoder)
            {
                if (a.State == WaveformState.Running)
                {
                    encoderStartStopCount = waveformEncoder.StartCounter;
                    if (configParams.GeneratePhaseOnEncoder)
                    {
                        if (waveformPhase != null)
                            waveformPhase.StartWaveform();
                    }
                    if (CountersIncremented != null)
                        CountersIncremented(this, EventArgs.Empty);
                }
                if (a.State == WaveformState.Stopped)
                {
                    if (configParams.GeneratePhaseOnEncoder)
                    {
                        if (waveformPhase != null)
                            waveformPhase.StopWaveform(true);
                    }
                }
            }
        }

        void waveformPhase_WaveformTransition(object sender, Digital_WaveformGenerator.WaveformEventArgs a)
        {
            if (a.Transition == DigitalLineTransition.ToHigh)
            {
                logger?.Log("PhaseOn");

                // ProtocolIndex to manage?
                if (configParams.GenerateProtocolIndex)
                {
                    // start the ProtocolIndex timer
                    if (configParams.ProtocolIndexFixedDelay <= 0)
                    {
                        // send now
                        ProtocolIndexSend();
                    }
                    else
                    {
                        if (configParams.GenerateFixedProtocolIndex)
                        {
                            startProtocolIndexTimer.Interval = configParams.ProtocolIndexFixedDelay;
                        }
                        else
                        {
                            Random rnd = new Random();
                            startProtocolIndexTimer.Interval = rnd.Next(configParams.ProtocolIndexRandMinDelay, configParams.ProtocolIndexRandMaxDelay);
                        }
                        startProtocolIndexTimer.Enabled = true;
                    }
                }
                else
                {
                    startProtocolIndexTimer = null;
                }

                phaseCount = waveformPhase.ActiveTransitionCounter;
                CountersIncremented?.Invoke(this, EventArgs.Empty);
            }
            if (configParams.TriggerOnPhaseOff == false)
            {
                if (a.Transition == DigitalLineTransition.ToHigh)
                {
                    if (waveformTrigger != null)
                        waveformTrigger.StartWaveform();
                }
                if (a.Transition == DigitalLineTransition.ToLow)
                {
                    if (waveformTrigger != null)
                        waveformTrigger.StopWaveform();
                }
            }
            if (a.Transition == DigitalLineTransition.ToLow)
            {
                logger?.Log("PhaseOff");
            }
        }

        void waveformTrigger_WaveformTransition(object sender, Digital_WaveformGenerator.WaveformEventArgs a)
        {
            if (a.Transition == DigitalLineTransition.ToHigh)
            {
                triggerCount = waveformTrigger.ActiveTransitionCounter;
                if (CountersIncremented != null)
                    CountersIncremented(this, EventArgs.Empty);
            }
            if (a.Transition == DigitalLineTransition.ToLow)
            {
            }
        }

        private bool StartTest()
        {
            if (serialPortMainThread != null)
                serialPortMainThread.Start();
            if (serialPortAUXThread != null)
                serialPortAUXThread.Start();

            for (int i = 0; i < tcpClientArray.Length; i++)
            {
                if (tcpClientArray[i] != null)
                    tcpClientArray[i].Open();
            }
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

            if (configParams.TestDigitalLines)
            {
                try
                {
                    if (DAQTask != null)
                        DAQTask.Start();
                }
                catch (DaqException ex)
                {
                    CloseTest();

                    MessageBox.Show(ex.Message);
                    return false;
                }
            }

            if (configParams.GenerateTrigger && configParams.TriggerOnPhaseOff)
            {
                if (waveformTrigger != null)
                {
                    waveformTrigger.StartWaveform();
                }
            }
            if (configParams.GeneratePhaseOnEncoder == false)
            {
                if (waveformPhase != null)
                {
                    waveformPhase.StartWaveform();
                }
            }
            if (waveformEncoder != null)
            {
                waveformEncoder.StartWaveform();
            }

            if (configParams.GenerateConveyorStartStop)
            {
                // start the start/stop timer
                startstopEncoderTimer = new System.Timers.Timer();
                Random rnd = new Random();
                startstopEncoderTimer.Interval = 1000 * rnd.Next(configParams.ConveyorONMinDuration, configParams.ConveyorONMaxDuration);
                startstopEncoderTimer.Elapsed += OnStartstopEncoderTimedEvent;
                startstopEncoderTimer.AutoReset = false;
                startstopEncoderTimer.Enabled = true;
                startstopEncoder_started = true;
            }
            else
            {
                startstopEncoderTimer = null;
            }
   
            return true;
        }

        private void OnStartstopEncoderTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (waveformEncoder != null)
            {
                Random rnd = new Random();
                if (startstopEncoder_started)
                {
                    startstopEncoderTimer.Interval = 1000 * rnd.Next(configParams.ConveyorOFFMinDuration, configParams.ConveyorOFFMaxDuration);
                    if (waveformEncoder != null)
                    {
                        waveformEncoder.StopWaveform();
                        startstopEncoder_started = false;
                    }
                }
                else
                {
                    startstopEncoderTimer.Interval = 1000 * rnd.Next(configParams.ConveyorONMinDuration, configParams.ConveyorONMaxDuration);
                    if (waveformEncoder != null)
                    {
                        waveformEncoder.StartWaveform();
                        startstopEncoder_started = true;
                    }
                }
                startstopEncoderTimer.AutoReset = false;
                startstopEncoderTimer.Enabled = true;
            }
        }

        private void OnProtocolIndexTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            ProtocolIndexSend();
        }

        private void ProtocolIndexSend()
        {
            logger?.Log("ProtocolIndex");
            startProtocolIndexTimer.Enabled = false;
            string message = configParams.ProtocolIndexMessage;
            bool b = configParams.ProtocolIndexMessage.Contains("%c");
            if (b)
            {
                int index = configParams.ProtocolIndexMessage.IndexOf("%c");
                message = configParams.ProtocolIndexMessage.Substring(0, index);
                message += configParams.ProtocolIndexCounter.ToString();
                message += configParams.ProtocolIndexMessage.Substring(index + 2);
                configParams.ProtocolIndexCounter++;
            }
            if (tcpProtocolIndex != null)
            {
                tcpProtocolIndex.Send(message);
            }

            if (serialProtocolIndex != null)
            {
                serialProtocolIndex.Send(message);
            }
            logger?.Log("ProtocolIndex sent!");
        }

        private void DAQTask_DigitalChangeDetection(object sender, DigitalChangeDetectionEventArgs e)
        {
            bool signalIncrement = false;
            try
            {
                bool[,] data = DAQDigitalReaderMultiChannel.ReadSingleSampleMultiLine();
                if (data[0,0] != chanVal[0])
                {
                    chanVal[0] = data[0,0];
                    if (chanVal[0])
                    {
                        signalIncrement = true;
                        out1Count++;
                    }
                }
                if (data[1,0] != chanVal[1])
                {
                    chanVal[1] = data[1,0];
                    if (chanVal[1])
                    {
                        signalIncrement = true;
                        out2Count++;
                    }
                }
                if (data[2, 0] != chanVal[2])
                {
                    chanVal[2] = data[2, 0];
                    if (chanVal[2])
                    {
                        signalIncrement = true;
                        out3Count++;
                    }
                }
                if (data[3, 0] != chanVal[3])
                {
                    chanVal[3] = data[3, 0];
                    if (chanVal[3])
                    {
                        signalIncrement = true;
                        in1Count++;
                    }
                }
                if (data[4, 0] != chanVal[4])
                {
                    chanVal[4] = data[4,0];
                    if (chanVal[4])
                    {
                        signalIncrement = true;
                        in2Count++;
                    }
                }
            }
            catch (DaqException ex)
            {
                CloseTest();

                MessageBox.Show(ex.Message);
            }
            if (signalIncrement)
            {
                if (CountersIncremented != null)
                    CountersIncremented(this, EventArgs.Empty);
            }

        }


        private void OnTCPClientGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.tcpClient]++;
            totalMessageCount[(int)ChannelType.tcpClient]++;
            Console.Write("goodreadCount[tcpClient]=");
            Console.Write(goodreadCount[(int)ChannelType.tcpClient]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPServerGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.tcpServer]++;
            totalMessageCount[(int)ChannelType.tcpServer]++;
            Console.Write("goodreadCount[tcpServer]=");
            Console.Write(goodreadCount[(int)ChannelType.tcpServer]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnUDPServerGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.udpServer]++;
            totalMessageCount[(int)ChannelType.udpServer]++;
            Console.Write("goodreadCount[udpServer]=");
            Console.Write(goodreadCount[(int)ChannelType.udpServer]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.mainPort]++;
            totalMessageCount[(int)ChannelType.mainPort]++;
            Console.Write("goodreadCount[serMain]=");
            Console.Write(goodreadCount[(int)ChannelType.mainPort]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxGoodReadPatternFound(object sender, EventArgs e)
        {
            goodreadCount[(int)ChannelType.auxPort]++;
            totalMessageCount[(int)ChannelType.auxPort]++;
            Console.Write("goodreadCount[serAUX]=");
            Console.Write(goodreadCount[(int)ChannelType.auxPort]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }


        private void OnTCPClientNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.tcpClient]++;
            totalMessageCount[(int)ChannelType.tcpClient]++;
            Console.Write("noreadCount[tcpClient]=");
            Console.Write(noreadCount[(int)ChannelType.tcpClient]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnTCPServerNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.tcpServer]++;
            totalMessageCount[(int)ChannelType.tcpServer]++;
            Console.Write("noreadCount[tcpServer]=");
            Console.Write(noreadCount[(int)ChannelType.tcpServer]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnUDPServerNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.udpServer]++;
            totalMessageCount[(int)ChannelType.udpServer]++;
            Console.Write("noreadCount[udpServer]=");
            Console.Write(noreadCount[(int)ChannelType.udpServer]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialMainNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.mainPort]++;
            totalMessageCount[(int)ChannelType.mainPort]++;
            Console.Write("noreadCount[serMain]=");
            Console.Write(noreadCount[(int)ChannelType.mainPort]);
            Console.WriteLine();
            if (CountersIncremented != null)
                CountersIncremented(this, EventArgs.Empty);
        }

        private void OnSerialAuxNoReadPatternFound(object sender, EventArgs e)
        {
            noreadCount[(int)ChannelType.auxPort]++;
            totalMessageCount[(int)ChannelType.auxPort]++;
            Console.Write("noreadCount[serAUX]=");
            Console.Write(noreadCount[(int)ChannelType.auxPort]);
            Console.WriteLine();
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
            if (waveformPhase != null)
            {
                Int32 count = 0;
                waveformPhase.StopWaveform();
                while (waveformPhase.IsRunning && count < 20)
                {
                    count++;
                    System.Threading.Thread.Sleep(100);
                }
                waveformPhase.Dispose();
            }
            if (waveformTrigger != null)
            {
                Int32 count = 0;
                waveformTrigger.StopWaveform();
                while (waveformTrigger.IsRunning && count < 20)
                {
                    count++;
                    System.Threading.Thread.Sleep(100);
                }
                waveformTrigger.Dispose();
            }
            if (waveformEncoder != null)
            {
                Int32 count = 0;
                waveformEncoder.StopWaveform();
                while (waveformEncoder.IsRunning && count < 20)
                {
                    count++;
                    System.Threading.Thread.Sleep(100);
                }
                waveformEncoder.Dispose();
            }

            if (startstopEncoderTimer != null)
            {
                startstopEncoderTimer.Stop();
                startstopEncoderTimer.Dispose();
                startstopEncoderTimer = null;
            }

            if (startProtocolIndexTimer != null)
            {
                startProtocolIndexTimer.Stop();
                startProtocolIndexTimer.Dispose();
                startProtocolIndexTimer = null;
            }

            if (DAQTask != null)
                DAQTask.Dispose();

            // wait for the last messages to be received...
            System.Threading.Thread.Sleep(2000);

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

            if (tcpProtocolIndex != null)
            {
                tcpProtocolIndex.Close();
                tcpProtocolIndex = null;
            }

            if (serialProtocolIndex != null)
            {
                serialProtocolIndex.Close();
                serialProtocolIndex.Dispose();
                serialProtocolIndex = null;
            }

            // setup log files
            if (logConfig.EnableLogging)
            {
                if (logger != null)
                {
                    logger.Close();
                    logger = null;
                }
            }

            OnEnded(EventArgs.Empty);
        }

    }

}

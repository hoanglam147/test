using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Net;
using System.Net.NetworkInformation;
using System.Globalization;
using NationalInstruments.DAQmx;
using System.IO;
namespace TestMatrix
{
    using Test1;
    using ClientConnection;
    public partial class MainForm : Form
    {

        enum ListViewColumn
        {
            channel = 0,
            goodreadCount,
            noreadCount,
            partialreadCount,
            multiplereadCount,
            heartbeatCount,
            totalMessageCount,
            MAX_ListViewColumn
        }

        private bool canUpdateCounters = false;
        private bool test1Running = false;
        private Test1Class test1class;
        private Thread testThread;
        private bool testTCPClient;
        private bool testTCPServer;
        private bool testUDPServer;
        private bool testSerialMain;
        private bool testSerialAux;
        private List<SerialPortItem> serialportArray;
        private LoggerConfiguration logConfig = new LoggerConfiguration();

        public MainForm()
        {
            testTCPClient = false;
            testTCPServer = false;
            testUDPServer = false;
            testSerialMain = false;
            testSerialAux = false;
            serialportArray = new List<SerialPortItem>();
            InitializeComponent();
        }

        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Parity({0}):", defaultPortParity.ToString());
            parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity);
        }

        private void loadSettings()
        {
            ipAddressControl_deviceIPAddress.Text = Properties.Settings.Default.DeviceAddress;

            logConfig.EnableLogging = Properties.Settings.Default.Logger_EnableLogging;
            logConfig.LoggingFolder = Properties.Settings.Default.Logger_LoggingFolder;
            logConfig.AppendLogs = Properties.Settings.Default.Logger_AppendLogs;
            logConfig.SplitLogFile = Properties.Settings.Default.Logger_SplitLogFile;
            logConfig.LogFileMaxSize = Properties.Settings.Default.Logger_SplitMaxSize;
            logConfig.LogFileName = Properties.Settings.Default.Logger_LogFileName;

            checkBox_test1_digitallines.Checked = Properties.Settings.Default.TestDigitalLines;
            textBox_test1_out1line.Text = Properties.Settings.Default.Out1_DigitalLine;
            textBox_test1_out2line.Text = Properties.Settings.Default.Out2_DigitalLine;
            textBox_test1_out3line.Text = Properties.Settings.Default.Out3_DigitalLine;
            textBox_test1_in1line.Text = Properties.Settings.Default.In1_DigitalLine;
            textBox_test1_in2line.Text = Properties.Settings.Default.In2_DigitalLine;

            checkBox_test1_goodread.Checked = Properties.Settings.Default.TestGoodRead;
            textBox_test1_goodreadPattern.Text = Properties.Settings.Default.GoodRead_pattern;
            textBox_test1_goodreadTcpClientPort.Text = Properties.Settings.Default.GoodRead_TCPClient_hostPort;
            textBox_test1_goodreadTcpServerPort.Text = Properties.Settings.Default.GoodRead_TCPServer_hostPort;
            textBox_test1_goodreadUdpServerPort.Text = Properties.Settings.Default.GoodRead_UDPServer_hostPort;
            comboBox_test1_goodreadMainPort.Text = Properties.Settings.Default.GoodRead_SerialMain_comPort;
            comboBox_test1_goodreadAuxPort.Text = Properties.Settings.Default.GoodRead_SerialAUX_comPort;

            checkBox_test1_noread.Checked = Properties.Settings.Default.TestNoRead;
            textBox_test1_noreadPattern.Text = Properties.Settings.Default.NoRead_pattern;
            textBox_test1_noreadTcpClientPort.Text = Properties.Settings.Default.NoRead_TCPClient_hostPort;
            textBox_test1_noreadTcpServerPort.Text = Properties.Settings.Default.NoRead_TCPServer_hostPort;
            textBox_test1_noreadUdpServerPort.Text = Properties.Settings.Default.NoRead_UDPServer_hostPort;
            comboBox_test1_noreadMainPort.Text = Properties.Settings.Default.NoRead_SerialMain_comPort;
            comboBox_test1_noreadAuxPort.Text = Properties.Settings.Default.NoRead_SerialAUX_comPort;

            checkBox_test1_partialread.Checked = Properties.Settings.Default.TestPartialRead;
            textBox_test1_partialreadPattern.Text = Properties.Settings.Default.PartialRead_pattern;
            textBox_test1_partialreadTcpClientPort.Text = Properties.Settings.Default.PartialRead_TCPClient_hostPort;
            textBox_test1_partialreadTcpServerPort.Text = Properties.Settings.Default.PartialRead_TCPServer_hostPort;
            textBox_test1_partialreadUdpServerPort.Text = Properties.Settings.Default.PartialRead_UDPServer_hostPort;
            comboBox_test1_partialreadMainPort.Text = Properties.Settings.Default.PartialRead_SerialMain_comPort;
            comboBox_test1_partialreadAuxPort.Text = Properties.Settings.Default.PartialRead_SerialAUX_comPort;

            checkBox_test1_multipleread.Checked = Properties.Settings.Default.TestMultipleRead;
            textBox_test1_multiplereadPattern.Text = Properties.Settings.Default.MultipleRead_pattern;
            textBox_test1_multiplereadTcpClientPort.Text = Properties.Settings.Default.MultipleRead_TCPClient_hostPort;
            textBox_test1_multiplereadTcpServerPort.Text = Properties.Settings.Default.MultipleRead_TCPServer_hostPort;
            textBox_test1_multiplereadUdpServerPort.Text = Properties.Settings.Default.MultipleRead_UDPServer_hostPort;
            comboBox_test1_multiplereadMainPort.Text = Properties.Settings.Default.MultipleRead_SerialMain_comPort;
            comboBox_test1_multiplereadAuxPort.Text = Properties.Settings.Default.MultipleRead_SerialAUX_comPort;

            checkBox_test1_heartbeat.Checked = Properties.Settings.Default.TestHeartBeat;
            textBox_test1_heartbeatPattern.Text = Properties.Settings.Default.HeartBeat_pattern;
            textBox_test1_heartbeatTcpClientPort.Text = Properties.Settings.Default.HeartBeat_TCPClient_hostPort;
            textBox_test1_heartbeatTcpServerPort.Text = Properties.Settings.Default.HeartBeat_TCPServer_hostPort;
            textBox_test1_heartbeatUdpServerPort.Text = Properties.Settings.Default.HeartBeat_UDPServer_hostPort;
            comboBox_test1_heartbeatMainPort.Text = Properties.Settings.Default.HeartBeat_SerialMain_comPort;
            comboBox_test1_heartbeatAuxPort.Text = Properties.Settings.Default.HeartBeat_SerialAUX_comPort;

            checkBox_generation_phaseMode.Checked = Properties.Settings.Default.GeneratePhase;
            if (Properties.Settings.Default.GeneratePeriodicPhase)
                radioButton_generation_periodicPhase.Checked = true;
            else
                radioButton_generation_randomPhase.Checked = true;
            checkBox_generation_phaseOnEncoder.Checked = Properties.Settings.Default.GeneratePhaseOnEncoder;
            textBox_generation_phaseOnDuration.Text = Properties.Settings.Default.PhaseOnDuration;
            textBox_generation_phaseOffDuration.Text = Properties.Settings.Default.PhaseOffDuration;
            textBox_generation_phaseOnMinDuration.Text = Properties.Settings.Default.PhaseOnRandMinDuration;
            textBox_generation_phaseOnMaxDuration.Text = Properties.Settings.Default.PhaseOnRandMaxDuration;
            textBox_generation_phaseOffMinDuration.Text = Properties.Settings.Default.PhaseOffRandMinDuration;
            textBox_generation_phaseOffMaxDuration.Text = Properties.Settings.Default.PhaseOffRandMaxDuration;
            checkBox_generation_phaseLineIO.Checked = Properties.Settings.Default.PhaseLineIO;
            comboBox_generation_phaseDevice.SelectedIndex = comboBox_generation_phaseDevice.FindStringExact(Properties.Settings.Default.PhaseDigitalDevice);
            comboBox_generation_phasePort.SelectedIndex = comboBox_generation_phasePort.FindStringExact(Properties.Settings.Default.PhaseDigitalPort);
            comboBox_generation_phaseLine.SelectedIndex = comboBox_generation_phaseLine.FindStringExact(Properties.Settings.Default.PhaseDigitalLine);
            comboBox_generation_phaseInputActive.Text = Properties.Settings.Default.PhaseOnActiveState;
            checkBox_generation_phaseLineTCP.Checked = Properties.Settings.Default.PhaseLineTCP;
            textBox_generation_phaseTcpPort.Text = Properties.Settings.Default.PhaseLineTcpPort;
            textBox_generation_phaseTcpPhaseonMesg.Text = Properties.Settings.Default.PhaseLineTcpPhaseonMsg;
            textBox_generation_phaseTcpPhaseoffMesg.Text = Properties.Settings.Default.PhaseLineTcpPhaseoffMsg;
            checkBox_generation_phaseLineSerial.Checked = Properties.Settings.Default.PhaseLineSerial;
            comboBox_generation_phaseSerialPort.Text = Properties.Settings.Default.PhaseLineSerialPort;
            textBox_generation_phaseSerialPhaseonMesg.Text = Properties.Settings.Default.PhaseLineSerialPhaseonMsg;
            textBox_generation_phaseSerialPhaseoffMesg.Text = Properties.Settings.Default.PhaseLineSerialPhaseoffMsg;

            checkBox_generation_trigger.Checked = Properties.Settings.Default.GenerateTrigger;
            checkBox_generation_triggerLineIO.Checked = Properties.Settings.Default.TriggerLineIO;
            comboBox_generation_triggerDevice.SelectedIndex = comboBox_generation_triggerDevice.FindStringExact(Properties.Settings.Default.TriggerDigitalDevice);
            comboBox_generation_triggerPort.SelectedIndex = comboBox_generation_triggerPort.FindStringExact(Properties.Settings.Default.TriggerDigitalPort);
            comboBox_generation_triggerLine.SelectedIndex = comboBox_generation_triggerLine.FindStringExact(Properties.Settings.Default.TriggerDigitalLine);
            textBox_generation_triggerDuration.Text = Properties.Settings.Default.TriggerDuration;
            textBox_generation_triggerPeriod.Text = Properties.Settings.Default.TriggerPeriod;
            comboBox_generation_triggerInputActive.Text = Properties.Settings.Default.TriggerActiveState;
            checkBox_generation_triggerOnPhaseOff.Checked = Properties.Settings.Default.TriggerOnPhaseOff;
            checkBox_generation_triggerLineTCP.Checked = Properties.Settings.Default.TriggerLineTCP;
            textBox_generation_triggerTcpPort.Text = Properties.Settings.Default.TriggerLineTcpPort;
            textBox_generation_triggerTcpTriggerMesg.Text = Properties.Settings.Default.TriggerLineTcpTriggerMsg;
            checkBox_generation_triggerLineSerial.Checked = Properties.Settings.Default.TriggerLineSerial;
            comboBox_generation_triggerSerialPort.Text = Properties.Settings.Default.TriggerLineSerialPort;
            textBox_generation_triggerSerialTriggerMesg.Text = Properties.Settings.Default.TriggerLineSerialTriggerMsg;

            checkBox_generation_encoder.Checked = Properties.Settings.Default.GenerateEncoder;
            checkBox_generation_encoderLineIO.Checked = Properties.Settings.Default.EncoderLineIO;
            comboBox_generation_encoderDevice.SelectedIndex = comboBox_generation_encoderDevice.FindStringExact(Properties.Settings.Default.EncoderDigitalDevice);
            comboBox_generation_encoderPort.SelectedIndex = comboBox_generation_encoderPort.FindStringExact(Properties.Settings.Default.EncoderDigitalPort);
            comboBox_generation_encoderLine.SelectedIndex = comboBox_generation_encoderLine.FindStringExact(Properties.Settings.Default.EncoderDigitalLine);
            comboBox_generation_encoderBasedOn.Text = Properties.Settings.Default.EncoderGenerated;
            textBox_generation_encoderFrequency.Text = Properties.Settings.Default.EncoderFrequency;
            textBox_generation_encoderSpeed.Text = Properties.Settings.Default.ConveyorSpeed;
            textBox_generation_encoderSteps.Text = Properties.Settings.Default.EncoderSteps;

            checkBox_generation_encoderStartstop.Checked = Properties.Settings.Default.GenerateStartStop;
            textBox_generation_encoderTransitionTime.Text = Properties.Settings.Default.StartStopTransition;
            textBox_generation_conveyorOFF_MaxDuration.Text = Properties.Settings.Default.ConveyorOFFMaxDuration;
            textBox_generation_conveyorOFF_MinDuration.Text = Properties.Settings.Default.ConveyorOFFMinDuration;
            textBox_generation_conveyorON_MaxDuration.Text = Properties.Settings.Default.ConveyorONMaxDuration;
            textBox_generation_conveyorON_MinDuration.Text = Properties.Settings.Default.ConveyorONMinDuration;

            checkBox_generation_protocolindex.Checked = Properties.Settings.Default.GenerateProtocolIndex;
            checkBox_generation_protocolindexLineTCP.Checked = Properties.Settings.Default.ProtocolIndexLineTCP;
            checkBox_generation_protocolindexLineSerial.Checked = Properties.Settings.Default.ProtocolIndexLineSerial;
            textBox_generation_protocolIndexTcpPort.Text = Properties.Settings.Default.ProtocolIndexLineTcpPort;
            comboBox_generation_protocolIndexSerialPort.Text = Properties.Settings.Default.ProtocolIndexLineSerialPort;
            if (Properties.Settings.Default.GenerateFixedTimingProtocolIndex)
                radioButton_generation_fixedProtocolIndex.Checked = true;
            else
                radioButton_generation_randomProtocolIndex.Checked = true;
            textBox_generation_protocolIndexMessage.Text = Properties.Settings.Default.ProtocolIndexMessage;
            textBox_generation_protocolIndexCounter.Text = Properties.Settings.Default.ProtocolIndexCounterStartsFrom;
            textBox_generation_protocolIndexFixedDelay.Text = Properties.Settings.Default.ProtocolIndexFixedDelay;
            textBox_generation_protocolIndexRandomMinDelay.Text = Properties.Settings.Default.ProtocolIndexRandomMinDelay;
            textBox_generation_protocolIndexRandomMaxDelay.Text = Properties.Settings.Default.ProtocolIndexRandomMaxDelay;

            digitallines_setVisualIntegrity();
            goodread_setVisualIntegrity();
            noread_setVisualIntegrity();
            partialread_setVisualIntegrity();
            multipleread_setVisualIntegrity();
            heartbeat_setVisualIntegrity();

            phasegeneration_setVisualIntegrity();
            triggergeneration_setVisualIntegrity();
            encodergeneration_setVisualIntegrity();
            protocolindexgeneration_setVisualIntegrity();

        }

        private void saveSettings()
        {
            Properties.Settings.Default.DeviceAddress = ipAddressControl_deviceIPAddress.Text;

            Properties.Settings.Default.Logger_EnableLogging = logConfig.EnableLogging;
            Properties.Settings.Default.Logger_LoggingFolder = logConfig.LoggingFolder;
            Properties.Settings.Default.Logger_AppendLogs = logConfig.AppendLogs;
            Properties.Settings.Default.Logger_SplitLogFile = logConfig.SplitLogFile;
            Properties.Settings.Default.Logger_SplitMaxSize = logConfig.LogFileMaxSize;
            Properties.Settings.Default.Logger_LogFileName = logConfig.LogFileName;

            Properties.Settings.Default.TestDigitalLines = checkBox_test1_digitallines.Checked;
            Properties.Settings.Default.Out1_DigitalLine = textBox_test1_out1line.Text;
            Properties.Settings.Default.Out2_DigitalLine = textBox_test1_out2line.Text;
            Properties.Settings.Default.Out3_DigitalLine = textBox_test1_out3line.Text;
            Properties.Settings.Default.In1_DigitalLine = textBox_test1_in1line.Text;
            Properties.Settings.Default.In2_DigitalLine = textBox_test1_in2line.Text;

            Properties.Settings.Default.TestGoodRead = checkBox_test1_goodread.Checked;
            Properties.Settings.Default.GoodRead_pattern = textBox_test1_goodreadPattern.Text;
            Properties.Settings.Default.GoodRead_TCPClient_hostPort = textBox_test1_goodreadTcpClientPort.Text;
            Properties.Settings.Default.GoodRead_TCPServer_hostPort = textBox_test1_goodreadTcpServerPort.Text;
            Properties.Settings.Default.GoodRead_UDPServer_hostPort = textBox_test1_goodreadUdpServerPort.Text;
            Properties.Settings.Default.GoodRead_SerialMain_comPort = comboBox_test1_goodreadMainPort.Text;
            Properties.Settings.Default.GoodRead_SerialAUX_comPort = comboBox_test1_goodreadAuxPort.Text;

            Properties.Settings.Default.TestNoRead = checkBox_test1_noread.Checked;
            Properties.Settings.Default.NoRead_pattern = textBox_test1_noreadPattern.Text;
            Properties.Settings.Default.NoRead_TCPClient_hostPort = textBox_test1_noreadTcpClientPort.Text;
            Properties.Settings.Default.NoRead_TCPServer_hostPort = textBox_test1_noreadTcpServerPort.Text;
            Properties.Settings.Default.NoRead_UDPServer_hostPort = textBox_test1_noreadUdpServerPort.Text;
            Properties.Settings.Default.NoRead_SerialMain_comPort = comboBox_test1_noreadMainPort.Text;
            Properties.Settings.Default.NoRead_SerialAUX_comPort = comboBox_test1_noreadAuxPort.Text;

            Properties.Settings.Default.TestPartialRead = checkBox_test1_partialread.Checked;
            Properties.Settings.Default.PartialRead_pattern = textBox_test1_partialreadPattern.Text;
            Properties.Settings.Default.PartialRead_TCPClient_hostPort = textBox_test1_partialreadTcpClientPort.Text;
            Properties.Settings.Default.PartialRead_TCPServer_hostPort = textBox_test1_partialreadTcpServerPort.Text;
            Properties.Settings.Default.PartialRead_UDPServer_hostPort = textBox_test1_partialreadUdpServerPort.Text;
            Properties.Settings.Default.PartialRead_SerialMain_comPort = comboBox_test1_partialreadMainPort.Text;
            Properties.Settings.Default.PartialRead_SerialAUX_comPort = comboBox_test1_partialreadAuxPort.Text;

            Properties.Settings.Default.TestMultipleRead = checkBox_test1_multipleread.Checked;
            Properties.Settings.Default.MultipleRead_pattern = textBox_test1_multiplereadPattern.Text;
            Properties.Settings.Default.MultipleRead_TCPClient_hostPort = textBox_test1_multiplereadTcpClientPort.Text;
            Properties.Settings.Default.MultipleRead_TCPServer_hostPort = textBox_test1_multiplereadTcpServerPort.Text;
            Properties.Settings.Default.MultipleRead_UDPServer_hostPort = textBox_test1_multiplereadUdpServerPort.Text;
            Properties.Settings.Default.MultipleRead_SerialMain_comPort = comboBox_test1_multiplereadMainPort.Text;
            Properties.Settings.Default.MultipleRead_SerialAUX_comPort = comboBox_test1_multiplereadAuxPort.Text;

            Properties.Settings.Default.TestHeartBeat = checkBox_test1_heartbeat.Checked;
            Properties.Settings.Default.HeartBeat_pattern = textBox_test1_heartbeatPattern.Text;
            Properties.Settings.Default.HeartBeat_TCPClient_hostPort = textBox_test1_heartbeatTcpClientPort.Text;
            Properties.Settings.Default.HeartBeat_TCPServer_hostPort = textBox_test1_heartbeatTcpServerPort.Text;
            Properties.Settings.Default.HeartBeat_UDPServer_hostPort = textBox_test1_heartbeatUdpServerPort.Text;
            Properties.Settings.Default.HeartBeat_SerialMain_comPort = comboBox_test1_heartbeatMainPort.Text;
            Properties.Settings.Default.HeartBeat_SerialAUX_comPort = comboBox_test1_heartbeatAuxPort.Text;

            Properties.Settings.Default.GeneratePhase = checkBox_generation_phaseMode.Checked;
            Properties.Settings.Default.GeneratePeriodicPhase = radioButton_generation_periodicPhase.Checked;
            Properties.Settings.Default.GeneratePhaseOnEncoder = checkBox_generation_phaseOnEncoder.Checked;
            Properties.Settings.Default.PhaseOnDuration = textBox_generation_phaseOnDuration.Text;
            Properties.Settings.Default.PhaseOffDuration = textBox_generation_phaseOffDuration.Text;
            Properties.Settings.Default.PhaseOnRandMinDuration = textBox_generation_phaseOnMinDuration.Text;
            Properties.Settings.Default.PhaseOnRandMaxDuration = textBox_generation_phaseOnMaxDuration.Text;
            Properties.Settings.Default.PhaseOffRandMinDuration = textBox_generation_phaseOffMinDuration.Text;
            Properties.Settings.Default.PhaseOffRandMaxDuration = textBox_generation_phaseOffMaxDuration.Text;
            Properties.Settings.Default.PhaseLineIO = checkBox_generation_phaseLineIO.Checked;
            Properties.Settings.Default.PhaseDigitalDevice = comboBox_generation_phaseDevice.Text;
            Properties.Settings.Default.PhaseDigitalPort = comboBox_generation_phasePort.Text;
            Properties.Settings.Default.PhaseDigitalLine = comboBox_generation_phaseLine.Text;
            Properties.Settings.Default.PhaseOnActiveState = comboBox_generation_phaseInputActive.Text;
            Properties.Settings.Default.PhaseLineTCP = checkBox_generation_phaseLineTCP.Checked;
            Properties.Settings.Default.PhaseLineTcpPort = textBox_generation_phaseTcpPort.Text;
            Properties.Settings.Default.PhaseLineTcpPhaseonMsg = textBox_generation_phaseTcpPhaseonMesg.Text;
            Properties.Settings.Default.PhaseLineTcpPhaseoffMsg = textBox_generation_phaseTcpPhaseoffMesg.Text;
            Properties.Settings.Default.PhaseLineSerial = checkBox_generation_phaseLineSerial.Checked;
            Properties.Settings.Default.PhaseLineSerialPort = comboBox_generation_phaseSerialPort.Text;
            Properties.Settings.Default.PhaseLineSerialPhaseonMsg = textBox_generation_phaseSerialPhaseonMesg.Text;
            Properties.Settings.Default.PhaseLineSerialPhaseoffMsg = textBox_generation_phaseSerialPhaseoffMesg.Text;

            Properties.Settings.Default.GenerateTrigger = checkBox_generation_trigger.Checked;
            Properties.Settings.Default.TriggerLineIO = checkBox_generation_triggerLineIO.Checked;
            Properties.Settings.Default.TriggerDigitalDevice = comboBox_generation_triggerDevice.Text;
            Properties.Settings.Default.TriggerDigitalPort = comboBox_generation_triggerPort.Text;
            Properties.Settings.Default.TriggerDigitalLine = comboBox_generation_triggerLine.Text;
            Properties.Settings.Default.TriggerDuration = textBox_generation_triggerDuration.Text;
            Properties.Settings.Default.TriggerPeriod = textBox_generation_triggerPeriod.Text;
            Properties.Settings.Default.TriggerActiveState = comboBox_generation_triggerInputActive.Text;
            Properties.Settings.Default.TriggerOnPhaseOff = checkBox_generation_triggerOnPhaseOff.Checked;
            Properties.Settings.Default.TriggerLineTCP = checkBox_generation_triggerLineTCP.Checked;
            Properties.Settings.Default.TriggerLineTcpPort = textBox_generation_triggerTcpPort.Text;
            Properties.Settings.Default.TriggerLineTcpTriggerMsg = textBox_generation_triggerTcpTriggerMesg.Text;
            Properties.Settings.Default.TriggerLineSerial = checkBox_generation_triggerLineSerial.Checked;
            Properties.Settings.Default.TriggerLineSerialPort = comboBox_generation_triggerSerialPort.Text;
            Properties.Settings.Default.TriggerLineSerialTriggerMsg = textBox_generation_triggerSerialTriggerMesg.Text;

            Properties.Settings.Default.GenerateEncoder = checkBox_generation_encoder.Checked;
            Properties.Settings.Default.EncoderLineIO = checkBox_generation_encoderLineIO.Checked;
            Properties.Settings.Default.EncoderDigitalDevice = comboBox_generation_encoderDevice.Text;
            Properties.Settings.Default.EncoderDigitalPort = comboBox_generation_encoderPort.Text;
            Properties.Settings.Default.EncoderDigitalLine = comboBox_generation_encoderLine.Text;
            Properties.Settings.Default.EncoderGenerated = comboBox_generation_encoderBasedOn.Text;
            Properties.Settings.Default.EncoderFrequency = textBox_generation_encoderFrequency.Text;
            Properties.Settings.Default.ConveyorSpeed = textBox_generation_encoderSpeed.Text;
            Properties.Settings.Default.EncoderSteps = textBox_generation_encoderSteps.Text;

            Properties.Settings.Default.GenerateStartStop = checkBox_generation_encoderStartstop.Checked;
            Properties.Settings.Default.StartStopTransition = textBox_generation_encoderTransitionTime.Text;
            Properties.Settings.Default.ConveyorOFFMaxDuration = textBox_generation_conveyorOFF_MaxDuration.Text;
            Properties.Settings.Default.ConveyorOFFMinDuration = textBox_generation_conveyorOFF_MinDuration.Text;
            Properties.Settings.Default.ConveyorONMaxDuration = textBox_generation_conveyorON_MaxDuration.Text;
            Properties.Settings.Default.ConveyorONMinDuration = textBox_generation_conveyorON_MinDuration.Text;

            Properties.Settings.Default.GenerateProtocolIndex = checkBox_generation_protocolindex.Checked;
            Properties.Settings.Default.ProtocolIndexLineTCP = checkBox_generation_protocolindexLineTCP.Checked;
            Properties.Settings.Default.ProtocolIndexLineSerial = checkBox_generation_protocolindexLineSerial.Checked;

            Properties.Settings.Default.ProtocolIndexLineTcpPort = textBox_generation_protocolIndexTcpPort.Text;
            Properties.Settings.Default.ProtocolIndexLineSerialPort = comboBox_generation_protocolIndexSerialPort.Text;
            Properties.Settings.Default.GenerateFixedTimingProtocolIndex = radioButton_generation_fixedProtocolIndex.Checked;
            Properties.Settings.Default.ProtocolIndexMessage = textBox_generation_protocolIndexMessage.Text;
            Properties.Settings.Default.ProtocolIndexCounterStartsFrom = textBox_generation_protocolIndexCounter.Text;
            Properties.Settings.Default.ProtocolIndexFixedDelay = textBox_generation_protocolIndexFixedDelay.Text;
            Properties.Settings.Default.ProtocolIndexRandomMinDelay = textBox_generation_protocolIndexRandomMinDelay.Text;
            Properties.Settings.Default.ProtocolIndexRandomMaxDelay = textBox_generation_protocolIndexRandomMaxDelay.Text;

            Properties.Settings.Default.Save();
        }

        private void initComponents()
        {

            try
            {
                test1Running = false;
                groupBox_settingTest1.Enabled = !test1Running;
                groupBox_monitorTest1.Enabled = test1Running;
                button_startTest.Enabled = !test1Running;
                button_stopTest.Enabled = test1Running;

                checkBox_test1_digitallines.Checked = false;
                label_test1_out1line.Visible = checkBox_test1_digitallines.Checked;
                textBox_test1_out1line.Visible = checkBox_test1_digitallines.Checked;
                label_test1_out2line.Visible = checkBox_test1_digitallines.Checked;
                textBox_test1_out2line.Visible = checkBox_test1_digitallines.Checked;
                label_test1_out3line.Visible = checkBox_test1_digitallines.Checked;
                textBox_test1_out3line.Visible = checkBox_test1_digitallines.Checked;
                label_test1_in1line.Visible = checkBox_test1_digitallines.Checked;
                textBox_test1_in1line.Visible = checkBox_test1_digitallines.Checked;
                label_test1_in2line.Visible = checkBox_test1_digitallines.Checked;
                textBox_test1_in2line.Visible = checkBox_test1_digitallines.Checked;

                checkBox_test1_goodread.Checked = false;
                label_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
                textBox_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadTcpClientPortComment.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
                textBox_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadTcpServerPortComment.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
                textBox_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadUdpServerPortComment.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadPattern.Visible = checkBox_test1_goodread.Checked;
                textBox_test1_goodreadPattern.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadMainPort.Visible = checkBox_test1_goodread.Checked;
                comboBox_test1_goodreadMainPort.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadAuxPort.Visible = checkBox_test1_goodread.Checked;
                comboBox_test1_goodreadAuxPort.Visible = checkBox_test1_goodread.Checked;
                ipAddressControl_test1_myAddressGoodRead.Visible = checkBox_test1_goodread.Checked;
                ipAddressControl_test1_myAddressGoodRead.Enabled = false;

                checkBox_test1_noread.Checked = false;
                label_test1_noreadTcpClientPort.Visible = checkBox_test1_noread.Checked;
                textBox_test1_noreadTcpClientPort.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadTcpClientPortComment.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
                textBox_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadTcpServerPortComment.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
                textBox_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadUdpServerPortComment.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadPattern.Visible = checkBox_test1_noread.Checked;
                textBox_test1_noreadPattern.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadMainPort.Visible = checkBox_test1_noread.Checked;
                comboBox_test1_noreadMainPort.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadAuxPort.Visible = checkBox_test1_noread.Checked;
                comboBox_test1_noreadAuxPort.Visible = checkBox_test1_noread.Checked;
                ipAddressControl_test1_myAddressNoRead.Visible = checkBox_test1_noread.Checked;
                ipAddressControl_test1_myAddressNoRead.Enabled = false;

                checkBox_test1_partialread.Checked = false;
                label_test1_partialreadTcpClientPort.Visible = checkBox_test1_partialread.Checked;
                textBox_test1_partialreadTcpClientPort.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadTcpClientPortComment.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
                textBox_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadTcpServerPortComment.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
                textBox_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadUdpServerPortComment.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadPattern.Visible = checkBox_test1_partialread.Checked;
                textBox_test1_partialreadPattern.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadMainPort.Visible = checkBox_test1_partialread.Checked;
                comboBox_test1_partialreadMainPort.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadAuxPort.Visible = checkBox_test1_partialread.Checked;
                comboBox_test1_partialreadAuxPort.Visible = checkBox_test1_partialread.Checked;
                ipAddressControl_test1_myAddressPartialRead.Visible = checkBox_test1_partialread.Checked;
                ipAddressControl_test1_myAddressPartialRead.Enabled = false;

                checkBox_test1_multipleread.Checked = false;
                label_test1_multiplereadTcpClientPort.Visible = checkBox_test1_multipleread.Checked;
                textBox_test1_multiplereadTcpClientPort.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadTcpClientPortComment.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
                textBox_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadTcpServerPortComment.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
                textBox_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadUdpServerPortComment.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
                textBox_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
                comboBox_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
                comboBox_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
                ipAddressControl_test1_myAddressMultipleRead.Visible = checkBox_test1_multipleread.Checked;
                ipAddressControl_test1_myAddressMultipleRead.Enabled = false;

                checkBox_test1_heartbeat.Checked = false;
                label_test1_heartbeatTcpClientPort.Visible = checkBox_test1_heartbeat.Checked;
                textBox_test1_heartbeatTcpClientPort.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatTcpClientPortComment.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatTcpServerPort.Visible = checkBox_test1_heartbeat.Checked;
                textBox_test1_heartbeatTcpServerPort.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatTcpServerPortComment.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatUdpServerPort.Visible = checkBox_test1_heartbeat.Checked;
                textBox_test1_heartbeatUdpServerPort.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatUdpServerPortComment.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatPattern.Visible = checkBox_test1_heartbeat.Checked;
                textBox_test1_heartbeatPattern.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatMainPort.Visible = checkBox_test1_heartbeat.Checked;
                comboBox_test1_heartbeatMainPort.Visible = checkBox_test1_heartbeat.Checked;
                label_test1_heartbeatAuxPort.Visible = checkBox_test1_heartbeat.Checked;
                comboBox_test1_heartbeatAuxPort.Visible = checkBox_test1_heartbeat.Checked;
                ipAddressControl_test1_myAddressHeartBeat.Visible = checkBox_test1_heartbeat.Checked;
                ipAddressControl_test1_myAddressHeartBeat.Enabled = false;

                comboBox_test1_goodreadMainPort.Items.Add("Not used");
                comboBox_test1_goodreadAuxPort.Items.Add("Not used");
                comboBox_test1_noreadMainPort.Items.Add("Not used");
                comboBox_test1_noreadAuxPort.Items.Add("Not used");
                comboBox_test1_partialreadMainPort.Items.Add("Not used");
                comboBox_test1_partialreadAuxPort.Items.Add("Not used");
                comboBox_test1_multiplereadMainPort.Items.Add("Not used");
                comboBox_test1_multiplereadAuxPort.Items.Add("Not used");
                comboBox_test1_heartbeatMainPort.Items.Add("Not used");
                comboBox_test1_heartbeatAuxPort.Items.Add("Not used");

                listView_test1_MessagesReceived.View = View.Details;
                listView_test1_MessagesReceived.GridLines = true;
                listView_test1_MessagesReceived.FullRowSelect = false;
                listView_test1_MessagesReceived.Columns.Clear();
                listView_test1_MessagesReceived.Columns.Add("Channel", 72);
                listView_test1_MessagesReceived.Columns.Add("GoodRead", 74);
                listView_test1_MessagesReceived.Columns.Add("NoRead", 74);
                listView_test1_MessagesReceived.Columns.Add("PartialRead", 74);
                listView_test1_MessagesReceived.Columns.Add("MultipleRead", 74);
                listView_test1_MessagesReceived.Columns.Add("HeartBeat", 74);
                listView_test1_MessagesReceived.Columns.Add("Total", 74);

                listView_test1_InOutCounters.View = View.Details;
                listView_test1_InOutCounters.GridLines = true;
                listView_test1_InOutCounters.FullRowSelect = false;
                listView_test1_InOutCounters.Columns.Clear();
                listView_test1_InOutCounters.Columns.Add("I/O", 55);
                listView_test1_InOutCounters.Columns.Add("Count", 68);

                checkBox_generation_phaseMode.Checked = false;

                radioButton_generation_periodicPhase.Checked = true;
                radioButton_generation_periodicPhase.Visible = checkBox_generation_phaseMode.Checked;
                radioButton_generation_randomPhase.Visible = checkBox_generation_phaseMode.Checked;

                checkBox_generation_phaseOnEncoder.Checked = false;
                checkBox_generation_phaseOnEncoder.Visible = checkBox_generation_phaseMode.Checked;

                groupBox_generation_periodicPhase.Enabled = radioButton_generation_periodicPhase.Checked;
                groupBox_generation_periodicPhase.Visible = checkBox_generation_phaseMode.Checked;
                groupBox_generation_randomPhase.Enabled = !radioButton_generation_periodicPhase.Checked;
                groupBox_generation_randomPhase.Visible = checkBox_generation_phaseMode.Checked;

                textBox_generation_phaseOnDuration.Text = Convert.ToString(100);
                textBox_generation_phaseOffDuration.Text = Convert.ToString(100);
                textBox_generation_phaseOnMinDuration.Text = Convert.ToString(10);
                textBox_generation_phaseOnMaxDuration.Text = Convert.ToString(1000);
                textBox_generation_phaseOffMinDuration.Text = Convert.ToString(10);
                textBox_generation_phaseOffMaxDuration.Text = Convert.ToString(1000);

                checkBox_generation_phaseLineIO.Visible = checkBox_generation_phaseMode.Checked;
                groupBox_generation_phaseLineIO.Visible = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineIO.Checked;
                comboBox_generation_phaseInputActive.Items.Clear();
                comboBox_generation_phaseInputActive.Items.Add("Active High");
                comboBox_generation_phaseInputActive.Items.Add("Active Low");
                comboBox_generation_phaseInputActive.SelectedIndex = 0;

                checkBox_generation_phaseLineTCP.Visible = checkBox_generation_phaseMode.Checked;
                groupBox_generation_phaseLineTCP.Visible = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineTCP.Checked;
                checkBox_generation_phaseLineSerial.Visible = checkBox_generation_phaseMode.Checked;
                groupBox_generation_phaseLineSerial.Visible = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineSerial.Checked;
                comboBox_generation_phaseSerialPort.Items.Add("Not used");

                checkBox_generation_trigger.Checked = false;
                label_generation_triggerDuration.Visible = checkBox_generation_trigger.Checked;
                textBox_generation_triggerDuration.Text = Convert.ToString(100);
                textBox_generation_triggerDuration.Visible = checkBox_generation_trigger.Checked;
                label_generation_triggerPeriod.Visible = checkBox_generation_trigger.Checked;
                textBox_generation_triggerPeriod.Text = Convert.ToString(200);
                textBox_generation_triggerPeriod.Visible = checkBox_generation_trigger.Checked;
                checkBox_generation_triggerOnPhaseOff.Visible = (checkBox_generation_phaseMode.Checked && checkBox_generation_trigger.Checked);
                checkBox_generation_triggerLineIO.Visible = checkBox_generation_trigger.Checked;
                groupBox_generation_triggerLineIO.Visible = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineIO.Checked;
                comboBox_generation_triggerInputActive.Items.Clear();
                comboBox_generation_triggerInputActive.Items.Add("Active High");
                comboBox_generation_triggerInputActive.Items.Add("Active Low");
                comboBox_generation_triggerInputActive.SelectedIndex = 0;

                checkBox_generation_triggerLineTCP.Visible = checkBox_generation_trigger.Checked;
                groupBox_generation_triggerLineTCP.Visible = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineTCP.Checked;
                checkBox_generation_triggerLineSerial.Visible = checkBox_generation_trigger.Checked;
                groupBox_generation_triggerLineSerial.Visible = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineSerial.Checked;
                comboBox_generation_triggerSerialPort.Items.Add("Not used");

                checkBox_generation_encoder.Checked = false;
                comboBox_generation_encoderBasedOn.Items.Clear();
                comboBox_generation_encoderBasedOn.Items.Add("Frequency");
                comboBox_generation_encoderBasedOn.Items.Add("Conveyor Speed");
                comboBox_generation_encoderBasedOn.SelectedIndex = 0;
                comboBox_generation_encoderBasedOn.Visible = checkBox_generation_encoder.Checked;
                textBox_generation_encoderSpeed.Text = Convert.ToString(1000);
                textBox_generation_encoderSteps.Text = Convert.ToString(1.27);
                textBox_generation_encoderFrequency.Text = Convert.ToString(100);

                checkBox_generation_encoderStartstop.Checked = false;
                textBox_generation_encoderTransitionTime.Text = Convert.ToString(1000);
                textBox_generation_conveyorOFF_MaxDuration.Text = Convert.ToString(60);
                textBox_generation_conveyorOFF_MinDuration.Text = Convert.ToString(60);
                textBox_generation_conveyorON_MaxDuration.Text = Convert.ToString(60);
                textBox_generation_conveyorON_MinDuration.Text = Convert.ToString(60);
                encodergeneration_setVisualIntegrity();

                fillDigitalLinesData();
                //23.08.2018 pass through groupbox invisible
                groupBoxpassthroughInputData.Visible = checkboxPassThrough.Checked;
                groupboxPassthroughContentField.Visible = checkboxPassThrough.Checked;
                groupBoxOutputChannels.Visible = checkboxPassThrough.Checked;
                groupBoxInputData.Visible = checkboxPassThrough.Checked;
                groupBoxOutputData.Visible = checkboxPassThrough.Checked;
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                txtLengthPassthrough.Visible = false;
                txtFillingPattern.Visible = false;
                txtFieldJustification.Visible = false;
                cbxFillingMode.Text = "Variable Length";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("1 - " + ex.Message);
            }

            try
            {
                // fill serial lines combobox
                string[] ArrayComPortsNames = null;
                try
                {
                    ArrayComPortsNames = SerialPort.GetPortNames();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("GetPortNames - " + ex.Message);
                }

                serialportArray.Clear();
                if (ArrayComPortsNames != null)
                {
                    if (ArrayComPortsNames.GetUpperBound(0) >= 0)
                    {
                        Array.Sort(ArrayComPortsNames);

                        foreach (string portname in ArrayComPortsNames)
                        {
                            if (portname.Substring(0, 3) == "COM")
                            {
                                SerialPortItem port = new SerialPortItem(portname);
                                serialportArray.Add(port);
                            }
                        }
                    }
                }

                foreach (SerialPortItem port in serialportArray)
                {
                    comboBox_test1_goodreadMainPort.Items.Add(port.portName);
                    comboBox_test1_goodreadAuxPort.Items.Add(port.portName);
                    comboBox_test1_noreadMainPort.Items.Add(port.portName);
                    comboBox_test1_noreadAuxPort.Items.Add(port.portName);
                    comboBox_test1_partialreadMainPort.Items.Add(port.portName);
                    comboBox_test1_partialreadAuxPort.Items.Add(port.portName);
                    comboBox_test1_multiplereadMainPort.Items.Add(port.portName);
                    comboBox_test1_multiplereadAuxPort.Items.Add(port.portName);
                    comboBox_test1_heartbeatMainPort.Items.Add(port.portName);
                    comboBox_test1_heartbeatAuxPort.Items.Add(port.portName);

                    comboBox_generation_phaseSerialPort.Items.Add(port.portName);
                    comboBox_generation_triggerSerialPort.Items.Add(port.portName);
                    comboBox_generation_protocolIndexSerialPort.Items.Add(port.portName);
                }
                if (serialportArray.Count > 0)
                {
                    comboBox_test1_goodreadMainPort.SelectedIndex = 0;
                    comboBox_test1_goodreadAuxPort.SelectedIndex = 0;
                    comboBox_test1_noreadMainPort.SelectedIndex = 0;
                    comboBox_test1_noreadAuxPort.SelectedIndex = 0;
                    comboBox_test1_partialreadMainPort.SelectedIndex = 0;
                    comboBox_test1_partialreadAuxPort.SelectedIndex = 0;
                    comboBox_test1_multiplereadMainPort.SelectedIndex = 0;
                    comboBox_test1_multiplereadAuxPort.SelectedIndex = 0;
                    comboBox_test1_heartbeatMainPort.SelectedIndex = 0;
                    comboBox_test1_heartbeatAuxPort.SelectedIndex = 0;

                    comboBox_generation_phaseSerialPort.SelectedIndex = 0;
                    comboBox_generation_triggerSerialPort.SelectedIndex = 0;
                    comboBox_generation_protocolIndexSerialPort.SelectedIndex = 0;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("2 - " + ex.Message);
            }

            try
            {
                IPHostEntry host;
                int count = 0;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily.ToString() == "InterNetwork")
                    {
                        count++;
                        string localIP = ip.ToString();
                        comboBox_netAdapters.Items.Add(localIP);
                    }
                }
                comboBox_netAdapters.SelectedIndex = 0;
                comboBox_netAdapters.Visible = (count > 1);
                label_networkAdapters.Visible = (count > 1);

                ipAddressControl_test1_myAddressGoodRead.Text = comboBox_netAdapters.Text;
                ipAddressControl_test1_myAddressNoRead.Text = comboBox_netAdapters.Text;
                ipAddressControl_test1_myAddressPartialRead.Text = comboBox_netAdapters.Text;
                ipAddressControl_test1_myAddressMultipleRead.Text = comboBox_netAdapters.Text;
                ipAddressControl_test1_myAddressHeartBeat.Text = comboBox_netAdapters.Text;

                ipAddressControl_deviceIPAddress.Select();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("3 - " + ex.Message);
            }
        }

        private void fillDigitalLinesData()
        {
            comboBox_generation_phaseDevice.Items.Clear();
            comboBox_generation_phasePort.Items.Clear();
            comboBox_generation_phaseLine.Items.Clear();
            comboBox_generation_triggerDevice.Items.Clear();
            comboBox_generation_triggerPort.Items.Clear();
            comboBox_generation_triggerLine.Items.Clear();
            comboBox_generation_encoderDevice.Items.Clear();
            comboBox_generation_encoderPort.Items.Clear();
            comboBox_generation_encoderLine.Items.Clear();

            string[] chans = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.All, PhysicalChannelAccess.External);
            if (chans == null || chans.Length == 0)
            {
                return;
            }
            foreach (string chan in chans)
            {
                string[] split = chan.Split(new Char[] { '/' });
                if (split.Length > 0 && split[0].Contains("Dev"))
                {
                    if (comboBox_generation_phaseDevice.Items.Contains(split[0]) == false)
                        comboBox_generation_phaseDevice.Items.Add(split[0]);
                    if (comboBox_generation_triggerDevice.Items.Contains(split[0]) == false)
                        comboBox_generation_triggerDevice.Items.Add(split[0]);
                    if (comboBox_generation_encoderDevice.Items.Contains(split[0]) == false)
                        comboBox_generation_encoderDevice.Items.Add(split[0]);
                }
                if (split.Length > 1 && split[1].Contains("port"))
                {
                    if (comboBox_generation_phasePort.Items.Contains(split[1]) == false)
                        comboBox_generation_phasePort.Items.Add(split[1]);
                    if (comboBox_generation_triggerPort.Items.Contains(split[1]) == false)
                        comboBox_generation_triggerPort.Items.Add(split[1]);
                    if (comboBox_generation_encoderPort.Items.Contains(split[1]) == false)
                        comboBox_generation_encoderPort.Items.Add(split[1]);
                }
                if (split.Length > 2 && split[2].Contains("line"))
                {
                    if (comboBox_generation_phaseLine.Items.Contains(split[2]) == false)
                        comboBox_generation_phaseLine.Items.Add(split[2]);
                    if (comboBox_generation_triggerLine.Items.Contains(split[2]) == false)
                        comboBox_generation_triggerLine.Items.Add(split[2]);
                    if (comboBox_generation_encoderLine.Items.Contains(split[2]) == false)
                        comboBox_generation_encoderLine.Items.Add(split[2]);
                }
                if (comboBox_generation_phaseDevice.Items.Count > 0)
                    comboBox_generation_phaseDevice.SelectedIndex = 0;
                if (comboBox_generation_triggerDevice.Items.Count > 0)
                    comboBox_generation_triggerDevice.SelectedIndex = 0;
                if (comboBox_generation_encoderDevice.Items.Count > 0)
                    comboBox_generation_encoderDevice.SelectedIndex = 0;
                if (comboBox_generation_phasePort.Items.Count > 0)
                    comboBox_generation_phasePort.SelectedIndex = 0;
                if (comboBox_generation_triggerPort.Items.Count > 0)
                    comboBox_generation_triggerPort.SelectedIndex = 0;
                if (comboBox_generation_encoderPort.Items.Count > 0)
                    comboBox_generation_encoderPort.SelectedIndex = 0;
                if (comboBox_generation_phaseLine.Items.Count > 0)
                    comboBox_generation_phaseLine.SelectedIndex = 0;
                if (comboBox_generation_triggerLine.Items.Count > 0)
                    comboBox_generation_triggerLine.SelectedIndex = 0;
                if (comboBox_generation_encoderLine.Items.Count > 0)
                    comboBox_generation_encoderLine.SelectedIndex = 0;

            }
        }

        private void debug_fillData()
        {
            //             ipAddressControl_deviceIPAddress.Text = "192.168.0.102";
            //             comboBox_netAdapters.SelectedIndex = 1;

            //             checkBox_test1_digitallines.Checked = true;

            //             checkBox_test1_goodread.Checked = true;
            //             textBox_test1_goodreadTcpClientPort.Text = "51236";
            //             textBox_test1_goodreadTcpServerPort.Text = "51200";
            //             textBox_test1_goodreadUdpServerPort.Text = "51300";
            //             comboBox_test1_goodreadMainPort.Text = "COM8";
            //             comboBox_test1_goodreadAuxPort.Text = "COM1";

            //             checkBox_test1_noread.Checked = true;
            //             textBox_test1_noreadTcpClientPort.Text = "51236";
            //             textBox_test1_noreadTcpServerPort.Text = "51200";
            //             textBox_test1_noreadUdpServerPort.Text = "51300";
            //             comboBox_test1_noreadMainPort.Text = "COM8";
            //             comboBox_test1_noreadAuxPort.Text = "COM1";

            //             checkBox_test1_partialread.Checked = true;
            //             textBox_test1_partialreadTcpClientPort.Text = "51236";
            //             textBox_test1_partialreadTcpServerPort.Text = "51200";
            //             textBox_test1_partialreadUdpServerPort.Text = "51300";
            //             comboBox_test1_partialreadMainPort.Text = "COM8";
            //             comboBox_test1_partialreadAuxPort.Text = "COM1";

            //             checkBox_test1_multipleread.Checked = true;
            //             textBox_test1_multiplereadTcpClientPort.Text = "51236";
            //             textBox_test1_multiplereadTcpServerPort.Text = "51200";
            //             textBox_test1_multiplereadUdpServerPort.Text = "51300";
            //             comboBox_test1_multiplereadMainPort.Text = "COM8";
            //             comboBox_test1_multiplereadAuxPort.Text = "COM1";

            //             checkBox_test1_heartbeat.Checked = true;
            //             textBox_test1_heartbeatTcpClientPort.Text = "51236";
            //             textBox_test1_heartbeatTcpServerPort.Text = "51200";
            //             textBox_test1_heartbeatUdpServerPort.Text = "51300";
            //             comboBox_test1_heartbeatMainPort.Text = "COM8";
            //             comboBox_test1_heartbeatAuxPort.Text = "COM1";
        }

        private void manageComponentsForStartTest()
        {
            test1Running = true;
            groupBox_settingTest1.Enabled = !test1Running;
            groupBox_monitorTest1.Enabled = test1Running;
            button_startTest.Enabled = !test1Running;
            button_stopTest.Enabled = test1Running;
            button_stopTest.Focus();
        }

        private void manageComponentsForWaitingStopTest()
        {
            Application.UseWaitCursor = true;
            button_startTest.Enabled = false;
            button_stopTest.Enabled = false;
        }

        private void manageComponentsForStopTest()
        {
            test1Running = false;
            Application.UseWaitCursor = false;
            groupBox_settingTest1.Enabled = !test1Running;
            groupBox_monitorTest1.Enabled = test1Running;
            button_startTest.Enabled = !test1Running;
            button_stopTest.Enabled = test1Running;
            button_startTest.Focus();
        }

        private bool isDigitalLinesTest()
        {
            return checkBox_test1_digitallines.Checked;
        }

        private bool isTCPClientTest()
        {
            bool tocheck = false;
            if (checkBox_test1_goodread.Checked && textBox_test1_goodreadTcpClientPort.Text != string.Empty
                || checkBox_test1_noread.Checked && textBox_test1_noreadTcpClientPort.Text != string.Empty
                || checkBox_test1_partialread.Checked && textBox_test1_partialreadTcpClientPort.Text != string.Empty
                || checkBox_test1_multipleread.Checked && textBox_test1_multiplereadTcpClientPort.Text != string.Empty
                || checkBox_test1_heartbeat.Checked && textBox_test1_heartbeatTcpClientPort.Text != string.Empty
                )
            {
                tocheck = true;
            }
            return tocheck;
        }

        private bool isTCPServerTest()
        {
            bool tocheck = false;
            if (checkBox_test1_goodread.Checked && textBox_test1_goodreadTcpServerPort.Text != string.Empty
                || checkBox_test1_noread.Checked && textBox_test1_noreadTcpServerPort.Text != string.Empty
                || checkBox_test1_partialread.Checked && textBox_test1_partialreadTcpServerPort.Text != string.Empty
                || checkBox_test1_multipleread.Checked && textBox_test1_multiplereadTcpServerPort.Text != string.Empty
                || checkBox_test1_heartbeat.Checked && textBox_test1_heartbeatTcpServerPort.Text != string.Empty
                )
            {
                tocheck = true;
            }
            return tocheck;
        }

        private bool isUDPServerTest()
        {
            bool tocheck = false;
            if (checkBox_test1_goodread.Checked && textBox_test1_goodreadUdpServerPort.Text != string.Empty
                || checkBox_test1_noread.Checked && textBox_test1_noreadUdpServerPort.Text != string.Empty
                || checkBox_test1_partialread.Checked && textBox_test1_partialreadUdpServerPort.Text != string.Empty
                || checkBox_test1_multipleread.Checked && textBox_test1_multiplereadUdpServerPort.Text != string.Empty
                || checkBox_test1_heartbeat.Checked && textBox_test1_heartbeatUdpServerPort.Text != string.Empty
                )
            {
                tocheck = true;
            }
            return tocheck;
        }

        private bool isSerialMainTest()
        {
            bool tocheck = false;
            if (checkBox_test1_goodread.Checked && comboBox_test1_goodreadMainPort.SelectedIndex != 0
                || checkBox_test1_noread.Checked && comboBox_test1_noreadMainPort.SelectedIndex != 0
                || checkBox_test1_partialread.Checked && comboBox_test1_partialreadMainPort.SelectedIndex != 0
                || checkBox_test1_multipleread.Checked && comboBox_test1_multiplereadMainPort.SelectedIndex != 0
                || checkBox_test1_heartbeat.Checked && comboBox_test1_heartbeatMainPort.SelectedIndex != 0
                )
            {
                tocheck = true;
            }
            return tocheck;
        }

        private bool isSerialAUXTest()
        {
            bool tocheck = false;
            if (checkBox_test1_goodread.Checked && comboBox_test1_goodreadAuxPort.SelectedIndex != 0
                || checkBox_test1_noread.Checked && comboBox_test1_noreadAuxPort.SelectedIndex != 0
                || checkBox_test1_partialread.Checked && comboBox_test1_partialreadAuxPort.SelectedIndex != 0
                || checkBox_test1_multipleread.Checked && comboBox_test1_multiplereadAuxPort.SelectedIndex != 0
                || checkBox_test1_heartbeat.Checked && comboBox_test1_heartbeatAuxPort.SelectedIndex != 0
                )
            {
                tocheck = true;
            }
            return tocheck;
        }

        private bool CheckGoodReadPortSettings()
        {
            if (checkBox_test1_goodread.Checked)
            {
                if (textBox_test1_goodreadTcpClientPort.Text != "")
                    return true;
                if (textBox_test1_goodreadTcpServerPort.Text != "")
                    return true;
                if (textBox_test1_goodreadUdpServerPort.Text != "")
                    return true;
            }
            return false;
        }

        private bool CheckNoReadPortSettings()
        {
            if (checkBox_test1_noread.Checked)
            {
                if (textBox_test1_noreadTcpClientPort.Text != "")
                    return true;
                if (textBox_test1_noreadTcpServerPort.Text != "")
                    return true;
                if (textBox_test1_noreadUdpServerPort.Text != "")
                    return true;
            }
            return false;
        }

        private bool CheckPartialReadPortSettings()
        {
            if (checkBox_test1_partialread.Checked)
            {
                if (textBox_test1_partialreadTcpClientPort.Text != "")
                    return true;
                if (textBox_test1_partialreadTcpServerPort.Text != "")
                    return true;
                if (textBox_test1_partialreadUdpServerPort.Text != "")
                    return true;
            }
            return false;
        }

        private bool CheckMultipleReadPortSettings()
        {
            if (checkBox_test1_multipleread.Checked)
            {
                if (textBox_test1_multiplereadTcpClientPort.Text != "")
                    return true;
                if (textBox_test1_multiplereadTcpServerPort.Text != "")
                    return true;
                if (textBox_test1_multiplereadUdpServerPort.Text != "")
                    return true;
            }
            return false;
        }

        private bool CheckHeartBeatPortSettings()
        {
            if (checkBox_test1_heartbeat.Checked)
            {
                if (textBox_test1_heartbeatTcpClientPort.Text != "")
                    return true;
                if (textBox_test1_heartbeatTcpServerPort.Text != "")
                    return true;
                if (textBox_test1_heartbeatUdpServerPort.Text != "")
                    return true;
            }
            return false;
        }
        Thread threadPassthrough1, threadPassthrough2;
        private void button_startTest_Click(object sender, EventArgs e)
        {
            bool canRun = true;

            timer_updateCounters.Enabled = true;

            testTCPClient = isTCPClientTest();
            testTCPServer = isTCPServerTest();
            testUDPServer = isUDPServerTest();
            testSerialMain = isSerialMainTest();
            testSerialAux = isSerialAUXTest();

            if (CheckGoodReadPortSettings()
                || CheckNoReadPortSettings()
                || CheckPartialReadPortSettings()
                || CheckMultipleReadPortSettings()
                || CheckHeartBeatPortSettings()
                )
            {
                if (ipAddressControl_deviceIPAddress.AnyBlank)
                {
                    MessageBox.Show("IP Address not valid", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (canRun)
            {
                Test1ConfigurationParameters config = new Test1ConfigurationParameters();
                config.DeviceAddr = ipAddressControl_deviceIPAddress.IPAddress;
                config.ServerAddr = IPAddress.Parse(comboBox_netAdapters.Text);

                config.serialportArray.Clear();
                foreach (SerialPortItem serial in serialportArray)
                {
                    config.serialportArray.Add(serial);
                }

                config.TestDigitalLines = checkBox_test1_digitallines.Checked;
                config.Out1_DigitalLine = textBox_test1_out1line.Text;
                config.Out2_DigitalLine = textBox_test1_out2line.Text;
                config.Out3_DigitalLine = textBox_test1_out3line.Text;
                config.In1_DigitalLine = textBox_test1_in1line.Text;
                config.In2_DigitalLine = textBox_test1_in2line.Text;

                config.GeneratePassthrough = checkboxPassThrough.Checked;
                config.PassthroughHeader = txtheaderpassthrough.Text;
                config.PassthroughTerminator = txtterminatorPassthorugh.Text;
                config.PassThroughInputChannel = cbxInputChannelPassthrough.Text;
                config.PassThroughFillingMode = cbxFillingMode.Text;
                config.PassthroughLength = txtLengthPassthrough.Text;
                config.PassthroughFillingPattern = txtFillingPattern.Text;
                config.PassthroughFieldJustification = txtFieldJustification.Text;
                config.PassthroughInputMessage = txtInputData.Text;
                Int32.TryParse(txtPassthroughCycle.Text, out config.PassthroughCycle);
                config.PassthroughRamdonCycle = checkRandom.Checked;
                Int32.TryParse(txtPassthroughMinCycle.Text, out config.PassthroughMinCycle);
                Int32.TryParse(txtPassthroughMaxCycle.Text, out config.PassthroughMaxCycle);
                config.PassThroughOutputChannel = cbxOutputchannelPassthrough.Text;

                config.TestGoodRead = checkBox_test1_goodread.Checked;
                config.GoodRead_pattern = textBox_test1_goodreadPattern.Text;
                try
                {
                    config.GoodRead_TCPClient_hostPort = ToInt32(textBox_test1_goodreadTcpClientPort.Text);
                }
                catch
                {
                    config.GoodRead_TCPClient_hostPort = 0;
                }
                try
                {
                    config.GoodRead_TCPServer_hostPort = ToInt32(textBox_test1_goodreadTcpServerPort.Text);
                }
                catch
                {
                    config.GoodRead_TCPServer_hostPort = 0;
                }
                try
                {
                    config.GoodRead_UDPServer_hostPort = ToInt32(textBox_test1_goodreadUdpServerPort.Text);
                }
                catch
                {
                    config.GoodRead_UDPServer_hostPort = 0;
                }
                config.GoodRead_SerialMain_comPort = comboBox_test1_goodreadMainPort.Text;
                config.GoodRead_SerialAUX_comPort = comboBox_test1_goodreadAuxPort.Text;

                config.TestNoRead = checkBox_test1_noread.Checked;
                config.NoRead_pattern = textBox_test1_noreadPattern.Text;
                try
                {
                    config.NoRead_TCPClient_hostPort = ToInt32(textBox_test1_noreadTcpClientPort.Text);
                }
                catch
                {
                    config.NoRead_TCPClient_hostPort = 0;
                }
                try
                {
                    config.NoRead_TCPServer_hostPort = ToInt32(textBox_test1_noreadTcpServerPort.Text);
                }
                catch
                {
                    config.NoRead_TCPServer_hostPort = 0;
                }
                try
                {
                    config.NoRead_UDPServer_hostPort = ToInt32(textBox_test1_noreadUdpServerPort.Text);
                }
                catch
                {
                    config.NoRead_UDPServer_hostPort = 0;
                }
                config.NoRead_SerialMain_comPort = comboBox_test1_noreadMainPort.Text;
                config.NoRead_SerialAUX_comPort = comboBox_test1_noreadAuxPort.Text;

                config.TestPartialRead = checkBox_test1_partialread.Checked;
                config.PartialRead_pattern = textBox_test1_partialreadPattern.Text;
                try
                {
                    config.PartialRead_TCPClient_hostPort = ToInt32(textBox_test1_partialreadTcpClientPort.Text);
                }
                catch
                {
                    config.PartialRead_TCPClient_hostPort = 0;
                }
                try
                {
                    config.PartialRead_TCPServer_hostPort = ToInt32(textBox_test1_partialreadTcpServerPort.Text);
                }
                catch
                {
                    config.PartialRead_TCPServer_hostPort = 0;
                }
                try
                {
                    config.PartialRead_UDPServer_hostPort = ToInt32(textBox_test1_partialreadUdpServerPort.Text);
                }
                catch
                {
                    config.PartialRead_UDPServer_hostPort = 0;
                }
                config.PartialRead_SerialMain_comPort = comboBox_test1_partialreadMainPort.Text;
                config.PartialRead_SerialAUX_comPort = comboBox_test1_partialreadAuxPort.Text;

                config.TestMultipleRead = checkBox_test1_multipleread.Checked;
                config.MultipleRead_pattern = textBox_test1_multiplereadPattern.Text;
                try
                {
                    config.MultipleRead_TCPClient_hostPort = ToInt32(textBox_test1_multiplereadTcpClientPort.Text);
                }
                catch
                {
                    config.MultipleRead_TCPClient_hostPort = 0;
                }
                try
                {
                    config.MultipleRead_TCPServer_hostPort = ToInt32(textBox_test1_multiplereadTcpServerPort.Text);
                }
                catch
                {
                    config.MultipleRead_TCPServer_hostPort = 0;
                }
                try
                {
                    config.MultipleRead_UDPServer_hostPort = ToInt32(textBox_test1_multiplereadUdpServerPort.Text);
                }
                catch
                {
                    config.MultipleRead_UDPServer_hostPort = 0;
                }
                config.MultipleRead_SerialMain_comPort = comboBox_test1_multiplereadMainPort.Text;
                config.MultipleRead_SerialAUX_comPort = comboBox_test1_multiplereadAuxPort.Text;

                config.TestHeartBeat = checkBox_test1_heartbeat.Checked;
                config.HeartBeat_pattern = textBox_test1_heartbeatPattern.Text;
                try
                {
                    config.HeartBeat_TCPClient_hostPort = ToInt32(textBox_test1_heartbeatTcpClientPort.Text);
                }
                catch
                {
                    config.HeartBeat_TCPClient_hostPort = 0;
                }
                try
                {
                    config.HeartBeat_TCPServer_hostPort = ToInt32(textBox_test1_heartbeatTcpServerPort.Text);
                }
                catch
                {
                    config.HeartBeat_TCPServer_hostPort = 0;
                }
                try
                {
                    config.HeartBeat_UDPServer_hostPort = ToInt32(textBox_test1_heartbeatUdpServerPort.Text);
                }
                catch
                {
                    config.HeartBeat_UDPServer_hostPort = 0;
                }
                config.HeartBeat_SerialMain_comPort = comboBox_test1_heartbeatMainPort.Text;
                config.HeartBeat_SerialAUX_comPort = comboBox_test1_heartbeatAuxPort.Text;

                config.GeneratePhase = checkBox_generation_phaseMode.Checked;
                config.GeneratePhaseIO = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineIO.Checked;
                config.GeneratePeriodicPhase = radioButton_generation_periodicPhase.Checked;
                config.GeneratePhaseOnEncoder = checkBox_generation_phaseOnEncoder.Checked;
                config.PhaseOnDuration = ToInt32(textBox_generation_phaseOnDuration.Text);
                config.PhaseOffDuration = ToInt32(textBox_generation_phaseOffDuration.Text);
                config.PhaseOnRandMinDuration = ToInt32(textBox_generation_phaseOnMinDuration.Text);
                config.PhaseOnRandMaxDuration = ToInt32(textBox_generation_phaseOnMaxDuration.Text);
                config.PhaseOffRandMinDuration = ToInt32(textBox_generation_phaseOffMinDuration.Text);
                config.PhaseOffRandMaxDuration = ToInt32(textBox_generation_phaseOffMaxDuration.Text);
                config.PhaseDigitalLine = comboBox_generation_phaseDevice.Text + "/" + comboBox_generation_phasePort.Text + "/" + comboBox_generation_phaseLine.Text;
                if (config.PhaseOnRandMinDuration > config.PhaseOnRandMaxDuration
                    || config.PhaseOffRandMinDuration > config.PhaseOffRandMaxDuration)
                {
                    canRun = false;
                    MessageBox.Show("Wrong min-max values.", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                if (config.GeneratePhaseIO
                    && (comboBox_generation_phaseDevice.Text == string.Empty
                        || comboBox_generation_phasePort.Text == string.Empty
                        || comboBox_generation_phaseLine.Text == string.Empty))
                {
                    config.PhaseDigitalLine = "";
                    canRun = false;
                    MessageBox.Show("Phase device/port/line not valid.", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                config.PhaseOnActiveState = (comboBox_generation_phaseInputActive.Text == "Active Low") ? WaveformGenerator.DigitalLineActiveState.ActiveLow : WaveformGenerator.DigitalLineActiveState.ActiveHigh;
                config.GeneratePhaseTCP = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineTCP.Checked;
                config.PhaseTCPPort = ToInt32(textBox_generation_phaseTcpPort.Text);
                config.PhaseTCPPhaseonMsg = textBox_generation_phaseTcpPhaseonMesg.Text;
                config.PhaseTCPPhaseoffMsg = textBox_generation_phaseTcpPhaseoffMesg.Text;
                config.GeneratePhaseSerial = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineSerial.Checked;
                config.PhaseSerialPort = comboBox_generation_phaseSerialPort.Text;
                config.PhaseSerialPhaseonMsg = textBox_generation_phaseSerialPhaseonMesg.Text;
                config.PhaseSerialPhaseoffMsg = textBox_generation_phaseSerialPhaseoffMesg.Text;

                config.GenerateTrigger = checkBox_generation_trigger.Checked;
                config.GenerateTriggerIO = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineIO.Checked;
                config.TriggerDigitalLine = comboBox_generation_triggerDevice.Text + "/" + comboBox_generation_triggerPort.Text + "/" + comboBox_generation_triggerLine.Text;
                if (config.GenerateTriggerIO
                    && (comboBox_generation_triggerDevice.Text == string.Empty
                        || comboBox_generation_triggerPort.Text == string.Empty
                        || comboBox_generation_triggerLine.Text == string.Empty))
                {
                    config.TriggerDigitalLine = "";
                    canRun = false;
                    MessageBox.Show("Trigger device/port/line not valid.", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                config.TriggerDuration = ToInt32(textBox_generation_triggerDuration.Text);
                config.TriggerPeriod = ToInt32(textBox_generation_triggerPeriod.Text);
                config.TriggerActiveState = (comboBox_generation_triggerInputActive.Text == "Active Low") ? WaveformGenerator.DigitalLineActiveState.ActiveLow : WaveformGenerator.DigitalLineActiveState.ActiveHigh;
                config.TriggerOnPhaseOff = checkBox_generation_triggerOnPhaseOff.Checked;
                if (config.TriggerDuration >= config.TriggerPeriod)
                {
                    canRun = false;
                    MessageBox.Show("Wrong duration/period values.", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                config.GenerateTriggerTCP = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineTCP.Checked;
                config.TriggerTCPPort = ToInt32(textBox_generation_triggerTcpPort.Text);
                config.TriggerTCPTriggerMsg = textBox_generation_triggerTcpTriggerMesg.Text;
                config.GenerateTriggerSerial = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineSerial.Checked;
                config.TriggerSerialPort = comboBox_generation_triggerSerialPort.Text;
                config.TriggerSerialTriggerMsg = textBox_generation_triggerSerialTriggerMesg.Text;

                config.GenerateEncoder = checkBox_generation_encoder.Checked;
                config.EncoderDigitalLine = comboBox_generation_encoderDevice.Text + "/" + comboBox_generation_encoderPort.Text + "/" + comboBox_generation_encoderLine.Text;
                if (config.GenerateEncoder
                    && (comboBox_generation_encoderDevice.Text == string.Empty
                        || comboBox_generation_encoderPort.Text == string.Empty
                        || comboBox_generation_encoderLine.Text == string.Empty))
                {
                    config.EncoderDigitalLine = "";
                    canRun = false;
                    MessageBox.Show("Encoder device/port/line not valid.", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                config.EncoderGenerated = (comboBox_generation_encoderBasedOn.Text == "Frequency") ? EncoderGeneration.BasedOnFrequency : EncoderGeneration.BasedOnConveyorSpeed;
                if (ToDouble(textBox_generation_encoderFrequency.Text, out config.EncoderFrequency) == false)
                {
                    canRun = false;
                    MessageBox.Show("Encoder Frequency value not valid", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                config.ConveyorSpeed = ToInt32(textBox_generation_encoderSpeed.Text);
                if (ToDouble(textBox_generation_encoderSteps.Text, out config.EncoderSteps) == false)
                {
                    canRun = false;
                    MessageBox.Show("Encoder Steps value not valid", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                config.GenerateConveyorStartStop = checkBox_generation_encoderStartstop.Checked;
                config.ConveyorStartStopTransition = ToInt32(textBox_generation_encoderTransitionTime.Text);
                config.ConveyorOFFMaxDuration = ToInt32(textBox_generation_conveyorOFF_MaxDuration.Text);
                config.ConveyorOFFMinDuration = ToInt32(textBox_generation_conveyorOFF_MinDuration.Text);
                config.ConveyorONMaxDuration = ToInt32(textBox_generation_conveyorON_MaxDuration.Text);
                config.ConveyorONMinDuration = ToInt32(textBox_generation_conveyorON_MinDuration.Text);
                if (config.ConveyorOFFMinDuration > config.ConveyorOFFMaxDuration
                    || config.ConveyorONMinDuration > config.ConveyorONMaxDuration)
                {
                    canRun = false;
                    MessageBox.Show("Wrong min-max values.", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                config.GenerateProtocolIndex = checkBox_generation_protocolindex.Checked;
                config.ProtocolIndexMessage = textBox_generation_protocolIndexMessage.Text;
                config.ProtocolIndexCounter = ToInt32(textBox_generation_protocolIndexCounter.Text);
                config.GenerateFixedProtocolIndex = radioButton_generation_fixedProtocolIndex.Checked;
                config.ProtocolIndexFixedDelay = ToInt32(textBox_generation_protocolIndexFixedDelay.Text);
                config.ProtocolIndexRandMinDelay = ToInt32(textBox_generation_protocolIndexRandomMinDelay.Text);
                config.ProtocolIndexRandMaxDelay = ToInt32(textBox_generation_protocolIndexRandomMaxDelay.Text);
                if (config.ProtocolIndexRandMinDelay > config.ProtocolIndexRandMaxDelay)
                {
                    canRun = false;
                    MessageBox.Show("Wrong min-max values.", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                config.GenerateProtocolIndexTCP = config.GenerateProtocolIndex && checkBox_generation_protocolindexLineTCP.Checked;
                config.ProtocolIndexTCPPort = ToInt32(textBox_generation_protocolIndexTcpPort.Text);

                config.GenerateProtocolIndexSerial = config.GenerateProtocolIndex && checkBox_generation_protocolindexLineSerial.Checked;
                config.ProtocolIndexSerialPort = comboBox_generation_protocolIndexSerialPort.Text;
                if (checkboxPassThrough.Checked)
                {
                    Random rnd = new Random();
                    int minCycle = 0, maxCycle = 0;
                    if (PrepareTestPassThrough())
                    {
                        string data;
                        data = txtheaderpassthrough.Text + txtInputData.Text + txtterminatorPassthorugh.Text;
                        string channelInput = cbxInputChannelPassthrough.Text, Outputchannel = cbxOutputchannelPassthrough.Text, Fillingmode = cbxFillingMode.Text;
                        if (int.TryParse(txtPassthroughCycle.Text, out int cycle) && !checkRandom.Checked)
                        {

                        }
                        else if (checkRandom.Checked && int.TryParse(txtPassthroughMinCycle.Text, out minCycle) && int.TryParse(txtPassthroughMaxCycle.Text, out maxCycle))
                        {
                            cycle = 0;
                        }
                        else
                        {

                        }
                        shouldStop = false;
                        shouldStop2 = false;
                        if(cbxOutputchannelPassthrough.Text == "Main" || cbxOutputchannelPassthrough.Text == "Aux")
                        {
                            threadPassthrough2 = new Thread(OnSerialReceivMessage);
                            threadPassthrough2.Start();
                        }
                        else if(cbxOutputchannelPassthrough.Text == "Reader TCP Server")
                        {
                            threadPassthrough2 = new Thread(OnTCPServerReceiveMessage);
                            threadPassthrough2.Start();
                        }
                    threadPassthrough1 = new Thread(() => StartPassthrough(data, channelInput, Outputchannel, Fillingmode, cycle, minCycle, maxCycle));
                    threadPassthrough1.Start();
                }
                else
                    {
                        MessageBox.Show("Please fill all field in Passthrough tab");
                        return;
                    }
                }
                button_startTest.Enabled = false;
                button_stopTest.Enabled = true;
            if (canRun)
            {
                test1class = new Test1Class(this, config, logConfig);
                test1class.Ended += OnTestEnded;
                test1class.WorkInProgress += OnWorkInProgress;
                test1class.CountersIncremented += OnCountersIncremented;

                testThread = new Thread(test1class.DoTest);
                testThread.Name = "TestThread";
                testThread.Start();

                InitCounters();
            }
        }
    }
        private string portCOMIn, portCOMOut;
        private int portbaudIn, portdatabitsIn, portbaudOut, portdatabitsOut;
        Parity PortparityIn, PortparityOut;
        StopBits PortStopbisIn, PortStopbisOut;
        private bool PrepareTestPassThrough()
        {
            bool ret = true;
            ret = ret && cbxInputChannelPassthrough.Text.Length != 0 && cbxOutputchannelPassthrough.Text.Length != 0;
            ret = ret && !(cbxInputChannelPassthrough.Text == cbxOutputchannelPassthrough.Text);
            ret = ret && cbxFillingMode.Text.Length != 0;
            if (cbxFillingMode.Text == "Fixed Lenth")
            {
                ret = ret && txtLengthPassthrough.Text.Length == 0 && txtFillingPattern.Text.Length == 0 && txtFieldJustification.Text.Length == 0;
            }
            if (checkRandom.Checked)
            {
                ret = ret && int.TryParse(txtPassthroughMinCycle.Text, out int temp1) && int.TryParse(txtPassthroughMaxCycle.Text, out int temp2);
            }
            else
            {
                ret = ret && int.TryParse(txtPassthroughCycle.Text, out int temp3);
            }
            if(cbxInputChannelPassthrough.Text=="Main" || cbxInputChannelPassthrough.Text == "Aux")
            {
                portCOMIn = cbxPassthroughComInput.SelectedItem.ToString();
                portbaudIn = serialportArray[cbxPassthroughComInput.FindStringExact(cbxPassthroughComInput.SelectedItem.ToString())].baud;
                PortparityIn = serialportArray[cbxPassthroughComInput.FindStringExact(cbxPassthroughComInput.SelectedItem.ToString())].parity;
                portdatabitsIn = serialportArray[cbxPassthroughComInput.FindStringExact(cbxPassthroughComInput.SelectedItem.ToString())].databits;
                PortStopbisIn = serialportArray[cbxPassthroughComInput.FindStringExact(cbxPassthroughComInput.SelectedItem.ToString())].stopbits;
            }
            if (cbxOutputchannelPassthrough.Text == "Main" || cbxOutputchannelPassthrough.Text == "Aux")
            {
                portCOMOut = cbxPassthroughComoutput.SelectedItem.ToString();
                portbaudOut = serialportArray[cbxPassthroughComoutput.FindStringExact(cbxPassthroughComoutput.SelectedItem.ToString())].baud;
                PortparityOut = serialportArray[cbxPassthroughComoutput.FindStringExact(cbxPassthroughComoutput.SelectedItem.ToString())].parity;
                portdatabitsOut = serialportArray[cbxPassthroughComoutput.FindStringExact(cbxPassthroughComoutput.SelectedItem.ToString())].databits;
                PortStopbisOut = serialportArray[cbxPassthroughComoutput.FindStringExact(cbxPassthroughComoutput.SelectedItem.ToString())].stopbits;
            }


                return ret;
        }
        private volatile int Inputcount = 0, Outputcount = 1;
        private volatile bool shouldStop = false, shouldStop2 = false;
        private ClientConnection tcpPassthrough = null;
        private SerialPort serialPort = null;
        private void StartPassthrough(string data, string channelInput, string Outputchannel, string Fillingmode, int cycle, int mincylce, int maxcycle)
        {
            Random rnd = null;
            if (cycle != 0)
            {
                if (channelInput == "Reader TCP Server")
                {
                    int PassthroughIndexTCPPort = 51236;
                    tcpPassthrough = new ClientConnection(ipAddressControl_deviceIPAddress.IPAddress.ToString(), PassthroughIndexTCPPort);
                    //SerialPortItem serialport = configParams.serialportArray.Find(x => x.portName == configParams.ProtocolIndexSerialPort);

                    tcpPassthrough.Open();
                    while (!shouldStop)
                    {
                        tcpPassthrough.Send(data);
                        Inputcount++;
                        Thread.Sleep(cycle);
                    }
                    tcpPassthrough.Close();
                }
                else if (channelInput == "Main")
                { 
                    serialPort = new SerialPort(portCOMIn, portbaudIn, PortparityIn, portdatabitsIn, PortStopbisIn);
                    try
                    {
                        serialPort.Open();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    while (!shouldStop)
                    {
                        serialPort.Write(data);
                        Inputcount++;
                        Thread.Sleep(cycle);
                    }
                    serialPort.Close();
                }
                else if (channelInput == "Aux")
                {
                    serialPort = new SerialPort(portCOMIn, portbaudIn, PortparityIn, portdatabitsIn, PortStopbisIn);
                    try
                    {
                        serialPort.Open();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    while (!shouldStop)
                    {
                        serialPort.Write(data);
                        Inputcount++;
                        Thread.Sleep(cycle);
                    }
                    serialPort.Close();
                }
                else
                {
                // do nothing
                }
            }
            else
            {
                rnd = new Random();
                cycle = rnd.Next(mincylce, maxcycle);
                if (channelInput == "Reader TCP Server")
                {
                    int PassthroughIndexTCPPort = 51236;
                    ClientConnection tcpPassthrough = new ClientConnection(ipAddressControl_deviceIPAddress.IPAddress.ToString(), PassthroughIndexTCPPort);
                    tcpPassthrough.Open();
                    while (!shouldStop)
                    {
                        tcpPassthrough.Send(data);
                        Inputcount++;
                        Thread.Sleep(cycle);
                        cycle = rnd.Next(mincylce, maxcycle);

                    }
                    tcpPassthrough.Close();
                }
                else if (channelInput == "Main")
                {
                    serialPort = new SerialPort(portCOMIn, portbaudIn, PortparityIn, portdatabitsIn, PortStopbisIn);
                    try
                    {
                        serialPort.Open();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    while (!shouldStop)
                    {
                        serialPort.Write(data);
                        Inputcount++;
                        Thread.Sleep(cycle);
                        cycle = rnd.Next(mincylce, maxcycle);
                    }
                    serialPort.Close();
                }
                else if (channelInput == "Aux")
                {
                    serialPort = new SerialPort(portCOMIn, portbaudIn, PortparityIn, portdatabitsIn, PortStopbisIn);
                    try
                    {
                        serialPort.Open();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    while (!shouldStop)
                    {
                        serialPort.Write(data);
                        Inputcount++;
                        Thread.Sleep(cycle);
                        cycle = rnd.Next(mincylce, maxcycle);
                    }
                    serialPort.Close();
                }
                else
                {

                }
            }

        }
        private SerialPort serialPortReceive = null;
        private bool checkOutputCompareInput = true;
        StreamWriter streamWriter = new StreamWriter("logPassthrough.txt");
        private void OnSerialReceivMessage()
        {

            serialPortReceive = new SerialPort(portCOMOut, portbaudOut, PortparityOut, portdatabitsOut, PortStopbisOut);
            try
            {
                serialPortReceive.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            string message;
            Outputcount = 0;
            while (!shouldStop2)
            {
                try
                {
                    message = serialPortReceive.ReadLine();
                }
                catch
                {
                    return;
                }
                Outputcount++;
                if (message.Contains(txtInputData.Text))
                {

                }
                else
                {
                    checkOutputCompareInput = false;
                    streamWriter.WriteLine(DateTime.Now.ToString("MM-dd hh:mm:ss:fff") + "Output on serial" + message);
                }
            }
        }
        private void OnTCPServerReceiveMessage()
        {
            int PassthroughIndexTCPPort = 51236;
            tcpPassthrough = new ClientConnection(ipAddressControl_deviceIPAddress.IPAddress.ToString(), PassthroughIndexTCPPort);
            tcpPassthrough.Open();
            string message;
            while (!shouldStop2)
            {
                try
                {
                    message = tcpPassthrough.Receive();
                }
                catch
                {
                    break;
                }
                while (message == "")
                {
                    try
                    {
                        message = tcpPassthrough.Receive();
                    }
                    catch
                    {
                        break;
                    }
                }
                Outputcount++;
                if (message.Contains(txtInputData.Text))
                {

                }
                else
                {
                    checkOutputCompareInput = false;
                    streamWriter.WriteLine(DateTime.Now.ToString("MM-dd hh:mm:ss:fff") + "Output on TCP Reader" + message);
                }
            }
            tcpPassthrough.Close();
        }
        private static Int32 ToInt32(String value)
        {
            if (value == null)
                return 0;
            Int32 outValue;
            bool rc = Int32.TryParse(value, out outValue);
            if (!rc)
                return 0;
            return outValue;
        }

        private bool ToDouble(string doubleString, out double outDouble)
        {
            int idx = doubleString.IndexOf(',');
            if (idx != -1)
            {
                var stringBuilder = new StringBuilder(doubleString);
                stringBuilder[idx] = '.';
                doubleString = stringBuilder.ToString();
            }

            NumberStyles style = NumberStyles.Number;
            CultureInfo culture = CultureInfo.InvariantCulture;
            double retval;
            if (Double.TryParse(doubleString, style, culture, out retval))
            {
                outDouble = retval;
                return true;
            }
            outDouble = 0.0;
            return false;
        }

        private void button_stopTest_Click(object sender, EventArgs e)
        {
            shouldStop = true;
            if (test1class != null)
            {
                manageComponentsForWaitingStopTest();
                test1class.RequestStop();
            }
            else
            {
                manageComponentsForStopTest();
            }
            Thread.Sleep(5000);
            shouldStop2 = true;
            Thread.Sleep(100);
            txtCountIn.Text = Inputcount.ToString();
            txtCountOut.Text = Outputcount.ToString();
            Inputcount = 0;
            Outputcount = 1;
            if(serialPort !=null)
            {
                serialPort.Close();
            }
            if (checkOutputCompareInput)
            {
                txtOutputData.Text = txtInputData.Text;
            }
            else
            {
                txtOutputData.Text = "Failed. See log for detail";
            }
            if(tcpPassthrough != null)
            {
                tcpPassthrough.Close();
            }
            
        }

        private void InitCounters()
        {
            if (test1class == null)
                return;

            // in/out listbox   ///////////////////////////////////////////////////////////
            ListViewItem itm;
            string[] inoutarr = new string[2];

            listView_test1_InOutCounters.Items.Clear();

            if (checkBox_generation_encoder.Checked)
            {
                inoutarr[0] = "Encoder start/stop";
                inoutarr[1] = Convert.ToString(test1class.encoderStartStopCount);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);
            }

            if (checkBox_generation_phaseMode.Checked)
            {
                inoutarr[0] = "Phase Count";
                inoutarr[1] = Convert.ToString(test1class.phaseCount);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);
            }

            if (checkBox_generation_trigger.Checked)
            {
                inoutarr[0] = "Trigger Count";
                inoutarr[1] = Convert.ToString(test1class.triggerCount);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);
            }

            if (checkBox_test1_digitallines.Checked)
            {
                inoutarr[0] = "In1";
                inoutarr[1] = Convert.ToString(test1class.in1Count);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);

                inoutarr[0] = "In2";
                inoutarr[1] = Convert.ToString(test1class.in2Count);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);

                inoutarr[0] = "Out1";
                inoutarr[1] = Convert.ToString(test1class.out1Count);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);

                inoutarr[0] = "Out2";
                inoutarr[1] = Convert.ToString(test1class.out2Count);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);

                inoutarr[0] = "Out3";
                inoutarr[1] = Convert.ToString(test1class.out3Count);
                itm = new ListViewItem(inoutarr);
                listView_test1_InOutCounters.Items.Add(itm);
            }

            // messages listbox     ///////////////////////////////////////////////////////////
            listView_test1_MessagesReceived.Items.Clear();

            string[] arr = new string[(int)ListViewColumn.MAX_ListViewColumn];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = String.Empty;
            arr[(int)ListViewColumn.channel] = "TCP client";
            if (testTCPClient)
            {
                int index = (int)Test1Class.ChannelType.tcpClient;
                if (checkBox_test1_goodread.Checked)
                    arr[(int)ListViewColumn.goodreadCount] = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    arr[(int)ListViewColumn.noreadCount] = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    arr[(int)ListViewColumn.partialreadCount] = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    arr[(int)ListViewColumn.multiplereadCount] = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    arr[(int)ListViewColumn.heartbeatCount] = test1class.heartbeatCount[index].ToString();
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
            }
            itm = new ListViewItem(arr);
            listView_test1_MessagesReceived.Items.Add(itm);

            for (int i = 0; i < arr.Length; i++)
                arr[i] = String.Empty;
            arr[(int)ListViewColumn.channel] = "TCP server";
            if (testTCPServer)
            {
                int index = (int)Test1Class.ChannelType.tcpServer;
                if (checkBox_test1_goodread.Checked)
                    arr[(int)ListViewColumn.goodreadCount] = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    arr[(int)ListViewColumn.noreadCount] = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    arr[(int)ListViewColumn.partialreadCount] = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    arr[(int)ListViewColumn.multiplereadCount] = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    arr[(int)ListViewColumn.heartbeatCount] = test1class.heartbeatCount[index].ToString();
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
            }
            itm = new ListViewItem(arr);
            listView_test1_MessagesReceived.Items.Add(itm);

            for (int i = 0; i < arr.Length; i++)
                arr[i] = String.Empty;
            arr[(int)ListViewColumn.channel] = "UDP server";
            if (testUDPServer)
            {
                int index = (int)Test1Class.ChannelType.udpServer;
                if (checkBox_test1_goodread.Checked)
                    arr[(int)ListViewColumn.goodreadCount] = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    arr[(int)ListViewColumn.noreadCount] = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    arr[(int)ListViewColumn.partialreadCount] = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    arr[(int)ListViewColumn.multiplereadCount] = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    arr[(int)ListViewColumn.heartbeatCount] = test1class.heartbeatCount[index].ToString();
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
            }
            itm = new ListViewItem(arr);
            listView_test1_MessagesReceived.Items.Add(itm);

            for (int i = 0; i < arr.Length; i++)
                arr[i] = String.Empty;
            arr[(int)ListViewColumn.channel] = "Serial Main";
            if (testSerialMain)
            {
                int index = (int)Test1Class.ChannelType.mainPort;
                if (checkBox_test1_goodread.Checked)
                    arr[(int)ListViewColumn.goodreadCount] = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    arr[(int)ListViewColumn.noreadCount] = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    arr[(int)ListViewColumn.partialreadCount] = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    arr[(int)ListViewColumn.multiplereadCount] = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    arr[(int)ListViewColumn.heartbeatCount] = test1class.heartbeatCount[index].ToString();
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
            }
            itm = new ListViewItem(arr);
            listView_test1_MessagesReceived.Items.Add(itm);

            for (int i = 0; i < arr.Length; i++)
                arr[i] = String.Empty;
            arr[(int)ListViewColumn.channel] = "Serial AUX";
            if (testSerialAux)
            {
                int index = (int)Test1Class.ChannelType.auxPort;
                if (checkBox_test1_goodread.Checked)
                    arr[(int)ListViewColumn.goodreadCount] = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    arr[(int)ListViewColumn.noreadCount] = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    arr[(int)ListViewColumn.partialreadCount] = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    arr[(int)ListViewColumn.multiplereadCount] = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    arr[(int)ListViewColumn.heartbeatCount] = test1class.heartbeatCount[index].ToString();
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
            }
            itm = new ListViewItem(arr);
            listView_test1_MessagesReceived.Items.Add(itm);

        }

        private void CanUpdateCounters()
        {
            canUpdateCounters = true;
        }

        private void UpdateCounters()
        {
            if (test1class == null)
                return;

            int listviewItem = 0;
            if (checkBox_generation_encoder.Checked)
            {
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.encoderStartStopCount.ToString();
            }

            if (checkBox_generation_phaseMode.Checked)
            {
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.phaseCount.ToString();
            }

            if (checkBox_generation_trigger.Checked)
            {
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.triggerCount.ToString();
            }

            if (checkBox_test1_digitallines.Checked)
            {
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.in1Count.ToString();
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.in2Count.ToString();
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.out1Count.ToString();
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.out2Count.ToString();
                listView_test1_InOutCounters.Items[listviewItem++].SubItems[1].Text = test1class.out3Count.ToString();
            }

            if (testTCPClient)
            {
                int index = (int)Test1Class.ChannelType.tcpClient;
                if (checkBox_test1_goodread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.goodreadCount].Text = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.noreadCount].Text = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.partialreadCount].Text = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.multiplereadCount].Text = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.heartbeatCount].Text = test1class.heartbeatCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
            }
            if (testTCPServer)
            {
                int index = (int)Test1Class.ChannelType.tcpServer;
                if (checkBox_test1_goodread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.goodreadCount].Text = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.noreadCount].Text = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.partialreadCount].Text = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.multiplereadCount].Text = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.heartbeatCount].Text = test1class.heartbeatCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
            }
            if (testUDPServer)
            {
                int index = (int)Test1Class.ChannelType.udpServer;
                if (checkBox_test1_goodread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.goodreadCount].Text = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.noreadCount].Text = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.partialreadCount].Text = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.multiplereadCount].Text = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.heartbeatCount].Text = test1class.heartbeatCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
            }
            if (testSerialMain)
            {
                int index = (int)Test1Class.ChannelType.mainPort;
                if (checkBox_test1_goodread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.goodreadCount].Text = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.noreadCount].Text = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.partialreadCount].Text = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.multiplereadCount].Text = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.heartbeatCount].Text = test1class.heartbeatCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
            }
            if (testSerialAux)
            {
                int index = (int)Test1Class.ChannelType.auxPort;
                if (checkBox_test1_goodread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.goodreadCount].Text = test1class.goodreadCount[index].ToString();
                if (checkBox_test1_noread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.noreadCount].Text = test1class.noreadCount[index].ToString();
                if (checkBox_test1_partialread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.partialreadCount].Text = test1class.partialreadCount[index].ToString();
                if (checkBox_test1_multipleread.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.multiplereadCount].Text = test1class.multiplereadCount[index].ToString();
                if (checkBox_test1_heartbeat.Checked)
                    listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.heartbeatCount].Text = test1class.heartbeatCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
            }

            canUpdateCounters = false;
        }

        private void OnCountersIncremented(object sender, EventArgs e)
        {
            BeginInvoke(new Action(CanUpdateCounters));
        }

        private void OnTestEnded(object sender, EventArgs e)
        {
            BeginInvoke(new Action(testEnded));
        }

        private void OnWorkInProgress(object sender, EventArgs e)
        {
            BeginInvoke(new Action(testStarted));
        }

        private void testEnded()
        {
            if (test1class != null)
            {
                test1class.Ended -= OnTestEnded;
                test1class.WorkInProgress -= OnWorkInProgress;
                test1class.CountersIncremented -= OnCountersIncremented;

                testThread.Join();
                test1class = null;
                testThread = null;
            }
            timer_updateCounters.Enabled = false;
            manageComponentsForStopTest();
        }

        private void testStarted()
        {
            manageComponentsForStartTest();
        }

        private void digitallines_setVisualIntegrity()
        {
            label_test1_out1line.Visible = checkBox_test1_digitallines.Checked;
            textBox_test1_out1line.Visible = checkBox_test1_digitallines.Checked;
            label_test1_out2line.Visible = checkBox_test1_digitallines.Checked;
            textBox_test1_out2line.Visible = checkBox_test1_digitallines.Checked;
            label_test1_out3line.Visible = checkBox_test1_digitallines.Checked;
            textBox_test1_out3line.Visible = checkBox_test1_digitallines.Checked;
            label_test1_in1line.Visible = checkBox_test1_digitallines.Checked;
            textBox_test1_in1line.Visible = checkBox_test1_digitallines.Checked;
            label_test1_in2line.Visible = checkBox_test1_digitallines.Checked;
            textBox_test1_in2line.Visible = checkBox_test1_digitallines.Checked;
        }

        private void goodread_setVisualIntegrity()
        {
            label_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadTcpClientPortComment.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadTcpServerPortComment.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadUdpServerPortComment.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadPattern.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadPattern.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadMainPort.Visible = checkBox_test1_goodread.Checked;
            comboBox_test1_goodreadMainPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadAuxPort.Visible = checkBox_test1_goodread.Checked;
            comboBox_test1_goodreadAuxPort.Visible = checkBox_test1_goodread.Checked;
            ipAddressControl_test1_myAddressGoodRead.Visible = checkBox_test1_goodread.Checked;
        }

        private void noread_setVisualIntegrity()
        {
            label_test1_noreadTcpClientPort.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadTcpClientPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadTcpClientPortComment.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadTcpServerPortComment.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadUdpServerPortComment.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadPattern.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadPattern.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadMainPort.Visible = checkBox_test1_noread.Checked;
            comboBox_test1_noreadMainPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadAuxPort.Visible = checkBox_test1_noread.Checked;
            comboBox_test1_noreadAuxPort.Visible = checkBox_test1_noread.Checked;
            ipAddressControl_test1_myAddressNoRead.Visible = checkBox_test1_noread.Checked;
        }

        private void partialread_setVisualIntegrity()
        {
            label_test1_partialreadTcpClientPort.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadTcpClientPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadTcpClientPortComment.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadTcpServerPortComment.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadUdpServerPortComment.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadPattern.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadPattern.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadMainPort.Visible = checkBox_test1_partialread.Checked;
            comboBox_test1_partialreadMainPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadAuxPort.Visible = checkBox_test1_partialread.Checked;
            comboBox_test1_partialreadAuxPort.Visible = checkBox_test1_partialread.Checked;
            ipAddressControl_test1_myAddressPartialRead.Visible = checkBox_test1_partialread.Checked;
        }

        private void multipleread_setVisualIntegrity()
        {
            label_test1_multiplereadTcpClientPort.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadTcpClientPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadTcpClientPortComment.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadTcpServerPortComment.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadUdpServerPortComment.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
            comboBox_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
            comboBox_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
            ipAddressControl_test1_myAddressMultipleRead.Visible = checkBox_test1_multipleread.Checked;
        }

        private void heartbeat_setVisualIntegrity()
        {
            label_test1_heartbeatTcpClientPort.Visible = checkBox_test1_heartbeat.Checked;
            textBox_test1_heartbeatTcpClientPort.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatTcpClientPortComment.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatTcpServerPort.Visible = checkBox_test1_heartbeat.Checked;
            textBox_test1_heartbeatTcpServerPort.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatTcpServerPortComment.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatUdpServerPort.Visible = checkBox_test1_heartbeat.Checked;
            textBox_test1_heartbeatUdpServerPort.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatUdpServerPortComment.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatPattern.Visible = checkBox_test1_heartbeat.Checked;
            textBox_test1_heartbeatPattern.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatMainPort.Visible = checkBox_test1_heartbeat.Checked;
            comboBox_test1_heartbeatMainPort.Visible = checkBox_test1_heartbeat.Checked;
            label_test1_heartbeatAuxPort.Visible = checkBox_test1_heartbeat.Checked;
            comboBox_test1_heartbeatAuxPort.Visible = checkBox_test1_heartbeat.Checked;
            ipAddressControl_test1_myAddressHeartBeat.Visible = checkBox_test1_heartbeat.Checked;
        }

        private void phasegeneration_setVisualIntegrity()
        {
            radioButton_generation_periodicPhase.Visible = checkBox_generation_phaseMode.Checked;
            radioButton_generation_randomPhase.Visible = checkBox_generation_phaseMode.Checked;
            checkBox_generation_phaseOnEncoder.Visible = checkBox_generation_phaseMode.Checked;
            groupBox_generation_periodicPhase.Enabled = radioButton_generation_periodicPhase.Checked;
            groupBox_generation_periodicPhase.Visible = checkBox_generation_phaseMode.Checked;
            groupBox_generation_randomPhase.Enabled = !radioButton_generation_periodicPhase.Checked;
            groupBox_generation_randomPhase.Visible = checkBox_generation_phaseMode.Checked;
            checkBox_generation_triggerOnPhaseOff.Visible = (checkBox_generation_phaseMode.Checked && checkBox_generation_trigger.Checked);
            checkBox_generation_phaseLineIO.Visible = checkBox_generation_phaseMode.Checked;
            groupBox_generation_phaseLineIO.Visible = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineIO.Checked;
            checkBox_generation_phaseLineTCP.Visible = checkBox_generation_phaseMode.Checked;
            groupBox_generation_phaseLineTCP.Visible = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineTCP.Checked;
            checkBox_generation_phaseLineSerial.Visible = checkBox_generation_phaseMode.Checked;
            groupBox_generation_phaseLineSerial.Visible = checkBox_generation_phaseMode.Checked && checkBox_generation_phaseLineSerial.Checked;
        }

        private void triggergeneration_setVisualIntegrity()
        {
            label_generation_triggerDuration.Visible = checkBox_generation_trigger.Checked;
            textBox_generation_triggerDuration.Visible = checkBox_generation_trigger.Checked;
            label_generation_triggerPeriod.Visible = checkBox_generation_trigger.Checked;
            textBox_generation_triggerPeriod.Visible = checkBox_generation_trigger.Checked;
            checkBox_generation_triggerOnPhaseOff.Visible = (checkBox_generation_phaseMode.Checked && checkBox_generation_trigger.Checked);
            checkBox_generation_triggerLineIO.Visible = checkBox_generation_trigger.Checked;
            groupBox_generation_triggerLineIO.Visible = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineIO.Checked;
            checkBox_generation_triggerLineIO.Visible = checkBox_generation_trigger.Checked;
            groupBox_generation_triggerLineIO.Visible = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineIO.Checked;
            checkBox_generation_triggerLineTCP.Visible = checkBox_generation_trigger.Checked;
            groupBox_generation_triggerLineTCP.Visible = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineTCP.Checked;
            checkBox_generation_triggerLineSerial.Visible = checkBox_generation_trigger.Checked;
            groupBox_generation_triggerLineSerial.Visible = checkBox_generation_trigger.Checked && checkBox_generation_triggerLineSerial.Checked;
        }

        private void encodergeneration_setVisualIntegrity()
        {
            label_generation_encoderBasedOn.Visible = checkBox_generation_encoder.Checked;
            comboBox_generation_encoderBasedOn.Visible = checkBox_generation_encoder.Checked;
            groupBox_generation_frequency.Visible = checkBox_generation_encoder.Checked;
            label_generation_encoderSpeed.Enabled = (checkBox_generation_encoder.Checked && comboBox_generation_encoderBasedOn.SelectedIndex == 1);
            textBox_generation_encoderSpeed.Enabled = (checkBox_generation_encoder.Checked && comboBox_generation_encoderBasedOn.SelectedIndex == 1);
            label_generation_encoderSteps.Enabled = (checkBox_generation_encoder.Checked && comboBox_generation_encoderBasedOn.SelectedIndex == 1);
            textBox_generation_encoderSteps.Enabled = (checkBox_generation_encoder.Checked && comboBox_generation_encoderBasedOn.SelectedIndex == 1);
            label_generation_encoderFrequency.Enabled = (checkBox_generation_encoder.Checked && comboBox_generation_encoderBasedOn.SelectedIndex == 0);
            textBox_generation_encoderFrequency.Enabled = (checkBox_generation_encoder.Checked && comboBox_generation_encoderBasedOn.SelectedIndex == 0);
            checkBox_generation_encoderStartstop.Visible = checkBox_generation_encoder.Checked;
            groupBox_generation_startstop.Visible = checkBox_generation_encoder.Checked;
            groupBox_generation_startstop.Enabled = checkBox_generation_encoder.Checked && checkBox_generation_encoderStartstop.Checked;
            checkBox_generation_phaseOnEncoder.Checked = checkBox_generation_encoder.Checked;
            checkBox_generation_encoderLineIO.Visible = checkBox_generation_encoder.Checked;
            groupBox_generation_encoderLineIO.Visible = checkBox_generation_encoder.Checked && checkBox_generation_encoderLineIO.Checked;
        }

        private void protocolindexgeneration_setVisualIntegrity()
        {
            groupBox_generation_protocolIndexTiming.Visible = checkBox_generation_protocolindex.Checked;
            groupBox_generation_protocolIndexLine.Visible = checkBox_generation_protocolindex.Checked;

            groupBox_generation_protocolIndexTcpPort.Visible = checkBox_generation_protocolindexLineTCP.Checked;
            groupBox_generation_protocolIndexSerialPort.Visible = checkBox_generation_protocolindexLineSerial.Checked;

            groupBox_generation_fixedProtocolIndex.Visible = radioButton_generation_fixedProtocolIndex.Checked;
            groupBox_generation_randomProtocolIndex.Visible = radioButton_generation_randomProtocolIndex.Checked;
        }

        private bool encoderValuesChanged()
        {
            double mmperstep, frequency;
            if (comboBox_generation_encoderBasedOn.Text == "Frequency")
            {
                if (ToDouble(textBox_generation_encoderSteps.Text, out mmperstep) == false)
                    return false;
                if (ToDouble(textBox_generation_encoderFrequency.Text, out frequency) == false)
                    return false;
                Int32 speed = Convert.ToInt32(frequency * mmperstep);
                textBox_generation_encoderSpeed.Text = speed.ToString();
            }
            else
            {
                if (ToDouble(textBox_generation_encoderSteps.Text, out mmperstep) == false)
                    return false;
                Int32 speed = ToInt32(textBox_generation_encoderSpeed.Text);
                frequency = speed / mmperstep;
                if (double.IsInfinity(frequency) == false)
                {
                    frequency = Math.Truncate(frequency * 100) / 100;
                    textBox_generation_encoderFrequency.Text = frequency.ToString();
                }
            }
            return true;
        }

        private void checkBox_test1_digitallines_CheckedChanged(object sender, EventArgs e)
        {
            digitallines_setVisualIntegrity();
        }

        private void checkBox_test1_goodread_CheckedChanged(object sender, EventArgs e)
        {
            goodread_setVisualIntegrity();
        }

        private void checkBox_test1_noread_CheckedChanged(object sender, EventArgs e)
        {
            noread_setVisualIntegrity();
        }

        private void checkBox_test1_partialread_CheckedChanged(object sender, EventArgs e)
        {
            partialread_setVisualIntegrity();
        }

        private void checkBox_test1_multipleread_CheckedChanged(object sender, EventArgs e)
        {
            multipleread_setVisualIntegrity();
        }

        private void checkBox_test1_heartbeat_CheckedChanged(object sender, EventArgs e)
        {
            heartbeat_setVisualIntegrity();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (test1class != null)
            {
                test1class.RequestStop();
                testThread.Join();
                test1class.Ended -= new EventHandler(OnTestEnded);
                test1class.WorkInProgress -= new EventHandler(OnWorkInProgress);
                test1class = null;
            }
            saveSettings();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            initComponents();
            fillDigitalLinesData();
            loadSettings();
            debug_fillData();
            for(int i = 0;i< serialportArray.Count;i++)
            {
                cbxPassthroughComInput.Items.Add(serialportArray[i].portName);
                cbxPassthroughComoutput.Items.Add(serialportArray[i].portName);
            }
        }

        private void comboBox_netAdapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            ipAddressControl_test1_myAddressGoodRead.Text = comboBox_netAdapters.Text;
            ipAddressControl_test1_myAddressNoRead.Text = comboBox_netAdapters.Text;
            ipAddressControl_test1_myAddressPartialRead.Text = comboBox_netAdapters.Text;
            ipAddressControl_test1_myAddressMultipleRead.Text = comboBox_netAdapters.Text;
            ipAddressControl_test1_myAddressHeartBeat.Text = comboBox_netAdapters.Text;
        }

        private void timer_updateCounters_Tick(object sender, EventArgs e)
        {
            if (canUpdateCounters)
                UpdateCounters();
        }

        private void button_loggingDialog_Click(object sender, EventArgs e)
        {
            Form_LoggingDialog loggingForm = new Form_LoggingDialog(logConfig);
            if (loggingForm.ShowDialog(this) == DialogResult.OK)
            {
                logConfig = loggingForm.Config;
            }
        }

        private void checkBox_generation_phaseMode_CheckedChanged(object sender, EventArgs e)
        {
            phasegeneration_setVisualIntegrity();
        }

        private void radioButton_generation_periodicPhase_CheckedChanged(object sender, EventArgs e)
        {
            phasegeneration_setVisualIntegrity();
        }

        private void checkBox_generation_trigger_CheckedChanged(object sender, EventArgs e)
        {
            triggergeneration_setVisualIntegrity();
        }

        private void checkBox_generation_encoder_CheckedChanged(object sender, EventArgs e)
        {
            encodergeneration_setVisualIntegrity();
        }

        private void checkBox_generation_startstop_CheckedChanged(object sender, EventArgs e)
        {
            encodergeneration_setVisualIntegrity();
        }

        private void comboBox_generation_encoderBasedOn_SelectedIndexChanged(object sender, EventArgs e)
        {
            encodergeneration_setVisualIntegrity();
        }

        private void checkBox_generation_phaseLineIO_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_generation_phaseLineIO.Checked)
            {
                string[] chans = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.All, PhysicalChannelAccess.External);
                if (chans == null || chans.Length == 0)
                {
                    MessageBox.Show("I/O device not detected. Disabling digital Phase generation", "Device failure", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    checkBox_generation_phaseLineIO.Checked = false;
                }
                //                 else
                //                     fillDigitalLinesData();
            }
            phasegeneration_setVisualIntegrity();
        }

        private void checkBox_generation_triggerLineIO_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_generation_triggerLineIO.Checked)
            {
                string[] chans = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.All, PhysicalChannelAccess.External);
                if (chans == null || chans.Length == 0)
                {
                    MessageBox.Show("I/O device not detected. Disabling digital Trigger generation", "Device failure", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    checkBox_generation_triggerLineIO.Checked = false;
                }
                //                 else
                //                     fillDigitalLinesData();
            }
            triggergeneration_setVisualIntegrity();
        }

        private void checkBox_generation_encoderLineIO_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_generation_encoderLineIO.Checked)
            {
                string[] chans = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.All, PhysicalChannelAccess.External);
                if (chans == null || chans.Length == 0)
                {
                    MessageBox.Show("I/O device not detected. Disabling digital Encoder generation", "Device failure", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    checkBox_generation_encoderLineIO.Checked = false;
                }
                //                 else
                //                     fillDigitalLinesData();
            }
            encodergeneration_setVisualIntegrity();
        }

        private void textBox_generation_encoderFrequency_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox_generation_encoderSteps_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox_generation_encoderSpeed_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox_generation_phaseLineTCP_CheckedChanged(object sender, EventArgs e)
        {
            phasegeneration_setVisualIntegrity();
        }

        private void checkBox_generation_phaseLineSerial_CheckedChanged(object sender, EventArgs e)
        {
            phasegeneration_setVisualIntegrity();
        }

        private void checkBox_generation_triggerLineTCP_CheckedChanged(object sender, EventArgs e)
        {
            triggergeneration_setVisualIntegrity();
        }

        private void checkBox_generation_triggerLineSerial_CheckedChanged(object sender, EventArgs e)
        {
            triggergeneration_setVisualIntegrity();
        }

        private void textBox_generation_encoderSpeed_Leave(object sender, EventArgs e)
        {
            double speed;
            if (ToDouble(textBox_generation_encoderSpeed.Text, out speed) == false)
            {
                MessageBox.Show("Encoder Speed value not valid", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
                encoderValuesChanged();
            encodergeneration_setVisualIntegrity();
        }

        private void textBox_generation_encoderSteps_Leave(object sender, EventArgs e)
        {
            double mmperstep;
            if (ToDouble(textBox_generation_encoderSteps.Text, out mmperstep) == false)
            {
                MessageBox.Show("Encoder Steps value not valid", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
                encoderValuesChanged();
            encodergeneration_setVisualIntegrity();
        }

        private void textBox_generation_encoderFrequency_Leave(object sender, EventArgs e)
        {
            double frequency;
            if (ToDouble(textBox_generation_encoderFrequency.Text, out frequency) == false)
            {
                MessageBox.Show("Encoder Frequency value not valid", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
                encoderValuesChanged();
            encodergeneration_setVisualIntegrity();
        }

        private void button_serialSettingsDialog_Click(object sender, EventArgs e)
        {
            Form_SerialSettingsDialog serialSettingsForm = new Form_SerialSettingsDialog(serialportArray);
            if (serialSettingsForm.ShowDialog(this) == DialogResult.OK)
            {
                serialportArray.Clear();
                foreach (SerialPortItem serial in serialSettingsForm.serialArray)
                {
                    serialportArray.Add(serial);
                }
            }
        }

        private void checkBox_generation_protocolindex_CheckedChanged(object sender, EventArgs e)
        {
            protocolindexgeneration_setVisualIntegrity();
        }

        private void checkBox_generation_protocolindexLineTCP_CheckedChanged(object sender, EventArgs e)
        {
            protocolindexgeneration_setVisualIntegrity();
        }

        private void checkBox_generation_protocolindexLineSerial_CheckedChanged(object sender, EventArgs e)
        {
            protocolindexgeneration_setVisualIntegrity();
        }

        private void radioButton_generation_fixedProtocolIndex_CheckedChanged(object sender, EventArgs e)
        {
            protocolindexgeneration_setVisualIntegrity();
        }

        private void radioButton_generation_randomProtocolIndex_CheckedChanged(object sender, EventArgs e)
        {
            protocolindexgeneration_setVisualIntegrity();
        }

        private void cbxInputChannelPassthrough_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbxInputChannelPassthrough.Text == "Main" || cbxInputChannelPassthrough.Text == "Serial")
            {
                cbxPassthroughComInput.Visible = true;
            }
            else
            {
                cbxPassthroughComInput.Visible = false;
            }
        }

        private void cbxOutputchannelPassthrough_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbxOutputchannelPassthrough.Text=="Main" || cbxOutputchannelPassthrough.Text == "Aux")
            {
                cbxPassthroughComoutput.Visible = true;
            }
            else
            {
                cbxPassthroughComoutput.Visible = false;
            }
        }

        private void checkboxPassThrough_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxpassthroughInputData.Visible = checkboxPassThrough.Checked;
            groupboxPassthroughContentField.Visible = checkboxPassThrough.Checked;
            groupBoxOutputChannels.Visible = checkboxPassThrough.Checked;
            groupBoxInputData.Visible = checkboxPassThrough.Checked;
            groupBoxOutputData.Visible = checkboxPassThrough.Checked;

        }

        private void tabPage_test1_digitalLines_Click(object sender, EventArgs e)
        {

        }

        private void checkRandom_CheckedChanged(object sender, EventArgs e)
        {
            txtPassthroughCycle.Enabled = !checkRandom.Checked;
            txtPassthroughMinCycle.Visible = checkRandom.Checked;
            txtPassthroughMaxCycle.Visible = checkRandom.Checked;
            label16.Visible = checkRandom.Checked;
            label17.Visible = checkRandom.Checked;
        }
    }
}

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

namespace TestMatrix_channels
{
    using Test1;

    public partial class MainForm : Form
    {

        enum ListViewColumn
        {
            channel = 0,
            goodreadCount,
            noreadCount,
            partialreadCount,
            multiplereadCount,
            totalMessageCount,
            outoforderPhases,
            missingPhases,

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
        private Test1LoggingConfigurationParameters logConfig = new Test1LoggingConfigurationParameters();

        public MainForm()
        {
            testTCPClient = false;
            testTCPServer = false;
            testUDPServer = false;
            testSerialMain = false;
            testSerialAux = false;

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

        private void initComponents()
        {
            try
            {
                test1Running = false;
                groupBox_settingTest1.Enabled = !test1Running;
                groupBox_monitorTest1.Enabled = test1Running;
                button_startTest1.Enabled = !test1Running;
                button_stopTest1.Enabled = test1Running;

                checkBox_test1_goodread.Checked = false;
                label_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
                textBox_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
                textBox_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
                label_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
                textBox_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
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
                label_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
                textBox_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
                label_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
                textBox_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
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
                label_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
                textBox_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
                label_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
                textBox_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
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
                label_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
                textBox_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
                textBox_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
                textBox_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
                comboBox_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
                label_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
                comboBox_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
                ipAddressControl_test1_myAddressMultipleRead.Visible = checkBox_test1_multipleread.Checked;
                ipAddressControl_test1_myAddressMultipleRead.Enabled = false;

                checkBox_test1_phaseanalysis.Checked = false;
                label_test1_phaseanalysisTcpClientPort.Visible = checkBox_test1_phaseanalysis.Checked;
                textBox_test1_phaseanalysisTcpClientPort.Visible = checkBox_test1_phaseanalysis.Checked;
                label_test1_phaseanalysisPattern.Visible = checkBox_test1_phaseanalysis.Checked;
                textBox_test1_phaseanalysisPattern.Visible = checkBox_test1_phaseanalysis.Checked;
                label_test1_phaseanalysisPatternComment.Visible = checkBox_test1_phaseanalysis.Checked;
                label_test1_phaseanalysisMainPort.Visible = checkBox_test1_phaseanalysis.Checked;
                comboBox_test1_phaseanalysisMainPort.Visible = checkBox_test1_phaseanalysis.Checked;
                label_test1_phaseanalysisAuxPort.Visible = checkBox_test1_phaseanalysis.Checked;
                comboBox_test1_phaseanalysisAuxPort.Visible = checkBox_test1_phaseanalysis.Checked;

                comboBox_test1_goodreadMainPort.Items.Add("Not used");
                comboBox_test1_goodreadAuxPort.Items.Add("Not used");
                comboBox_test1_noreadMainPort.Items.Add("Not used");
                comboBox_test1_noreadAuxPort.Items.Add("Not used");
                comboBox_test1_partialreadMainPort.Items.Add("Not used");
                comboBox_test1_partialreadAuxPort.Items.Add("Not used");
                comboBox_test1_multiplereadMainPort.Items.Add("Not used");
                comboBox_test1_multiplereadAuxPort.Items.Add("Not used");

                listView_test1_MessagesReceived.View = View.Details;
                listView_test1_MessagesReceived.GridLines = true;
                listView_test1_MessagesReceived.FullRowSelect = false;
                listView_test1_MessagesReceived.Columns.Clear();

                listView_test1_InOutCounters.View = View.Details;
                listView_test1_InOutCounters.GridLines = true;
                listView_test1_InOutCounters.FullRowSelect = false;
                listView_test1_InOutCounters.Columns.Clear();
                listView_test1_InOutCounters.Columns.Add("I/O", 55);
                listView_test1_InOutCounters.Columns.Add("Count", 68);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("1 - " + ex.Message);
            }

            try
            {
                // fill serial lines combobox
                string[] ArrayComPortsNames = null;
                int index = -1;
                try
                {
                    ArrayComPortsNames = SerialPort.GetPortNames();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("GetPortNames - " + ex.Message);
                }

                if (ArrayComPortsNames != null)
                {
                    if (ArrayComPortsNames.GetUpperBound(0) >= 0)
                    {
                        Array.Sort(ArrayComPortsNames);

                        do
                        {
                            index++;
                            comboBox_test1_goodreadMainPort.Items.Add(ArrayComPortsNames[index]);
                            comboBox_test1_goodreadAuxPort.Items.Add(ArrayComPortsNames[index]);
                            comboBox_test1_noreadMainPort.Items.Add(ArrayComPortsNames[index]);
                            comboBox_test1_noreadAuxPort.Items.Add(ArrayComPortsNames[index]);
                            comboBox_test1_partialreadMainPort.Items.Add(ArrayComPortsNames[index]);
                            comboBox_test1_partialreadAuxPort.Items.Add(ArrayComPortsNames[index]);
                            comboBox_test1_multiplereadMainPort.Items.Add(ArrayComPortsNames[index]);
                            comboBox_test1_multiplereadAuxPort.Items.Add(ArrayComPortsNames[index]);
                            switch (index)
                            {
                                case 0:
                                    tabPage_test1_1stSerialPort.Text = ArrayComPortsNames[index];

                                    comboBox_test1_1stBaud.Items.Add(300);
                                    comboBox_test1_1stBaud.Items.Add(600);
                                    comboBox_test1_1stBaud.Items.Add(1200);
                                    comboBox_test1_1stBaud.Items.Add(2400);
                                    comboBox_test1_1stBaud.Items.Add(9600);
                                    comboBox_test1_1stBaud.Items.Add(14400);
                                    comboBox_test1_1stBaud.Items.Add(19200);
                                    comboBox_test1_1stBaud.Items.Add(38400);
                                    comboBox_test1_1stBaud.Items.Add(57600);
                                    comboBox_test1_1stBaud.Items.Add(115200);
                                    comboBox_test1_1stBaud.Items.ToString();
                                    comboBox_test1_1stBaud.Text = comboBox_test1_1stBaud.Items[9].ToString();

                                    comboBox_test1_1stDataSize.Items.Add(7);
                                    comboBox_test1_1stDataSize.Items.Add(8);
                                    comboBox_test1_1stDataSize.Items.ToString();
                                    comboBox_test1_1stDataSize.Text = comboBox_test1_1stDataSize.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Parity)))
                                    {
                                        comboBox_test1_1stParity.Items.Add(s);
                                    }
                                    comboBox_test1_1stParity.Text = comboBox_test1_1stParity.Items[0].ToString();

                                    foreach (string s in Enum.GetNames(typeof(StopBits)))
                                    {
                                        comboBox_test1_1stStopBits.Items.Add(s);
                                    }
                                    comboBox_test1_1stStopBits.Text = comboBox_test1_1stStopBits.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Handshake)))
                                    {
                                        comboBox_test1_1stHandshake.Items.Add(s);
                                    }
                                    comboBox_test1_1stHandshake.Text = comboBox_test1_1stHandshake.Items[0].ToString();
                                    break;

                                case 1:
                                    tabPage_test1_2ndSerialPort.Text = ArrayComPortsNames[index];

                                    comboBox_test1_2ndBaud.Items.Add(300);
                                    comboBox_test1_2ndBaud.Items.Add(600);
                                    comboBox_test1_2ndBaud.Items.Add(1200);
                                    comboBox_test1_2ndBaud.Items.Add(2400);
                                    comboBox_test1_2ndBaud.Items.Add(9600);
                                    comboBox_test1_2ndBaud.Items.Add(14400);
                                    comboBox_test1_2ndBaud.Items.Add(19200);
                                    comboBox_test1_2ndBaud.Items.Add(38400);
                                    comboBox_test1_2ndBaud.Items.Add(57600);
                                    comboBox_test1_2ndBaud.Items.Add(115200);
                                    comboBox_test1_2ndBaud.Items.ToString();
                                    comboBox_test1_2ndBaud.Text = comboBox_test1_2ndBaud.Items[9].ToString();

                                    comboBox_test1_2ndDataSize.Items.Add(7);
                                    comboBox_test1_2ndDataSize.Items.Add(8);
                                    comboBox_test1_2ndDataSize.Items.ToString();
                                    comboBox_test1_2ndDataSize.Text = comboBox_test1_2ndDataSize.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Parity)))
                                    {
                                        comboBox_test1_2ndParity.Items.Add(s);
                                    }
                                    comboBox_test1_2ndParity.Text = comboBox_test1_2ndParity.Items[0].ToString();

                                    foreach (string s in Enum.GetNames(typeof(StopBits)))
                                    {
                                        comboBox_test1_2ndStopBits.Items.Add(s);
                                    }
                                    comboBox_test1_2ndStopBits.Text = comboBox_test1_2ndStopBits.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Handshake)))
                                    {
                                        comboBox_test1_2ndHandshake.Items.Add(s);
                                    }
                                    comboBox_test1_2ndHandshake.Text = comboBox_test1_2ndHandshake.Items[0].ToString();
                                    break;

                                case 2:
                                    tabPage_test1_3rdSerialPort.Text = ArrayComPortsNames[index];

                                    comboBox_test1_3rdBaud.Items.Add(300);
                                    comboBox_test1_3rdBaud.Items.Add(600);
                                    comboBox_test1_3rdBaud.Items.Add(1200);
                                    comboBox_test1_3rdBaud.Items.Add(2400);
                                    comboBox_test1_3rdBaud.Items.Add(9600);
                                    comboBox_test1_3rdBaud.Items.Add(14400);
                                    comboBox_test1_3rdBaud.Items.Add(19200);
                                    comboBox_test1_3rdBaud.Items.Add(38400);
                                    comboBox_test1_3rdBaud.Items.Add(57600);
                                    comboBox_test1_3rdBaud.Items.Add(115200);
                                    comboBox_test1_3rdBaud.Items.ToString();
                                    comboBox_test1_3rdBaud.Text = comboBox_test1_3rdBaud.Items[9].ToString();

                                    comboBox_test1_3rdDataSize.Items.Add(7);
                                    comboBox_test1_3rdDataSize.Items.Add(8);
                                    comboBox_test1_3rdDataSize.Items.ToString();
                                    comboBox_test1_3rdDataSize.Text = comboBox_test1_3rdDataSize.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Parity)))
                                    {
                                        comboBox_test1_3rdParity.Items.Add(s);
                                    }
                                    comboBox_test1_3rdParity.Text = comboBox_test1_3rdParity.Items[0].ToString();

                                    foreach (string s in Enum.GetNames(typeof(StopBits)))
                                    {
                                        comboBox_test1_3rdStopBits.Items.Add(s);
                                    }
                                    comboBox_test1_3rdStopBits.Text = comboBox_test1_3rdStopBits.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Handshake)))
                                    {
                                        comboBox_test1_3rdHandshake.Items.Add(s);
                                    }
                                    comboBox_test1_3rdHandshake.Text = comboBox_test1_3rdHandshake.Items[0].ToString();
                                    break;

                                case 3:
                                    tabPage_test1_4thSerialPort.Text = ArrayComPortsNames[index];

                                    comboBox_test1_4thBaud.Items.Add(300);
                                    comboBox_test1_4thBaud.Items.Add(600);
                                    comboBox_test1_4thBaud.Items.Add(1200);
                                    comboBox_test1_4thBaud.Items.Add(2400);
                                    comboBox_test1_4thBaud.Items.Add(9600);
                                    comboBox_test1_4thBaud.Items.Add(14400);
                                    comboBox_test1_4thBaud.Items.Add(19200);
                                    comboBox_test1_4thBaud.Items.Add(38400);
                                    comboBox_test1_4thBaud.Items.Add(57600);
                                    comboBox_test1_4thBaud.Items.Add(115200);
                                    comboBox_test1_4thBaud.Items.ToString();
                                    comboBox_test1_4thBaud.Text = comboBox_test1_4thBaud.Items[9].ToString();

                                    comboBox_test1_4thDataSize.Items.Add(7);
                                    comboBox_test1_4thDataSize.Items.Add(8);
                                    comboBox_test1_4thDataSize.Items.ToString();
                                    comboBox_test1_4thDataSize.Text = comboBox_test1_4thDataSize.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Parity)))
                                    {
                                        comboBox_test1_4thParity.Items.Add(s);
                                    }
                                    comboBox_test1_4thParity.Text = comboBox_test1_4thParity.Items[0].ToString();

                                    foreach (string s in Enum.GetNames(typeof(StopBits)))
                                    {
                                        comboBox_test1_4thStopBits.Items.Add(s);
                                    }
                                    comboBox_test1_4thStopBits.Text = comboBox_test1_4thStopBits.Items[1].ToString();

                                    foreach (string s in Enum.GetNames(typeof(Handshake)))
                                    {
                                        comboBox_test1_4thHandshake.Items.Add(s);
                                    }
                                    comboBox_test1_4thHandshake.Text = comboBox_test1_4thHandshake.Items[0].ToString();
                                    break;
                            }
                        }
                        while (index != ArrayComPortsNames.GetUpperBound(0));
                    }

                    for (++index; index < 4; index++)
                    {
                        switch (index)
                        {
                            case 0:
                                tabControl_test1_serialConfig.TabPages.Remove(tabPage_test1_1stSerialPort);
                                break;
                            case 1:
                                tabControl_test1_serialConfig.TabPages.Remove(tabPage_test1_2ndSerialPort);
                                break;
                            case 2:
                                tabControl_test1_serialConfig.TabPages.Remove(tabPage_test1_3rdSerialPort);
                                break;
                            case 3:
                                tabControl_test1_serialConfig.TabPages.Remove(tabPage_test1_4thSerialPort);
                                break;
                        }
                    }

                    comboBox_test1_goodreadMainPort.SelectedIndex = 0;
                    comboBox_test1_goodreadAuxPort.SelectedIndex = 0;
                    comboBox_test1_noreadMainPort.SelectedIndex = 0;
                    comboBox_test1_noreadAuxPort.SelectedIndex = 0;
                    comboBox_test1_partialreadMainPort.SelectedIndex = 0;
                    comboBox_test1_partialreadAuxPort.SelectedIndex = 0;
                    comboBox_test1_multiplereadMainPort.SelectedIndex = 0;
                    comboBox_test1_multiplereadAuxPort.SelectedIndex = 0;
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
                        comboBox_test1_netAdapters.Items.Add(localIP);
                    }
                }
                comboBox_test1_netAdapters.SelectedIndex = 0;
                comboBox_test1_netAdapters.Visible = (count > 1);
                label_test1_networkAdapters.Visible = (count > 1);

                ipAddressControl_test1_myAddressGoodRead.Text = comboBox_test1_netAdapters.Text;
                ipAddressControl_test1_myAddressNoRead.Text = comboBox_test1_netAdapters.Text;
                ipAddressControl_test1_myAddressPartialRead.Text = comboBox_test1_netAdapters.Text;
                ipAddressControl_test1_myAddressMultipleRead.Text = comboBox_test1_netAdapters.Text;

                ipAddressControl_test1.Select();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("3 - " + ex.Message);
            }
        }

        private void debug_fillData()
        {
//             ipAddressControl_test1.Text = "192.168.0.102";
//             comboBox_test1_netAdapters.SelectedIndex = 1;

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
        }

        private void manageComponentsForStartTest()
        {
            test1Running = true;
            groupBox_settingTest1.Enabled = !test1Running;
            groupBox_monitorTest1.Enabled = test1Running;
            button_startTest1.Enabled = !test1Running;
            button_stopTest1.Enabled = test1Running;
            button_stopTest1.Focus();
        }

        private void manageComponentsForStopTest()
        {
            test1Running = false;
            groupBox_settingTest1.Enabled = !test1Running;
            groupBox_monitorTest1.Enabled = test1Running;
            button_startTest1.Enabled = !test1Running;
            button_stopTest1.Enabled = test1Running;
            button_startTest1.Focus();
        }

        private bool isTCPClientTest()
        {
            bool tocheck = false;
            if (checkBox_test1_goodread.Checked && textBox_test1_goodreadTcpClientPort.Text != string.Empty
                || checkBox_test1_noread.Checked && textBox_test1_noreadTcpClientPort.Text != string.Empty
                || checkBox_test1_partialread.Checked && textBox_test1_partialreadTcpClientPort.Text != string.Empty
                || checkBox_test1_multipleread.Checked && textBox_test1_multiplereadTcpClientPort.Text != string.Empty
                || checkBox_test1_phaseanalysis.Checked && textBox_test1_phaseanalysisTcpClientPort.Text != string.Empty
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
                || checkBox_test1_phaseanalysis.Checked && comboBox_test1_phaseanalysisMainPort.SelectedIndex != 0
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
                || checkBox_test1_phaseanalysis.Checked && comboBox_test1_phaseanalysisAuxPort.SelectedIndex != 0
                )
            {
                tocheck = true;
            }
            return tocheck;
        }

        private void button_startTest1_Click(object sender, EventArgs e)
        {
            bool canRun = true;

            timer_updateCounters.Enabled = true;

            testTCPClient = isTCPClientTest();
            testTCPServer = isTCPServerTest();
            testUDPServer = isUDPServerTest();
            testSerialMain = isSerialMainTest();
            testSerialAux = isSerialAUXTest();

            if (   checkBox_test1_goodread.Checked
                || checkBox_test1_noread.Checked
                || checkBox_test1_partialread.Checked
                || checkBox_test1_multipleread.Checked
                || checkBox_test1_phaseanalysis.Checked)
            {
                if (ipAddressControl_test1.AnyBlank)
                {
                    MessageBox.Show("IP Address not valid", "Not valid data Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (canRun)
            {
                Test1ConfigurationParameters config = new Test1ConfigurationParameters();
                config.DeviceAddr = ipAddressControl_test1.IPAddress;
                config.ServerAddr = IPAddress.Parse(comboBox_test1_netAdapters.Text);

                foreach (TabPage page in tabControl_test1_serialConfig.TabPages)
                {
                    if (page.Name == "tabPage_test1_1stSerialPort")
                    {
                        string portName = page.Text;
                        int baud = Convert.ToInt32(comboBox_test1_1stBaud.Text);
                        Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_test1_1stParity.Text);
                        int databits = Convert.ToInt32(comboBox_test1_1stDataSize.Text);
                        StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_test1_1stStopBits.Text);
                        Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_test1_1stHandshake.Text);
                        config.AddSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                    }
                    if (page.Name == "tabPage_test1_2ndSerialPort")
                    {
                        string portName = page.Text;
                        int baud = Convert.ToInt32(comboBox_test1_2ndBaud.Text);
                        Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_test1_2ndParity.Text);
                        int databits = Convert.ToInt32(comboBox_test1_2ndDataSize.Text);
                        StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_test1_2ndStopBits.Text);
                        Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_test1_2ndHandshake.Text);
                        config.AddSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                    }
                    if (page.Name == "tabPage_test1_3rdSerialPort")
                    {
                        string portName = page.Text;
                        int baud = Convert.ToInt32(comboBox_test1_3rdBaud.Text);
                        Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_test1_3rdParity.Text);
                        int databits = Convert.ToInt32(comboBox_test1_3rdDataSize.Text);
                        StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_test1_3rdStopBits.Text);
                        Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_test1_3rdHandshake.Text);
                        config.AddSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                    }
                    if (page.Name == "tabPage_test1_4thSerialPort")
                    {
                        string portName = page.Text;
                        int baud = Convert.ToInt32(comboBox_test1_4thBaud.Text);
                        Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_test1_4thParity.Text);
                        int databits = Convert.ToInt32(comboBox_test1_4thDataSize.Text);
                        StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_test1_4thStopBits.Text);
                        Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_test1_4thHandshake.Text);
                        config.AddSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                    }
                }

                config.TestGoodRead = checkBox_test1_goodread.Checked;
                config.GoodRead_pattern = textBox_test1_goodreadPattern.Text;
                try { config.GoodRead_TCPClient_hostPort = Convert.ToInt32(textBox_test1_goodreadTcpClientPort.Text); }
                catch { config.GoodRead_TCPClient_hostPort = 0; }
                try { config.GoodRead_TCPServer_hostPort = Convert.ToInt32(textBox_test1_goodreadTcpServerPort.Text); }
                catch { config.GoodRead_TCPServer_hostPort = 0; }
                try { config.GoodRead_UDPServer_hostPort = Convert.ToInt32(textBox_test1_goodreadUdpServerPort.Text); }
                catch { config.GoodRead_UDPServer_hostPort = 0; }
                config.GoodRead_SerialMain_comPort = comboBox_test1_goodreadMainPort.Text;
                config.GoodRead_SerialAUX_comPort = comboBox_test1_goodreadAuxPort.Text;

                config.TestNoRead = checkBox_test1_noread.Checked;
                config.NoRead_pattern = textBox_test1_noreadPattern.Text;
                try { config.NoRead_TCPClient_hostPort = Convert.ToInt32(textBox_test1_noreadTcpClientPort.Text); }
                catch { config.NoRead_TCPClient_hostPort = 0; }
                try { config.NoRead_TCPServer_hostPort = Convert.ToInt32(textBox_test1_noreadTcpServerPort.Text); }
                catch { config.NoRead_TCPServer_hostPort = 0; }
                try { config.NoRead_UDPServer_hostPort = Convert.ToInt32(textBox_test1_noreadUdpServerPort.Text); }
                catch { config.NoRead_UDPServer_hostPort = 0; }
                config.NoRead_SerialMain_comPort = comboBox_test1_noreadMainPort.Text;
                config.NoRead_SerialAUX_comPort = comboBox_test1_noreadAuxPort.Text;

                config.TestPartialRead = checkBox_test1_partialread.Checked;
                config.PartialRead_pattern = textBox_test1_partialreadPattern.Text;
                try { config.PartialRead_TCPClient_hostPort = Convert.ToInt32(textBox_test1_partialreadTcpClientPort.Text); }
                catch { config.PartialRead_TCPClient_hostPort = 0; }
                try { config.PartialRead_TCPServer_hostPort = Convert.ToInt32(textBox_test1_partialreadTcpServerPort.Text); }
                catch { config.PartialRead_TCPServer_hostPort = 0; }
                try { config.PartialRead_UDPServer_hostPort = Convert.ToInt32(textBox_test1_partialreadUdpServerPort.Text); }
                catch { config.PartialRead_UDPServer_hostPort = 0; }
                config.PartialRead_SerialMain_comPort = comboBox_test1_partialreadMainPort.Text;
                config.PartialRead_SerialAUX_comPort = comboBox_test1_partialreadAuxPort.Text;

                config.TestMultipleRead = checkBox_test1_multipleread.Checked;
                config.MultipleRead_pattern = textBox_test1_multiplereadPattern.Text;
                try { config.MultipleRead_TCPClient_hostPort = Convert.ToInt32(textBox_test1_multiplereadTcpClientPort.Text); }
                catch { config.MultipleRead_TCPClient_hostPort = 0; }
                try { config.MultipleRead_TCPServer_hostPort = Convert.ToInt32(textBox_test1_multiplereadTcpServerPort.Text); }
                catch { config.MultipleRead_TCPServer_hostPort = 0; }
                try { config.MultipleRead_UDPServer_hostPort = Convert.ToInt32(textBox_test1_multiplereadUdpServerPort.Text); }
                catch { config.MultipleRead_UDPServer_hostPort = 0; }
                config.MultipleRead_SerialMain_comPort = comboBox_test1_multiplereadMainPort.Text;
                config.MultipleRead_SerialAUX_comPort = comboBox_test1_multiplereadAuxPort.Text;

                config.TestPhaseAnalysis = checkBox_test1_phaseanalysis.Checked;
                config.PhaseAnalysis_pattern = textBox_test1_phaseanalysisPattern.Text;
                try { config.PhaseAnalysis_TCPClient_hostPort = Convert.ToInt32(textBox_test1_phaseanalysisTcpClientPort.Text); }
                catch { config.PhaseAnalysis_TCPClient_hostPort = 0; }
                config.PhaseAnalysis_SerialMain_comPort = comboBox_test1_phaseanalysisMainPort.Text;
                config.PhaseAnalysis_SerialAUX_comPort = comboBox_test1_phaseanalysisAuxPort.Text;

                test1class = new Test1Class(this, config, logConfig);
                test1class.Ended += OnTestEnded;
                test1class.WorkInProgress += OnWorkInProgress;
                test1class.CountersIncremented += OnCountersIncremented;

                testThread = new Thread(test1class.DoTest);
                testThread.Name = "TestThread";
                testThread.Start();

                richTextBox_testPhaseOutput.Clear();

                InitCounters();
            }
        }

        private void button_stopTest1_Click(object sender, EventArgs e)
        {
            timer_updateCounters.Enabled = false;
            List<Int32> missing = new List<Int32>();
            List<Int32> outoforder = new List<Int32>();
            if (test1class != null)
            {
                test1class.RequestStop();
                missing = test1class.MissingPhaseIndex;
                outoforder = test1class.OutOfOrderPhaseIndex;
            }
            manageComponentsForStopTest();

            if (missing != null)
            {
                for (int i = 0; i < missing.Count; i++)
                {
                    richTextBox_testPhaseOutput.Text += "missing " + Convert.ToString(missing[i]) + "\r\n";
                }
            }
            if (outoforder != null)
            {
                for (int i = 0; i < outoforder.Count; i++)
                {
                    richTextBox_testPhaseOutput.Text += "outoforder " + Convert.ToString(outoforder[i]) + "\r\n";
                }
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

            // messages listbox     ///////////////////////////////////////////////////////////
            listView_test1_MessagesReceived.Columns.Clear();
            listView_test1_MessagesReceived.Columns.Add("Channel", 72);
            listView_test1_MessagesReceived.Columns.Add("GoodRead", 74);
            listView_test1_MessagesReceived.Columns.Add("NoRead", 74);
            listView_test1_MessagesReceived.Columns.Add("PartialRead", 74);
            listView_test1_MessagesReceived.Columns.Add("MultipleRead", 74);
            listView_test1_MessagesReceived.Columns.Add("Total", 74);
            listView_test1_MessagesReceived.Columns.Add("OutOfOrder Phases", 74);
            listView_test1_MessagesReceived.Columns.Add("Missing Phases", 74);

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
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
                if (checkBox_test1_phaseanalysis.Checked)
                {
                    arr[(int)ListViewColumn.outoforderPhases] = test1class.outOfOrderPhaseCount[index].ToString();
                    arr[(int)ListViewColumn.missingPhases] = test1class.missingPhaseCount[index].ToString();
                }
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
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
                if (checkBox_test1_phaseanalysis.Checked)
                {
                    arr[(int)ListViewColumn.outoforderPhases] = test1class.outOfOrderPhaseCount[index].ToString();
                    arr[(int)ListViewColumn.missingPhases] = test1class.missingPhaseCount[index].ToString();
                }
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
                arr[(int)ListViewColumn.totalMessageCount] = test1class.totalMessageCount[index].ToString();
                if (checkBox_test1_phaseanalysis.Checked)
                {
                    arr[(int)ListViewColumn.outoforderPhases] = test1class.outOfOrderPhaseCount[index].ToString();
                    arr[(int)ListViewColumn.missingPhases] = test1class.missingPhaseCount[index].ToString();
                }
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
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.outoforderPhases].Text = test1class.outOfOrderPhaseCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.missingPhases].Text = test1class.missingPhaseCount[index].ToString();
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
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.outoforderPhases].Text = test1class.outOfOrderPhaseCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.missingPhases].Text = test1class.missingPhaseCount[index].ToString();
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
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.totalMessageCount].Text = test1class.totalMessageCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.outoforderPhases].Text = test1class.outOfOrderPhaseCount[index].ToString();
                listView_test1_MessagesReceived.Items[index].SubItems[(int)ListViewColumn.missingPhases].Text = test1class.missingPhaseCount[index].ToString();
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
            bool testEnded = CloseTest();
            if (testEnded)
                manageComponentsForStopTest();
        }

        private void testStarted()
        {
            manageComponentsForStartTest();
        }

        private bool CloseTest()
        {
            bool testsEnded = true;
            if (test1class != null)
            {
                test1class.Ended -= OnTestEnded;
                test1class.WorkInProgress -= OnWorkInProgress;
                test1class.CountersIncremented -= OnCountersIncremented;

                testThread.Join();
                test1class = null;
                testThread = null;
            }
            return testsEnded;
        }

        private void checkBox_test1_goodread_CheckedChanged(object sender, EventArgs e)
        {
            label_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadTcpClientPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadTcpServerPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadUdpServerPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadPattern.Visible = checkBox_test1_goodread.Checked;
            textBox_test1_goodreadPattern.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadMainPort.Visible = checkBox_test1_goodread.Checked;
            comboBox_test1_goodreadMainPort.Visible = checkBox_test1_goodread.Checked;
            label_test1_goodreadAuxPort.Visible = checkBox_test1_goodread.Checked;
            comboBox_test1_goodreadAuxPort.Visible = checkBox_test1_goodread.Checked;
            ipAddressControl_test1_myAddressGoodRead.Visible = checkBox_test1_goodread.Checked;
        }

        private void checkBox_test1_noread_CheckedChanged(object sender, EventArgs e)
        {
            label_test1_noreadTcpClientPort.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadTcpClientPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadTcpServerPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadUdpServerPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadPattern.Visible = checkBox_test1_noread.Checked;
            textBox_test1_noreadPattern.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadMainPort.Visible = checkBox_test1_noread.Checked;
            comboBox_test1_noreadMainPort.Visible = checkBox_test1_noread.Checked;
            label_test1_noreadAuxPort.Visible = checkBox_test1_noread.Checked;
            comboBox_test1_noreadAuxPort.Visible = checkBox_test1_noread.Checked;
            ipAddressControl_test1_myAddressNoRead.Visible = checkBox_test1_noread.Checked;
        }

        private void checkBox_test1_partialread_CheckedChanged(object sender, EventArgs e)
        {
            label_test1_partialreadTcpClientPort.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadTcpClientPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadTcpServerPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadUdpServerPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadPattern.Visible = checkBox_test1_partialread.Checked;
            textBox_test1_partialreadPattern.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadMainPort.Visible = checkBox_test1_partialread.Checked;
            comboBox_test1_partialreadMainPort.Visible = checkBox_test1_partialread.Checked;
            label_test1_partialreadAuxPort.Visible = checkBox_test1_partialread.Checked;
            comboBox_test1_partialreadAuxPort.Visible = checkBox_test1_partialread.Checked;
            ipAddressControl_test1_myAddressPartialRead.Visible = checkBox_test1_partialread.Checked;
        }

        private void checkBox_test1_multipleread_CheckedChanged(object sender, EventArgs e)
        {
            label_test1_multiplereadTcpClientPort.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadTcpClientPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadTcpServerPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadUdpServerPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
            textBox_test1_multiplereadPattern.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
            comboBox_test1_multiplereadMainPort.Visible = checkBox_test1_multipleread.Checked;
            label_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
            comboBox_test1_multiplereadAuxPort.Visible = checkBox_test1_multipleread.Checked;
            ipAddressControl_test1_myAddressMultipleRead.Visible = checkBox_test1_multipleread.Checked;
        }

        private void checkBox_test1_phaseanalysis_CheckedChanged(object sender, EventArgs e)
        {
            label_test1_phaseanalysisTcpClientPort.Visible = checkBox_test1_phaseanalysis.Checked;
            textBox_test1_phaseanalysisTcpClientPort.Visible = checkBox_test1_phaseanalysis.Checked;
            label_test1_phaseanalysisPattern.Visible = checkBox_test1_phaseanalysis.Checked;
            textBox_test1_phaseanalysisPattern.Visible = checkBox_test1_phaseanalysis.Checked;
            label_test1_phaseanalysisPatternComment.Visible = checkBox_test1_phaseanalysis.Checked;
            label_test1_phaseanalysisMainPort.Visible = checkBox_test1_phaseanalysis.Checked;
            comboBox_test1_phaseanalysisMainPort.Visible = checkBox_test1_phaseanalysis.Checked;
            label_test1_phaseanalysisAuxPort.Visible = checkBox_test1_phaseanalysis.Checked;
            comboBox_test1_phaseanalysisAuxPort.Visible = checkBox_test1_phaseanalysis.Checked;
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
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            initComponents();

            debug_fillData();
        }

        private void comboBox_test1_netAdapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            ipAddressControl_test1_myAddressGoodRead.Text = comboBox_test1_netAdapters.Text;
            ipAddressControl_test1_myAddressNoRead.Text = comboBox_test1_netAdapters.Text;
            ipAddressControl_test1_myAddressPartialRead.Text = comboBox_test1_netAdapters.Text;
            ipAddressControl_test1_myAddressMultipleRead.Text = comboBox_test1_netAdapters.Text;
        }

        private void timer_updateCounters_Tick(object sender, EventArgs e)
        {
            if (canUpdateCounters)
                UpdateCounters();
        }


    }
}

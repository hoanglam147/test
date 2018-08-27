using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace TestMatrix
{
    using Test1;

    public partial class Form_SerialSettingsDialog : Form
    {
        public Form_SerialSettingsDialog(List<SerialPortItem> serialports)
        {
            serialArray = new List<SerialPortItem>();
            foreach (SerialPortItem port in serialports)
            {
                serialArray.Add(port);
            }
            InitializeComponent();
            initVisualItems();
        }

        public List<SerialPortItem> serialArray;

        private void initVisualItems()
        {
            // fill serial lines combobox
            int index = -1;
            foreach (SerialPortItem port in serialArray)
            {
                index++;
                switch (index)
                {
                    case 0:
                        tabPage_1stSerialPort.Text = port.portName;

                        comboBox_1stBaud.Items.Add(300);
                        comboBox_1stBaud.Items.Add(600);
                        comboBox_1stBaud.Items.Add(1200);
                        comboBox_1stBaud.Items.Add(2400);
                        comboBox_1stBaud.Items.Add(9600);
                        comboBox_1stBaud.Items.Add(14400);
                        comboBox_1stBaud.Items.Add(19200);
                        comboBox_1stBaud.Items.Add(38400);
                        comboBox_1stBaud.Items.Add(57600);
                        comboBox_1stBaud.Items.Add(115200);
                        comboBox_1stBaud.Items.ToString();
                        comboBox_1stBaud.Text = port.baud.ToString();

                        comboBox_1stDataSize.Items.Add(7);
                        comboBox_1stDataSize.Items.Add(8);
                        comboBox_1stDataSize.Items.ToString();
                        comboBox_1stDataSize.Text = port.databits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Parity)))
                        {
                            comboBox_1stParity.Items.Add(s);
                        }
                        comboBox_1stParity.Text = port.parity.ToString();

                        foreach (string s in Enum.GetNames(typeof(StopBits)))
                        {
                            comboBox_1stStopBits.Items.Add(s);
                        }
                        comboBox_1stStopBits.Text = port.stopbits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Handshake)))
                        {
                            comboBox_1stHandshake.Items.Add(s);
                        }
                        comboBox_1stHandshake.Text = port.handshake.ToString();
                        break;

                    case 1:
                        tabPage_2ndSerialPort.Text = port.portName;

                        comboBox_2ndBaud.Items.Add(300);
                        comboBox_2ndBaud.Items.Add(600);
                        comboBox_2ndBaud.Items.Add(1200);
                        comboBox_2ndBaud.Items.Add(2400);
                        comboBox_2ndBaud.Items.Add(9600);
                        comboBox_2ndBaud.Items.Add(14400);
                        comboBox_2ndBaud.Items.Add(19200);
                        comboBox_2ndBaud.Items.Add(38400);
                        comboBox_2ndBaud.Items.Add(57600);
                        comboBox_2ndBaud.Items.Add(115200);
                        comboBox_2ndBaud.Items.ToString();
                        comboBox_2ndBaud.Text = port.baud.ToString();

                        comboBox_2ndDataSize.Items.Add(7);
                        comboBox_2ndDataSize.Items.Add(8);
                        comboBox_2ndDataSize.Items.ToString();
                        comboBox_2ndDataSize.Text = port.databits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Parity)))
                        {
                            comboBox_2ndParity.Items.Add(s);
                        }
                        comboBox_2ndParity.Text = port.parity.ToString();

                        foreach (string s in Enum.GetNames(typeof(StopBits)))
                        {
                            comboBox_2ndStopBits.Items.Add(s);
                        }
                        comboBox_2ndStopBits.Text = port.stopbits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Handshake)))
                        {
                            comboBox_2ndHandshake.Items.Add(s);
                        }
                        comboBox_2ndHandshake.Text = port.handshake.ToString();
                        break;

                    case 2:
                        tabPage_3rdSerialPort.Text = port.portName;

                        comboBox_3rdBaud.Items.Add(300);
                        comboBox_3rdBaud.Items.Add(600);
                        comboBox_3rdBaud.Items.Add(1200);
                        comboBox_3rdBaud.Items.Add(2400);
                        comboBox_3rdBaud.Items.Add(9600);
                        comboBox_3rdBaud.Items.Add(14400);
                        comboBox_3rdBaud.Items.Add(19200);
                        comboBox_3rdBaud.Items.Add(38400);
                        comboBox_3rdBaud.Items.Add(57600);
                        comboBox_3rdBaud.Items.Add(115200);
                        comboBox_3rdBaud.Items.ToString();
                        comboBox_3rdBaud.Text = port.baud.ToString();

                        comboBox_3rdDataSize.Items.Add(7);
                        comboBox_3rdDataSize.Items.Add(8);
                        comboBox_3rdDataSize.Items.ToString();
                        comboBox_3rdDataSize.Text = port.databits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Parity)))
                        {
                            comboBox_3rdParity.Items.Add(s);
                        }
                        comboBox_3rdParity.Text = port.parity.ToString();

                        foreach (string s in Enum.GetNames(typeof(StopBits)))
                        {
                            comboBox_3rdStopBits.Items.Add(s);
                        }
                        comboBox_3rdStopBits.Text = port.stopbits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Handshake)))
                        {
                            comboBox_3rdHandshake.Items.Add(s);
                        }
                        comboBox_3rdHandshake.Text = port.handshake.ToString();
                        break;

                    case 3:
                        tabPage_4thSerialPort.Text = port.portName;

                        comboBox_4thBaud.Items.Add(300);
                        comboBox_4thBaud.Items.Add(600);
                        comboBox_4thBaud.Items.Add(1200);
                        comboBox_4thBaud.Items.Add(2400);
                        comboBox_4thBaud.Items.Add(9600);
                        comboBox_4thBaud.Items.Add(14400);
                        comboBox_4thBaud.Items.Add(19200);
                        comboBox_4thBaud.Items.Add(38400);
                        comboBox_4thBaud.Items.Add(57600);
                        comboBox_4thBaud.Items.Add(115200);
                        comboBox_4thBaud.Items.ToString();
                        comboBox_4thBaud.Text = port.baud.ToString();

                        comboBox_4thDataSize.Items.Add(7);
                        comboBox_4thDataSize.Items.Add(8);
                        comboBox_4thDataSize.Items.ToString();
                        comboBox_4thDataSize.Text = port.databits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Parity)))
                        {
                            comboBox_4thParity.Items.Add(s);
                        }
                        comboBox_4thParity.Text = port.parity.ToString();

                        foreach (string s in Enum.GetNames(typeof(StopBits)))
                        {
                            comboBox_4thStopBits.Items.Add(s);
                        }
                        comboBox_4thStopBits.Text = port.stopbits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Handshake)))
                        {
                            comboBox_4thHandshake.Items.Add(s);
                        }
                        comboBox_4thHandshake.Text = port.handshake.ToString();
                        break;

                    case 4:
                        tabPage_5thSerialPort.Text = port.portName;

                        comboBox_5thBaud.Items.Add(300);
                        comboBox_5thBaud.Items.Add(600);
                        comboBox_5thBaud.Items.Add(1200);
                        comboBox_5thBaud.Items.Add(2400);
                        comboBox_5thBaud.Items.Add(9600);
                        comboBox_5thBaud.Items.Add(14400);
                        comboBox_5thBaud.Items.Add(19200);
                        comboBox_5thBaud.Items.Add(38400);
                        comboBox_5thBaud.Items.Add(57600);
                        comboBox_5thBaud.Items.Add(115200);
                        comboBox_5thBaud.Items.ToString();
                        comboBox_5thBaud.Text = port.baud.ToString();

                        comboBox_5thDataSize.Items.Add(7);
                        comboBox_5thDataSize.Items.Add(8);
                        comboBox_5thDataSize.Items.ToString();
                        comboBox_5thDataSize.Text = port.databits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Parity)))
                        {
                            comboBox_5thParity.Items.Add(s);
                        }
                        comboBox_5thParity.Text = port.parity.ToString();

                        foreach (string s in Enum.GetNames(typeof(StopBits)))
                        {
                            comboBox_5thStopBits.Items.Add(s);
                        }
                        comboBox_5thStopBits.Text = port.stopbits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Handshake)))
                        {
                            comboBox_5thHandshake.Items.Add(s);
                        }
                        comboBox_5thHandshake.Text = port.handshake.ToString();
                        break;

                    case 5:
                        tabPage_6thSerialPort.Text = port.portName;

                        comboBox_6thBaud.Items.Add(300);
                        comboBox_6thBaud.Items.Add(600);
                        comboBox_6thBaud.Items.Add(1200);
                        comboBox_6thBaud.Items.Add(2400);
                        comboBox_6thBaud.Items.Add(9600);
                        comboBox_6thBaud.Items.Add(14400);
                        comboBox_6thBaud.Items.Add(19200);
                        comboBox_6thBaud.Items.Add(38400);
                        comboBox_6thBaud.Items.Add(57600);
                        comboBox_6thBaud.Items.Add(115200);
                        comboBox_6thBaud.Items.ToString();
                        comboBox_6thBaud.Text = port.baud.ToString();

                        comboBox_6thDataSize.Items.Add(7);
                        comboBox_6thDataSize.Items.Add(8);
                        comboBox_6thDataSize.Items.ToString();
                        comboBox_6thDataSize.Text = port.databits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Parity)))
                        {
                            comboBox_6thParity.Items.Add(s);
                        }
                        comboBox_6thParity.Text = port.parity.ToString();

                        foreach (string s in Enum.GetNames(typeof(StopBits)))
                        {
                            comboBox_6thStopBits.Items.Add(s);
                        }
                        comboBox_6thStopBits.Text = port.stopbits.ToString();

                        foreach (string s in Enum.GetNames(typeof(Handshake)))
                        {
                            comboBox_6thHandshake.Items.Add(s);
                        }
                        comboBox_6thHandshake.Text = port.handshake.ToString();
                        break;
                }
            }

            for (++index; index < 6; index++)
            {
                switch (index)
                {
                    case 0:
                        tabControl_serialConfig.TabPages.Remove(tabPage_1stSerialPort);
                        break;
                    case 1:
                        tabControl_serialConfig.TabPages.Remove(tabPage_2ndSerialPort);
                        break;
                    case 2:
                        tabControl_serialConfig.TabPages.Remove(tabPage_3rdSerialPort);
                        break;
                    case 3:
                        tabControl_serialConfig.TabPages.Remove(tabPage_4thSerialPort);
                        break;
                    case 4:
                        tabControl_serialConfig.TabPages.Remove(tabPage_5thSerialPort);
                        break;
                    case 5:
                        tabControl_serialConfig.TabPages.Remove(tabPage_6thSerialPort);
                        break;
                }
            }
        
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

        private void addSerialPort(string name, int baud, Parity parity, int databits, StopBits stopbits, Handshake hanshake)
        {
            if (serialArray == null)
                serialArray = new List<SerialPortItem>();
            int index = serialArray.FindIndex(x => x.portName == name);
            if (index >= 0)
            {
                serialArray[index].portName = name;
                serialArray[index].baud = baud;
                serialArray[index].parity = parity;
                serialArray[index].databits = databits;
                serialArray[index].stopbits = stopbits;
                serialArray[index].handshake = hanshake;
            }
            else
            {
                SerialPortItem item = new SerialPortItem();
                item.portName = name;
                item.baud = baud;
                item.parity = parity;
                item.databits = databits;
                item.stopbits = stopbits;
                item.handshake = hanshake;
                serialArray.Add(item);
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            foreach (TabPage page in tabControl_serialConfig.TabPages)
            {
                if (page.Name == "tabPage_1stSerialPort")
                {
                    string portName = page.Text;
                    int baud = ToInt32(comboBox_1stBaud.Text);
                    Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_1stParity.Text);
                    int databits = ToInt32(comboBox_1stDataSize.Text);
                    StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_1stStopBits.Text);
                    Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_1stHandshake.Text);
                    addSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                }
                if (page.Name == "tabPage_2ndSerialPort")
                {
                    string portName = page.Text;
                    int baud = ToInt32(comboBox_2ndBaud.Text);
                    Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_2ndParity.Text);
                    int databits = ToInt32(comboBox_2ndDataSize.Text);
                    StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_2ndStopBits.Text);
                    Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_2ndHandshake.Text);
                    addSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                }
                if (page.Name == "tabPage_3rdSerialPort")
                {
                    string portName = page.Text;
                    int baud = ToInt32(comboBox_3rdBaud.Text);
                    Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_3rdParity.Text);
                    int databits = ToInt32(comboBox_3rdDataSize.Text);
                    StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_3rdStopBits.Text);
                    Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_3rdHandshake.Text);
                    addSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                }
                if (page.Name == "tabPage_4thSerialPort")
                {
                    string portName = page.Text;
                    int baud = ToInt32(comboBox_4thBaud.Text);
                    Parity parity = (Parity)Enum.Parse(typeof(Parity), comboBox_4thParity.Text);
                    int databits = ToInt32(comboBox_4thDataSize.Text);
                    StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), comboBox_4thStopBits.Text);
                    Handshake hanshake = (Handshake)Enum.Parse(typeof(Handshake), comboBox_4thHandshake.Text);
                    addSerialPort(portName, baud, parity, databits, stopbits, hanshake);
                }
            }

        }
    }
}

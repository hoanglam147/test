using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestMatrix
{
    using Test1;

    public partial class Form_LoggingDialog : Form
    {
        public LoggerConfiguration Config;

        public Form_LoggingDialog(LoggerConfiguration conf)
        {
            InitializeComponent();

            Config = conf;
            checkBox_enableLogging.Checked = Config.EnableLogging;
            textBox_logsFolder.Text = Config.LoggingFolder;
            checkBox_appendLogs.Checked = Config.AppendLogs;
            checkBox_splitLogs.Checked = Config.SplitLogFile;
            textBox_logFileMaxSize.Text = Convert.ToString(Config.LogFileMaxSize);
            checkBox_splitLogs.Visible = false;
            label_logFileMaxSize.Visible = false;
            textBox_logFileMaxSize.Visible = false;
            updateFormVisibilityRules();
        }

        private void updateFormVisibilityRules()
        {
            label_logsFolder.Visible = checkBox_enableLogging.Checked;
            textBox_logsFolder.Visible = checkBox_enableLogging.Checked;
            button_logsFolder.Visible = checkBox_enableLogging.Checked;
            checkBox_appendLogs.Visible = checkBox_enableLogging.Checked;
            checkBox_splitLogs.Visible = checkBox_enableLogging.Checked;
            label_logFileMaxSize.Visible = checkBox_enableLogging.Checked && checkBox_splitLogs.Checked;
            textBox_logFileMaxSize.Visible = checkBox_enableLogging.Checked && checkBox_splitLogs.Checked;
        }

        private void checkBox_enableLogging_CheckedChanged(object sender, EventArgs e)
        {
            updateFormVisibilityRules();
        }

        private void checkBox_splitLogs_CheckedChanged(object sender, EventArgs e)
        {
            updateFormVisibilityRules();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            Config.EnableLogging = checkBox_enableLogging.Checked;
            Config.LoggingFolder = textBox_logsFolder.Text;
            Config.AppendLogs = checkBox_appendLogs.Checked;
            Config.SplitLogFile = checkBox_splitLogs.Checked;
            try { Config.LogFileMaxSize = Convert.ToInt32(textBox_logFileMaxSize.Text); }
            catch { Config.LogFileMaxSize = 0; }
        }

        private void button_logsFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog_openFolder.SelectedPath = textBox_logsFolder.Text;
            DialogResult result = folderBrowserDialog_openFolder.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_logsFolder.Text = folderBrowserDialog_openFolder.SelectedPath;
            }
        }
    }
}

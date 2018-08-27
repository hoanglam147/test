namespace TestMatrix
{
    partial class Form_LoggingDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.checkBox_enableLogging = new System.Windows.Forms.CheckBox();
            this.textBox_logsFolder = new System.Windows.Forms.TextBox();
            this.button_logsFolder = new System.Windows.Forms.Button();
            this.label_logsFolder = new System.Windows.Forms.Label();
            this.checkBox_appendLogs = new System.Windows.Forms.CheckBox();
            this.label_logFileMaxSize = new System.Windows.Forms.Label();
            this.textBox_logFileMaxSize = new System.Windows.Forms.TextBox();
            this.checkBox_splitLogs = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog_openFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_ok.Location = new System.Drawing.Point(43, 229);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(67, 23);
            this.button_ok.TabIndex = 0;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(160, 230);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(72, 21);
            this.button_cancel.TabIndex = 1;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            // 
            // checkBox_enableLogging
            // 
            this.checkBox_enableLogging.AutoSize = true;
            this.checkBox_enableLogging.Location = new System.Drawing.Point(15, 22);
            this.checkBox_enableLogging.Name = "checkBox_enableLogging";
            this.checkBox_enableLogging.Size = new System.Drawing.Size(100, 17);
            this.checkBox_enableLogging.TabIndex = 2;
            this.checkBox_enableLogging.Text = "Enable Logging";
            this.checkBox_enableLogging.UseVisualStyleBackColor = true;
            this.checkBox_enableLogging.CheckedChanged += new System.EventHandler(this.checkBox_enableLogging_CheckedChanged);
            // 
            // textBox_logsFolder
            // 
            this.textBox_logsFolder.Location = new System.Drawing.Point(75, 61);
            this.textBox_logsFolder.Name = "textBox_logsFolder";
            this.textBox_logsFolder.Size = new System.Drawing.Size(164, 20);
            this.textBox_logsFolder.TabIndex = 3;
            // 
            // button_logsFolder
            // 
            this.button_logsFolder.Location = new System.Drawing.Point(245, 61);
            this.button_logsFolder.Name = "button_logsFolder";
            this.button_logsFolder.Size = new System.Drawing.Size(27, 20);
            this.button_logsFolder.TabIndex = 4;
            this.button_logsFolder.Text = "...";
            this.button_logsFolder.UseVisualStyleBackColor = true;
            this.button_logsFolder.Click += new System.EventHandler(this.button_logsFolder_Click);
            // 
            // label_logsFolder
            // 
            this.label_logsFolder.AutoSize = true;
            this.label_logsFolder.Location = new System.Drawing.Point(12, 64);
            this.label_logsFolder.Name = "label_logsFolder";
            this.label_logsFolder.Size = new System.Drawing.Size(59, 13);
            this.label_logsFolder.TabIndex = 5;
            this.label_logsFolder.Text = "Logs folder";
            // 
            // checkBox_appendLogs
            // 
            this.checkBox_appendLogs.AutoSize = true;
            this.checkBox_appendLogs.Location = new System.Drawing.Point(15, 98);
            this.checkBox_appendLogs.Name = "checkBox_appendLogs";
            this.checkBox_appendLogs.Size = new System.Drawing.Size(89, 17);
            this.checkBox_appendLogs.TabIndex = 6;
            this.checkBox_appendLogs.Text = "Append Logs";
            this.checkBox_appendLogs.UseVisualStyleBackColor = true;
            // 
            // label_logFileMaxSize
            // 
            this.label_logFileMaxSize.AutoSize = true;
            this.label_logFileMaxSize.Location = new System.Drawing.Point(12, 163);
            this.label_logFileMaxSize.Name = "label_logFileMaxSize";
            this.label_logFileMaxSize.Size = new System.Drawing.Size(109, 13);
            this.label_logFileMaxSize.TabIndex = 8;
            this.label_logFileMaxSize.Text = "Log file max size (MB)";
            // 
            // textBox_logFileMaxSize
            // 
            this.textBox_logFileMaxSize.Location = new System.Drawing.Point(127, 160);
            this.textBox_logFileMaxSize.Name = "textBox_logFileMaxSize";
            this.textBox_logFileMaxSize.Size = new System.Drawing.Size(112, 20);
            this.textBox_logFileMaxSize.TabIndex = 7;
            // 
            // checkBox_splitLogs
            // 
            this.checkBox_splitLogs.AutoSize = true;
            this.checkBox_splitLogs.Location = new System.Drawing.Point(15, 134);
            this.checkBox_splitLogs.Name = "checkBox_splitLogs";
            this.checkBox_splitLogs.Size = new System.Drawing.Size(86, 17);
            this.checkBox_splitLogs.TabIndex = 9;
            this.checkBox_splitLogs.Text = "Split Log File";
            this.checkBox_splitLogs.UseVisualStyleBackColor = true;
            this.checkBox_splitLogs.CheckedChanged += new System.EventHandler(this.checkBox_splitLogs_CheckedChanged);
            // 
            // Form_LoggingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.checkBox_splitLogs);
            this.Controls.Add(this.label_logFileMaxSize);
            this.Controls.Add(this.textBox_logFileMaxSize);
            this.Controls.Add(this.checkBox_appendLogs);
            this.Controls.Add(this.label_logsFolder);
            this.Controls.Add(this.button_logsFolder);
            this.Controls.Add(this.textBox_logsFolder);
            this.Controls.Add(this.checkBox_enableLogging);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form_LoggingDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Logging";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.CheckBox checkBox_enableLogging;
        private System.Windows.Forms.TextBox textBox_logsFolder;
        private System.Windows.Forms.Button button_logsFolder;
        private System.Windows.Forms.Label label_logsFolder;
        private System.Windows.Forms.CheckBox checkBox_appendLogs;
        private System.Windows.Forms.Label label_logFileMaxSize;
        private System.Windows.Forms.TextBox textBox_logFileMaxSize;
        private System.Windows.Forms.CheckBox checkBox_splitLogs;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog_openFolder;
    }
}
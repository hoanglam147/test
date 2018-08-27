using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestMatrix
{

    public class LoggerConfiguration
    {
        public bool EnableLogging = true;
        public string LoggingFolder = Environment.CurrentDirectory;
        public bool AppendLogs = false;
        public bool SplitLogFile = false;
        public Int32 LogFileMaxSize = 10;
        public string LogFileName = "TestMatrixLog.txt";
    }

    public class Logger
    {
        public Logger(LoggerConfiguration config)
        {
            enableLogging = config.EnableLogging;
            loggingFolder = config.LoggingFolder;
            appendLogs = config.AppendLogs;
            splitLogFile = config.SplitLogFile;
            logFileMaxSize = config.LogFileMaxSize;
            logFileName = config.LogFileName;
            currentSplitFileName = logFileName;
            splitFileNumber = 0;
            stream = new StreamWriter(logFileName, appendLogs);
        }

        public void Close()
        {
            stream.Close();
            stream.Dispose();
            stream = null;
        }

        private StreamWriter stream;
        private bool enableLogging;
        private string loggingFolder;
        private bool appendLogs;
        private bool splitLogFile;
        private Int32 logFileMaxSize;
        private string logFileName;
        private int splitFileNumber;
        private string currentSplitFileName;

        public void Log(string logstring)
        {
            Log(logstring, "");
        }

        public void Log(string logstring, string portToLog)
        {
            if (enableLogging == false)
                return;

            if (stream == null)
            {
                stream = new StreamWriter(logFileName, appendLogs);
            }
            string replace = String.Concat(logstring.Select(c => Char.IsControl(c) ?
                                                            String.Format("[{0:X2}]", (int)c) :
                                                            c.ToString()));
            stream.WriteLine(DateTime.Now.ToString("dd/MM HH:mm:ss.fff") + portToLog + replace);
            stream.Flush();

            if (splitLogFile)
            {
                // verify current log file size
                FileInfo f = new FileInfo(currentSplitFileName);
                long filelength = f.Length;
                if (filelength > (logFileMaxSize*1000000))
                {
                    splitFileNumber++;
                    currentSplitFileName = Path.GetFileNameWithoutExtension(logFileName);
                    currentSplitFileName += splitFileNumber.ToString();
                    currentSplitFileName += Path.GetExtension(logFileName);

                    stream.Close();
                    stream = new StreamWriter(currentSplitFileName, false);
                }
            }
        }
    }

}

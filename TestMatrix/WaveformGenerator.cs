using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Net;
using NationalInstruments.DAQmx;

namespace WaveformGenerator
{
    using ClientConnection;

    public enum WaveformType
    {
        DigitalIO,
        TCP,
        Serial
    }

    public enum WaveformState
    {
        Running,
        Paused,
        Stopped
    }

    public enum DigitalLineTransition
    {
        ToHigh,
        ToLow
    }

    public enum DigitalLineActiveState
    {
        ActiveHigh,
        ActiveLow
    }

    public class Digital_WaveformGenerator : IDisposable
    {
        public delegate void ToHighTransition();
        public delegate void ToLowTransition();

        public class WaveformEventArgs : EventArgs
        {
            public WaveformEventArgs()
            {
            }

            public WaveformEventArgs(WaveformState s)
            {
                state = s;
            }

            public WaveformEventArgs(DigitalLineTransition s)
            {
                transition = s;
            }

            private WaveformState state;
            private DigitalLineTransition transition;

            public WaveformState State
            {
                get { return state; }
                set { state = value; }
            }

            public DigitalLineTransition Transition
            {
                get { return transition; }
                set { transition = value; }
            }
        }

        public delegate void WaveformStateEventHandler(object sender, WaveformEventArgs a);
        public event WaveformStateEventHandler WaveformStateChange;

        public delegate void WaveformTransitionEventHandler(object sender, WaveformEventArgs a);
        public event WaveformTransitionEventHandler WaveformTransition;

        public class SignalState
        {
            public bool state;
            public Int32 durationMicroSec;
        }

        public bool IsRunning
        {
            get { return !this.stopped; }
        }

        public string DigitalLine
        {
            get { return this.digitalLine; }
            set { this.digitalLine = value; }
        }

        public double Frequency
        {
            get { return this.frequency; }
            set
            {
                this.frequency = value;
                Int32 periodMicroSec = Convert.ToInt32(1000000 / this.frequency);
                this.onDuration = Convert.ToInt32((double)periodMicroSec * (double)this.dutyCycle / 100.0);
                this.offDuration = periodMicroSec - this.onDuration;
                this.waveform[0].durationMicroSec = this.onDuration;
                this.waveform[1].durationMicroSec = this.offDuration;
            }
        }

        public Int32 DutyCycle
        {
            get { return this.dutyCycle; }
            set
            {
                this.dutyCycle = value;
                Int32 periodMicroSec = Convert.ToInt32(1000000 / this.frequency);
                this.onDuration = Convert.ToInt32((double)periodMicroSec * (double)this.dutyCycle / 100.0);
                this.offDuration = periodMicroSec - this.onDuration;
                this.waveform[0].durationMicroSec = this.onDuration;
                this.waveform[1].durationMicroSec = this.offDuration;
            }
        }

        public Int32 OnDuration
        {
            get { return this.onDuration; }
            set
            {
                this.onDuration = value;
                this.frequency = 1000000 / ((double)(this.onDuration + this.offDuration));
                this.dutyCycle = Convert.ToInt32(100 * (double)(this.onDuration) / ((double)this.onDuration + this.offDuration));
                this.waveform[0].durationMicroSec = this.onDuration;
            }
        }

        public Int32 OffDuration
        {
            get { return this.offDuration; }
            set
            {
                this.offDuration = value;
                this.frequency = 1000000 / ((double)(this.onDuration + this.offDuration));
                this.dutyCycle = Convert.ToInt32(100 * (double)(this.onDuration) / ((double)this.onDuration + this.offDuration));
                this.waveform[1].durationMicroSec = this.offDuration;
            }
        }

        public Int32 OnMinDuration
        {
            get { return this.onMinDuration; }
            set { this.onMinDuration = value; }
        }

        public Int32 OnMaxDuration
        {
            get { return this.onMaxDuration; }
            set { this.onMaxDuration = value; }
        }

        public Int32 OffMinDuration
        {
            get { return this.offMinDuration; }
            set { this.offMinDuration = value; }
        }

        public Int32 OffMaxDuration
        {
            get { return this.offMaxDuration; }
            set { this.offMaxDuration = value; }
        }

        public DigitalLineActiveState ActiveState
        {
            get { return this.activeState; }
            set { this.activeState = value; }
        }

        public string SignalOnMsg
        {
            get { return this.signalOnMsg; }
            set { this.signalOnMsg = value; }
        }

        public string SignalOffMsg
        {
            get { return this.signalOffMsg; }
            set { this.signalOffMsg = value; }
        }

        public bool SmoothStartStop
        {
            get { return this.smoothStartStop; }
            set { this.smoothStartStop = value; }
        }

        public Int32 StartStopTransitionTime
        {
            get { return this.startstopTransitionTime; }
            set { this.startstopTransitionTime = value; }
        }

        public Int32 ActiveTransitionCounter
        {
            get { return this.activeTransitionCounter; }
            set { this.activeTransitionCounter = value; }
        }

        public Int32 StartCounter
        {
            get { return this.startCounter; }
            set { this.startCounter = value; }
        }

        private WaveformType waveformType;
        private Int32 activeTransitionCounter = 0;
        private Int32 startCounter = 0;
        private bool periodicWaveform;
        private string digitalLine;
        private double frequency = 50;
        private Int32 dutyCycle = 50;
        private Int32 onDuration = 10;
        private Int32 offDuration = 10;
        private Int32 onMinDuration = 10;
        private Int32 onMaxDuration = 1000;
        private Int32 offMinDuration = 10;
        private Int32 offMaxDuration = 1000;
        private bool smoothStartStop = false;
        private Int32 startstopTransitionTime = 1000;
        private DigitalLineActiveState activeState = DigitalLineActiveState.ActiveHigh;
        private string signalOnMsg = "";
        private string signalOffMsg = "";

        private System.Timers.Timer smoothtransitionTimer;
        private bool smoothtransition_inprogress;
        private int smoothtransitionStep;
        private bool smoothstarting = false;
        private bool smoothstopping = false;
        private double microsec2ticks;
        private Stopwatch waitDurationStopwatch;
        private Stopwatch delaystopwatch;

        private SignalState[] waveform;
        private bool running = false;
        private bool stopped = true;
        private bool stopimmediate = false;

        private Task DAQTask = null;
        private DigitalSingleChannelWriter writer = null;
        private WaveformEventArgs transitionevent;
        private WaveformEventArgs stateevent;

        private ClientConnection tcpClientGenerator;
        private ClientConnection tcpClientPassthrough;
        private Thread tcpClientGeneratorThread;

        //private SerialPortClient serialClientGenerator;
        //private Thread serialClientGeneratorThread;

        Random rnd;

        private Int32 lastStateIdx = 0;

        private bool GetSignalLevel(bool state)
        {
            if (activeState == DigitalLineActiveState.ActiveLow)
                return !state;
            return state;
        }

        public Digital_WaveformGenerator(WaveformType type, IPAddress deviceAddress, string line, bool periodic)
        {
            waveformType = type;
            periodicWaveform = periodic;
            waveform = new SignalState[2];
            waveform[0] = new SignalState();
            waveform[0].state = true;
            waveform[0].durationMicroSec = onDuration;
            waveform[1] = new SignalState();
            waveform[1].state = false;
            waveform[1].durationMicroSec = offDuration;
            transitionevent = new WaveformEventArgs();
            stateevent = new WaveformEventArgs();
            lastStateIdx = 0;
            digitalLine = line;
            activeTransitionCounter = 0;
            startCounter = 0;
            if (waveformType == WaveformType.DigitalIO)
            {
                try
                {
                    DAQTask = new Task();
                    DOChannel outputChannel = DAQTask.DOChannels.CreateChannel(digitalLine, "waveform", ChannelLineGrouping.OneChannelForAllLines);
                    //outputChannel.OutputDriveType = DOOutputDriveType.ActiveDrive;
                    DAQTask.Start();
                    writer = new DigitalSingleChannelWriter(DAQTask.Stream);
                }
                catch (DaqException ex)
                {
                    MessageBox.Show(ex.Message);
                    if (DAQTask != null)
                    {
                        DAQTask.Dispose();
                        DAQTask = null;
                    }
                }
            }
            if (waveformType == WaveformType.TCP)
            {
                Int32 hostport;
                Int32.TryParse(line, out hostport);
                tcpClientGenerator = new ClientConnection(deviceAddress.ToString(), hostport);
                tcpClientGeneratorThread = new Thread(new ThreadStart(DoTCPConnection));


            }
            if (waveformType == WaveformType.Serial)
            {

            }
        }

        ~Digital_WaveformGenerator()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (waveformType == WaveformType.DigitalIO)
            {
                try
                {
                    if (DAQTask != null)
                    {
                        DAQTask.Stop();
                        DAQTask.Dispose();
                        DAQTask = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (waveformType == WaveformType.TCP)
            {
                if (tcpClientGeneratorThread != null)
                {
                    tcpClientGeneratorThread.Join();
                    tcpClientGeneratorThread = null;
                }
            }
        }

        public void StartWaveform()
        {
            StartGenerating();
        }

        public void StopWaveform(bool immediate = false)
        {
            StopGenerating(immediate);
        }

        public void PauseWaveform(bool immediate = false)
        {
            PauseGenerating(immediate);
        }
        
        public void ResumeWaveform()
        {
            ResumeGenerating();
        }

        private double GetSmoothStart_StepMultiplier(int step)
        {
            double multiplier;
            switch (step)
            {
                case 0:
                    multiplier = 12; break;
                case 1:
                    multiplier = 5; break;
                case 2:
                    multiplier = 2; break;
                case 3:
                    multiplier = 1.5; break;
                default:
                    multiplier = 1; break;
            }
            return multiplier;
        }

        private double GetSmoothStop_StepMultiplier(int step)
        {
            double multiplier;
            switch (step)
            {
                case 0:
                    multiplier = 1.5; break;
                case 1:
                    multiplier = 2; break;
                case 2:
                    multiplier = 5; break;
                case 3:
                default:
                    multiplier = 12; break;
//                 default:
//                     multiplier = 0; break;
            }
            return multiplier;
        }

        private void OnSmoothTransitionTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (smoothtransition_inprogress)
            {
                smoothtransitionStep++;
                double multiplier = 0;

                if (smoothstarting)
                    multiplier = GetSmoothStart_StepMultiplier(smoothtransitionStep);

                if (smoothstopping)
                    multiplier = GetSmoothStop_StepMultiplier(smoothtransitionStep);

                waveform[0].durationMicroSec = Convert.ToInt32(onDuration * multiplier);
                waveform[1].durationMicroSec = Convert.ToInt32(offDuration * multiplier); ;
            }
            if (smoothtransitionStep >= 4)
            {
                smoothtransition_inprogress = false;

                if (smoothstopping)
                {
                    smoothstopping = false;
                    endGeneration();
                    return;
                }

                smoothstarting = smoothstopping = false;
                waveform[0].durationMicroSec = onDuration;
                waveform[1].durationMicroSec = offDuration;
            }
            smoothtransitionTimer.Enabled = true;
        }

        private void DoTCPConnection()
        {
            if (tcpClientGenerator.Open() == true)
            {
                bool nextLevel = GetSignalLevel(waveform[lastStateIdx].state);

                while (running)
                {
                    if (nextLevel == true)
                    {
                        if (signalOnMsg != string.Empty)
                            tcpClientGenerator.Send(signalOnMsg);
                    }
                    else
                    {
                        if (signalOffMsg != string.Empty)
                            tcpClientGenerator.Send(signalOffMsg);
                    }

                    NOP(waveform[lastStateIdx].durationMicroSec);

                    if (stopimmediate || (!running && GetSignalLevel(waveform[lastStateIdx].state) == false))
                    {
                        stopped = true;
                        //tcpClientGeneratorThread.Abort();
                        //return;
                    }

                    nextLevel = prepareNextTransition();
                }
            }
            else
            {
                running = false;
                MessageBox.Show("Cannot open connection to host", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void StartGenerating()
        {
            if (!stopimmediate)
                lastStateIdx = 0;
            stopimmediate = false;

            running = true;

            waitDurationStopwatch = new Stopwatch();
            delaystopwatch = new Stopwatch();
            microsec2ticks = Convert.ToDouble(Stopwatch.Frequency) / 1000000.0;

            if (!periodicWaveform)
            {
                if (rnd == null)
                    rnd = new Random();

                if (waveform[lastStateIdx].state == true)
                    waveform[lastStateIdx].durationMicroSec = rnd.Next(onMinDuration, onMaxDuration);
                else
                    waveform[lastStateIdx].durationMicroSec = rnd.Next(offMinDuration, offMaxDuration);
            }
            else
            {
                //startstopTransitionTime
                if (smoothStartStop)
                {
                    smoothtransitionStep = 0;
                    smoothstarting = true;
                    double multiplier = GetSmoothStart_StepMultiplier(smoothtransitionStep);
                    waveform[0].durationMicroSec = Convert.ToInt32(onDuration * multiplier);
                    waveform[1].durationMicroSec = Convert.ToInt32(offDuration * multiplier);

                    smoothtransitionTimer = new System.Timers.Timer();
                    smoothtransitionTimer.Interval = startstopTransitionTime / 4;
                    smoothtransitionTimer.Elapsed += OnSmoothTransitionTimedEvent;
                    smoothtransitionTimer.AutoReset = false;
                    smoothtransitionTimer.Enabled = true;
                    smoothtransition_inprogress = true;
                }
                else
                {
                    waveform[0].durationMicroSec = onDuration;
                    waveform[1].durationMicroSec = offDuration;
                }
            }

            bool signalLevel = GetSignalLevel(waveform[lastStateIdx].state);

            if (signalLevel == true)
            {
                activeTransitionCounter++;
                transitionevent.Transition = DigitalLineTransition.ToHigh;
            }
            else
                transitionevent.Transition = DigitalLineTransition.ToLow;
            OnWaveformTransition(transitionevent);

            if (waveformType == WaveformType.DigitalIO)
            {
                if (DAQTask == null)
                    return;
                try
                {
                    writer.BeginWriteSingleSampleSingleLine(true, signalLevel, new AsyncCallback(OnDataWritten), DAQTask);
                }
                catch (DaqException ex)
                {
                    MessageBox.Show(ex.Message);
                    running = false;
                }
            }
            if (waveformType == WaveformType.TCP)
            {
                if (tcpClientGeneratorThread == null)
                    tcpClientGeneratorThread = new Thread(new ThreadStart(DoTCPConnection));

                if (tcpClientGeneratorThread.ThreadState == System.Threading.ThreadState.Unstarted)
                    tcpClientGeneratorThread.Start();
                running = true;
            }

            startCounter++;
            stopped = !running;

            if (running)
                stateevent.State = WaveformState.Running;
            else
                stateevent.State = WaveformState.Stopped;
            OnWaveformStateChange(stateevent);
        }

        private void PauseGenerating(bool immediate = false)
        {
            running = false;
            stopimmediate = immediate;
            if (smoothtransitionTimer != null)
            {
                smoothtransitionTimer.Stop();
            }
            stateevent.State = WaveformState.Stopped;
            OnWaveformStateChange(stateevent);
        }

        private void ResumeGenerating()
        {
            bool signalLevel = GetSignalLevel(waveform[lastStateIdx].state);

            if (!periodicWaveform)
            {
                if (rnd == null)
                    rnd = new Random();
                if (signalLevel == true)
                    waveform[lastStateIdx].durationMicroSec = rnd.Next(onMinDuration, onMaxDuration);
                else
                    waveform[lastStateIdx].durationMicroSec = rnd.Next(offMinDuration, offMaxDuration);
            }

            WaveformEventArgs transitionevent;
            if (signalLevel == true)
            {
                activeTransitionCounter++;
                transitionevent = new WaveformEventArgs(DigitalLineTransition.ToHigh);
            }
            else
                transitionevent = new WaveformEventArgs(DigitalLineTransition.ToLow);
            OnWaveformTransition(transitionevent);

            if (waveformType == WaveformType.DigitalIO)
            {
                try
                {
                    writer.BeginWriteSingleSampleSingleLine(true, signalLevel, new AsyncCallback(OnDataWritten), DAQTask);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (waveformType == WaveformType.TCP)
            {
                tcpClientGeneratorThread.Start();
                running = true;
            }
            startCounter++;
        }

        private void NOP(long durationMicroSec)
        {
            long durationTicks = Convert.ToInt64(durationMicroSec * microsec2ticks);
            waitDurationStopwatch.Restart();
            while (waitDurationStopwatch.ElapsedTicks < durationTicks)
            {
                if (stopimmediate)
                    return;
            }
        }

        private bool prepareNextTransition()
        {
            lastStateIdx = (++lastStateIdx) % waveform.Length;

            bool signalLevel = GetSignalLevel(waveform[lastStateIdx].state);

            if (!periodicWaveform)
            {
                if (rnd == null)
                    rnd = new Random();
                if (signalLevel == true)
                    waveform[lastStateIdx].durationMicroSec = rnd.Next(onMinDuration, onMaxDuration);
                else
                    waveform[lastStateIdx].durationMicroSec = rnd.Next(offMinDuration, offMaxDuration);
            }

            WaveformEventArgs transitionevent;
            if (signalLevel == true)
            {
                activeTransitionCounter++;
                transitionevent = new WaveformEventArgs(DigitalLineTransition.ToHigh);
            }
            else
                transitionevent = new WaveformEventArgs(DigitalLineTransition.ToLow);
            OnWaveformTransition(transitionevent);

            return signalLevel;
        }

        private void OnDataWritten(IAsyncResult result)
        {
            if (waveformType == WaveformType.DigitalIO)
            {
                if (DAQTask != null && DAQTask == result.AsyncState)
                {
                    writer.EndWrite(result);

                    long elapsed = delaystopwatch.ElapsedTicks;

                    if (stopimmediate || (!running && GetSignalLevel(waveform[lastStateIdx].state) == false))
                    {
                        stopped = true;
                        return;
                    }

                    long durationMicrosec = waveform[lastStateIdx].durationMicroSec - Convert.ToInt64(elapsed / microsec2ticks);
                    NOP(durationMicrosec);
                    delaystopwatch.Restart();

                    if (stopimmediate || (!running && GetSignalLevel(waveform[lastStateIdx].state) == false))
                    {
                        stopped = true;
                        return;
                    }

                    bool nextLevel = prepareNextTransition();

                    try
                    {
                        writer.BeginWriteSingleSampleSingleLine(true, nextLevel, new AsyncCallback(OnDataWritten), DAQTask);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void endGeneration()
        {
            running = false;
            if (smoothtransitionTimer != null)
            {
                smoothtransitionTimer.Stop();
                smoothtransitionTimer.Dispose();
                smoothtransitionTimer = null;
            }
            stateevent.State = WaveformState.Stopped;
            OnWaveformStateChange(stateevent);
        }

        private void StopGenerating(bool immediate = false)
        {
            stopimmediate = immediate;
            if (smoothStartStop)
            {
                smoothtransitionStep = 0;
                smoothstopping = true;
                double multiplier = GetSmoothStop_StepMultiplier(smoothtransitionStep);
                waveform[0].durationMicroSec = Convert.ToInt32(onDuration * multiplier);
                waveform[1].durationMicroSec = Convert.ToInt32(offDuration * multiplier);

                smoothtransitionTimer = new System.Timers.Timer();
                smoothtransitionTimer.Interval = startstopTransitionTime / 4;
                smoothtransitionTimer.Elapsed += OnSmoothTransitionTimedEvent;
                smoothtransitionTimer.AutoReset = false;
                smoothtransitionTimer.Enabled = true;
                smoothtransition_inprogress = true;
            }
            else
            {
                endGeneration();
                if (waveformType == WaveformType.TCP)
                {
                    if (tcpClientGeneratorThread != null)
                    {
                        tcpClientGeneratorThread.Join();
                        tcpClientGeneratorThread = null;
                    }
                }
            }
        }

        protected virtual void OnWaveformStateChange(WaveformEventArgs e)
        {
            WaveformStateChange?.Invoke(this, e);
        }

        protected virtual void OnWaveformTransition(WaveformEventArgs e)
        {
            WaveformTransition?.Invoke(this, e);
        }

    }
}

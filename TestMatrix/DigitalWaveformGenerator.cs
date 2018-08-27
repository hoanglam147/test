using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NationalInstruments.DAQmx;

namespace WaveformGenerator
{
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

            public WaveformEventArgs(DigitalLineTransition s)
            {
                transition = s;
            }

            private DigitalLineTransition transition;

            public DigitalLineTransition Transition
            {
                get { return transition; }
                set { transition = value; }
            }
        }

        public delegate void WaveformTransitionEventHandler(object sender, WaveformEventArgs a);

        public event WaveformTransitionEventHandler WaveformTransition;

        public class WaveformState
        {
            public bool state;
            public Int32 durationMilliSec;
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
                Int32 periodMSec = Convert.ToInt32(1000 / this.frequency);
                this.onDuration = Convert.ToInt32((double)periodMSec * (double)this.dutyCycle / 100.0);
                this.offDuration = periodMSec - this.onDuration;
                this.waveform[0].durationMilliSec = this.onDuration;
                this.waveform[1].durationMilliSec = this.offDuration;
            }
        }

        public Int32 DutyCycle
        {
            get { return this.dutyCycle; }
            set
            {
                this.dutyCycle = value;
                Int32 periodMSec = Convert.ToInt32(1000 / this.frequency);
                this.onDuration = Convert.ToInt32((double)periodMSec * (double)this.dutyCycle / 100.0);
                this.offDuration = periodMSec - this.onDuration;
                this.waveform[0].durationMilliSec = this.onDuration;
                this.waveform[1].durationMilliSec = this.offDuration;
            }
        }

        public Int32 OnDuration
        {
            get { return this.onDuration; }
            set
            {
                this.onDuration = value;
                this.frequency = 1000 / ((double)(this.onDuration + this.offDuration));
                this.dutyCycle = Convert.ToInt32(100 * (double)(this.onDuration) / ((double)this.onDuration + this.offDuration));
                this.waveform[0].durationMilliSec = this.onDuration;
            }
        }

        public Int32 OffDuration
        {
            get { return this.offDuration; }
            set
            {
                this.offDuration = value;
                this.frequency = 1000 / ((double)(this.onDuration + this.offDuration));
                this.dutyCycle = Convert.ToInt32(100 * (double)(this.onDuration) / ((double)this.onDuration + this.offDuration));
                this.waveform[1].durationMilliSec = this.offDuration;
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
        private DigitalLineActiveState activeState = DigitalLineActiveState.ActiveHigh;

        private WaveformState[] waveform;
        private bool running = false;
        private bool stopped = true;

        private Task DAQTask = null;
        private DigitalSingleChannelWriter writer = null;
        private WaveformEventArgs transitionevent;

        Random rnd;

        private Int32 lastStateIdx = 0;

        private bool GetSignalLevel(bool state)
        {
            if (activeState == DigitalLineActiveState.ActiveLow)
                return !state;
            return state;
        }

        public DigitalWaveformGenerator(string line, bool periodic)
        {
            periodicWaveform = periodic;
            waveform = new WaveformState[2];
            waveform[0] = new WaveformState();
            waveform[0].state = true;
            waveform[0].durationMilliSec = onDuration;
            waveform[1] = new WaveformState();
            waveform[1].state = false;
            waveform[1].durationMilliSec = offDuration;
            transitionevent = new WaveformEventArgs();
            lastStateIdx = 0;
            digitalLine = line;
            try
            {
                DAQTask = new Task();
                DOChannel outputChannel = DAQTask.DOChannels.CreateChannel(digitalLine, "waveform", ChannelLineGrouping.OneChannelForAllLines);
                outputChannel.OutputDriveType = DOOutputDriveType.ActiveDrive;
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

        ~DigitalWaveformGenerator()
        {
            Dispose();
        }

        public void Dispose()
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

        public bool StartWaveform()
        {
            if (DAQTask == null)
                return false;

            lastStateIdx = 0;
            running = true;

            if (!periodicWaveform)
            {
                if (rnd == null)
                    rnd = new Random();

                if (waveform[lastStateIdx].state == true)
                    waveform[lastStateIdx].durationMilliSec = rnd.Next(onMinDuration, onMaxDuration);
                else
                    waveform[lastStateIdx].durationMilliSec = rnd.Next(offMinDuration, offMaxDuration);
            }

            bool signalLevel = GetSignalLevel(waveform[lastStateIdx].state);

            if (signalLevel == true)
                transitionevent.Transition = DigitalLineTransition.ToHigh;
            else
                transitionevent.Transition = DigitalLineTransition.ToLow;
            OnWaveformTransition(transitionevent);

            try
            {
                writer.BeginWriteSingleSampleSingleLine(true, signalLevel, new AsyncCallback(OnDataWritten), DAQTask);
            }
            catch (DaqException ex)
            {
                MessageBox.Show(ex.Message);
                running = false;
            }
            stopped = !running;
            return running;
        }

        private void OnDataWritten(IAsyncResult result)
        {
            if (DAQTask != null && DAQTask == result.AsyncState)
            {
                writer.EndWrite(result);

                if (!running && GetSignalLevel(waveform[lastStateIdx].state) == false)
                {
                    stopped = true;
                    return;
                }

                System.Threading.Thread.Sleep(waveform[lastStateIdx].durationMilliSec);

                //if (running || GetSignalLevel(waveform[lastStateIdx].state) == true)
                {
                    lastStateIdx = (++lastStateIdx) % waveform.Length;

                    bool signalLevel = GetSignalLevel(waveform[lastStateIdx].state);

                    if (!periodicWaveform)
                    {
                        if (rnd == null)
                            rnd = new Random();
                        if (signalLevel == true)
                            waveform[lastStateIdx].durationMilliSec = rnd.Next(onMinDuration, onMaxDuration);
                        else
                            waveform[lastStateIdx].durationMilliSec = rnd.Next(offMinDuration, offMaxDuration);
                    }

                    WaveformEventArgs transitionevent;
                    if (signalLevel == true)
                        transitionevent = new WaveformEventArgs(DigitalLineTransition.ToHigh);
                    else
                        transitionevent = new WaveformEventArgs(DigitalLineTransition.ToLow);
                    OnWaveformTransition(transitionevent);

                    try
                    {
                        writer.BeginWriteSingleSampleSingleLine(true, signalLevel, new AsyncCallback(OnDataWritten), DAQTask);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                //                 else
                //                 {
                //                     if (writer != null)
                //                         writer.EndWrite(result);
                //                 }
            }
        }

        public void StopWaveform()
        {
            running = false;
        }

        protected virtual void OnWaveformTransition(WaveformEventArgs e)
        {
            if (WaveformTransition != null)
            {
                WaveformTransition(this, e);
            }
        }

    }
}

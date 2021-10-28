using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using Microsoft.Win32;

namespace AmbientLightingLib
{
    public class AmbientLightEngine
    {
        private SerialPort _port;
        private byte[] _serialData;
        private bool _run;
        private bool _stop;
        private bool _desktopLocked;

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets the active RGB helper.
        /// </summary>
        /// <value>
        /// The active RGB helper.
        /// </value>
        public IRgbProvider ActiveRgbHelper { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize(IRgbProvider initialRgbProvider)
        {
            ActiveRgbHelper = initialRgbProvider;

            ActiveRgbHelper.Initialize();

            var portNames = SerialPort.GetPortNames();
            _port = new SerialPort(portNames[0]) { BaudRate = 9600 };

            _serialData = new byte[4];

            Microsoft.Win32.SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
        }

        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock || e.Reason == SessionSwitchReason.SessionLogoff)
            {
                _desktopLocked = true;
                Logger.LogInformation("Desktop locked");
                Stop();
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock || e.Reason == SessionSwitchReason.SessionLogon)
            {
                _desktopLocked = false;
                Logger.LogInformation("Desktop unlocked");
                Start();
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (IsRunning) return;

            _stop = false;
            _run = true;
            _process();
            Logger.LogInformation("Turned on");
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _stop = true;
            Logger.LogInformation("Turned off");
        }

        private void _process()
        {
            var ts = new Thread(() =>
            {
                _port.Open();

                while (_run && !_stop && !_desktopLocked)
                {
                    IsRunning = true;

                    CalculateRgbValueBegin?.Invoke(this, EventArgs.Empty);
                    Color avgScreenColor;

                    try
                    {
                        avgScreenColor = ActiveRgbHelper.GetRgbValues();
                    }
                    catch (Win32Exception win32Exception)
                    {
                        // this can be thrown because of user account control display
                        Logger.LogException(win32Exception);
                        continue;
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e);
                        break;
                    }

                    CalculateRgbValueEnd?.Invoke(this, new ScreenColorEventArgs
                    {
                        Blue = avgScreenColor.B,
                        Green = avgScreenColor.G,
                        Red = avgScreenColor.R
                    });

                    _writeToPort(avgScreenColor.R, avgScreenColor.G, avgScreenColor.B);
                }

                _writeToPort(0, 0, 0);
                _port.Dispose();
                IsRunning = false;
            });
            ts.Start();
        }

        private void _writeToPort(byte red, byte green, byte blue)
        {
            _serialData[0] = 255;
            _serialData[1] = red;
            _serialData[2] = green;
            _serialData[3] = blue;
            _port.Write(_serialData, 0, _serialData.Length);
        }

        
        public event EventHandler CalculateRgbValueBegin;

        
        public event EventHandler<ScreenColorEventArgs> CalculateRgbValueEnd;
    }
}

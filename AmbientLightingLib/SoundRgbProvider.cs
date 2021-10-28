using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace AmbientLightingLib
{
    public class SoundRgbProvider : IRgbProvider
    {
        /// <summary>
        /// Gets the sound devices.
        /// </summary>
        /// <value>
        /// The sound devices.
        /// </value>
        public ReadOnlyDictionary<int, string> SoundDevices { get; private set; }

        /// <summary>
        /// Gets or sets the selected sound device.
        /// </summary>
        /// <value>
        /// The selected sound device.
        /// </value>
        public int SelectedSoundDevice { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            var availableSoundDevices = new Dictionary<int, string>();
            for (var deviceIndex = 0; deviceIndex < BassWasapi.BASS_WASAPI_GetDeviceCount(); deviceIndex++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(deviceIndex);
                if (device.IsEnabled && device.IsLoopback)
                {
                    availableSoundDevices.Add(deviceIndex, device.name);
                }
            }
            SoundDevices = new ReadOnlyDictionary<int, string>(availableSoundDevices);

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            // default
            SelectedSoundDevice = SoundDevices.ElementAt(1).Key;
        }

        public Color GetRgbValues()
        {
           BassWasapi.BASS_WASAPI_Init(SelectedSoundDevice, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f,
                0.05f, _process, IntPtr.Zero);
            BassWasapi.BASS_WASAPI_Start();

            var _fft = new float[1024];
            var ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int) BASSData.BASS_DATA_FFT2048);
            var _spectrumdata = new List<int>();

            int _lines = 16, x, y, b0 = 0;            // number of spectrum lines

            for (x = 0; x < _lines; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (y > 255) y = 255;
                if (y < 0) y = 0;
                _spectrumdata.Add(y);
            }

            var r = _spectrumdata.Take(5).Sum() / 5;
            var g = _spectrumdata.Skip(5).Take(5).Sum() / 5;
            var b = _spectrumdata.Skip(10).Take(5).Sum() / 5;

            r = Math.Min(r, 255);
            g = Math.Min(g, 255);
            b = Math.Min(b, 255);

            // sleep
            Thread.Sleep(100);

            return Color.FromArgb(r, g, b);
        }

        private int _process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }
    }
}
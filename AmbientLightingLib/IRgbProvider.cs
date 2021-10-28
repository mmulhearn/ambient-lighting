using System;
using System.Drawing;

namespace AmbientLightingLib
{
    public interface IRgbProvider : IDisposable
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the RGB values.
        /// </summary>
        /// <returns></returns>
        Color GetRgbValues();
    }
}
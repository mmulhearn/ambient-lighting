using System;
using System.Drawing;
using System.Windows.Forms;

namespace AmbientLightingLib
{
    public class ScreenRgbProvider : IRgbProvider
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// Gets the RGB values.
        /// </summary>
        /// <returns></returns>
        public Color GetRgbValues()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                float red = 0,
                    green = 0,
                    blue = 0;

                // to ignore window headers and such, just get the middle 1/2 of the screen
                int xMin = bounds.Width / 4;
                int xMax = bounds.Width / 4 * 3;
                int yMin = bounds.Height / 4;
                int yMax = bounds.Height / 4 * 3;

                for (var x = xMin; x <= xMax; x = x + 3)
                {
                    for (var y = yMin; y <= yMax; y = y + 3)
                    {
                        var pixel = bmp.GetPixel(x, y);
                        red += pixel.R;
                        blue += pixel.B;
                        green += pixel.G;
                    }
                }

                // average the colors out
                var screenAreaHalved = bounds.Width / 6 * bounds.Height / 6;
                red = Math.Min(red / screenAreaHalved, 255);
                blue = Math.Min(blue / screenAreaHalved, 255);
                green = Math.Min(green / screenAreaHalved, 255);

                return Color.FromArgb((int)red, (int)green, (int)blue);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

        }
    }
}

using System;
using System.Drawing;

namespace Watermark
{
    /// <summary>
    /// Static Steganography class which can be used to hide and extract watermarks from the same size images
    /// </summary>
    public static class Steganography
    {
        /// <summary>
        /// Method, that hides given watermark within the given image of the same size
        /// </summary>
        /// <param name="original">Image where to hide the watermark</param>
        /// <param name="watermark">Watermark to hide</param>
        /// <param name="shift">Number (1-7) by how many bits to shift. Default - 4</param>
        /// <returns>Watermarked original image</returns>
        public static Bitmap Watermark(Bitmap original, Bitmap watermark, int shift = 4)
        {
            if (original.Size != watermark.Size)
                throw new ArgumentException("Watermark must be the same size as the original image!");
            if (shift < 1 || shift > 7)
                throw new ArgumentException("Shift amount must be between 1 and 7");

            Bitmap result = new Bitmap(original.Width, original.Height);

            for(int i = 0; i < original.Width; i++)
            {
                for(int j = 0; j < original.Height; j++)
                {
                    Color newColor = MergePixels(original.GetPixel(i, j), watermark.GetPixel(i, j), shift);
                    result.SetPixel(i,j,newColor);
                }
            }
            return result;
        }

        /// <summary>
        /// Method, that adds MSBs of the watermark color in the LSBs position of the original color
        /// </summary>
        /// <param name="a">Original color</param>
        /// <param name="b">Watermark color</param>
        /// <param name="shift">Shift amount</param>
        /// <returns>Merged color</returns>
        static Color MergePixels(Color a, Color b, int shift)
        {
            // Merge reds
            byte redA = a.R;                        // **** ****  
            redA = Shift(redA, shift);              // 0000 **** >> moves all bits to the right by n, 0's appear in the empty spots
            redA = Shift(redA, shift, false);       // **** 0000 >> moves all bits to the left by n. After these two operations, n LSBs of the original color are cleared
            byte redB = b.R;                        // •••• ••••
            redB = Shift(redB, 8 - shift);          // 0000 •••• >> moves all bits to the right by m, so the MSBs are in the LSBs position
            redA += redB;                           // **** •••• >> adds both colors together

            // Merge greens
            byte greenA = a.G;
            greenA = Shift(greenA, shift);
            greenA = Shift(greenA, shift, false);
            byte greenB = b.G;
            greenB = Shift(greenB, 8 - shift);
            greenA += greenB;

            // Merge blues
            byte blueA = a.B;
            blueA = Shift(blueA, shift);
            blueA = Shift(blueA, shift, false);
            byte blueB = b.B;
            blueB = Shift(blueB,  8- shift);
            blueA += blueB;

            return Color.FromArgb(redA, greenA, blueA);
        }

        /// <summary>
        /// Method, used to extract a watermark from an image
        /// </summary>
        /// <param name="original">Watermarked image</param>
        /// <param name="shift">Shift amount</param>
        /// <returns>Watermark image</returns>
        public static Bitmap ExtractWatermark(ref Bitmap original, int shift = 4)
        {
            if (shift < 1 || shift > 7)
                throw new ArgumentException("Shift amount must be between 1 and 7");
            Bitmap watermark = new Bitmap(original.Width, original.Height);

            for(int i = 0; i < original.Width; i++)
            {
                for(int j = 0; j < original.Height; j++)
                {
                    Color pixel = original.GetPixel(i, j);
                    Color watermarkPixel = ExtractColor(ref pixel, shift);
                    watermark.SetPixel(i, j, watermarkPixel);
                    original.SetPixel(i, j, pixel);
                }
            }
            return watermark;
        }

        /// <summary>
        /// Method, that removes the watermark color
        /// </summary>
        /// <param name="original">Watermarked color</param>
        /// <param name="shift">Shift amount</param>
        /// <returns>Watermark's color</returns>
        static Color ExtractColor(ref Color original, int shift)
        {
            // Extract reds
            byte redA, redB;                            // Making two copies of the red color
            redA = redB = original.R;
            redA = Shift(redA, shift);                  // Shifting the first copy to the right to remove n bits. 0's are added to the front
            redA = Shift(redA, shift, false);           // Shifting the first copy to the left. 0's are added to the back. This made redA only contain bits from the first image by removing n LSBs.
            redB = Shift(redB, 8 - shift, false);       // Shifting the second copy by m bitsto the left. 0s are added to the back.

            // Extract greens
            byte greenA, greenB;
            greenA = greenB = original.G;
            greenA = Shift(greenA, shift);
            greenA = Shift(greenA, shift, false);
            greenB = Shift(greenB, 8 - shift, false);

            // Extract blues
            byte blueA, blueB;
            blueA = blueB = original.B;
            blueA = Shift(blueA, shift);
            blueA = Shift(blueA, shift, false);
            blueB = Shift(blueB, 8 - shift, false);
            original = Color.FromArgb(redA, greenA, blueA);

            return Color.FromArgb(redB, greenB, blueB);
        }

        /// <summary>
        /// Method to shift bits
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="amount">Amount of how many bits to shift</param>
        /// <param name="toRight">Direction</param>
        /// <returns></returns>
        static byte Shift(byte color, int amount, bool toRight = true)
        {
            return (toRight) ? (byte)(color >> amount) : (byte)(color << amount);
        }
    }
}

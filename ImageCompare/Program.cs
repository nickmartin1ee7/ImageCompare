using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using static System.Console;

namespace ImageCompare
{
    class Program
    {
        private static Bitmap img1;
        private static Bitmap img2;
        static void Main(string[] args)
        {
            InitImgs(args);
            var diffPercent = CompareImgsRGB();
            //var diffPercent = CompareImgsQuick();
            DisposeImgs();

            string level;
            const double medLevel = .03;
            const double highLevel = .10;

            if (diffPercent >= highLevel)
            {
                level = "HIGH";
            }
            else if (diffPercent >= medLevel)
            {
                level = "MODERATE";
            }
            else if (diffPercent == 0)
            {
                level = "IDENTICAL";
            }
            else
            {
                level = "LOW";
            }
            WriteLine($"Difference: {diffPercent:P} - {level} pixel difference");
            ReadKey();
        }


        private static void InitImgs(string[] args)
        {
            bool valid = false;
            do
            {
                try
                {
                    if (args.Length == 2)
                    {
                        InitImgsArg(args);
                    }
                    else
                    {
                        InitImgsUser();
                    }
                    valid = true;
                }
                catch (Exception e)
                {
                    WriteLine($"Error: Could not import image! {e.Message}");
                    DisposeImgs();
                }
            } while (!valid);

        }

        private static void InitImgsArg(string[] args)
        {
            img1 = new Bitmap(args[0]);
            img2 = new Bitmap(args[1]);
        }

        private static void InitImgsUser()
        {
            Write("Image 1 path: ");
            img1 = new Bitmap(ReadLine());
            Write("Image 2 path: ");
            img2 = new Bitmap(ReadLine());
        }

        // Calculates difference between two images RGB pixel values.
        private static double CompareImgsRGB()
        {
            if (img1.Size != img2.Size)
            {
                throw new NotImplementedException("Error: Image sizes are different. Must be the same for this implementation.");
            }
            else
            {
                int maxPixelRange = 255;    // Pixel color range 0-255
                int width = img1.Width;     // Width of both images
                int height = img1.Height;   // Height of both images
                double diff = 0;            // Difference double-percision value

                // Consider threading for different images
                for (int y = 0; y < height; ++y)   // Y-axis
                {
                    for (int x = 0; x < width; ++x)    // X-axis
                    {
                        Color pixel1 = img1.GetPixel(x, y);
                        Color pixel2 = img2.GetPixel(x, y);

                        diff += Math.Abs(pixel1.R - pixel2.R);
                        diff += Math.Abs(pixel1.G - pixel2.G);
                        diff += Math.Abs(pixel1.B - pixel2.B);
                    }
                }
                return (diff / maxPixelRange) / (width * height * 3);   // Average pixel difference for image pixels (accounts for 3 layers)
            }
        }

        [Obsolete("Output produced is not understood at this time. Futher evaluation of lambda expression needed.\nCredit (@fubo): https://stackoverflow.com/questions/35151067/algorithm-to-compare-two-images-in-c-sharp")]
        private static int CompareImgsQuick()
        {
            List<bool> GetHash(Bitmap bmpSource)
            {
                List<bool> r = new List<bool>();
                //create new image with 16x16 pixel
                Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
                for (int y = 0; y < bmpMin.Height; y++)
                {
                    for (int x = 0; x < bmpMin.Width; x++)
                    {
                        //reduce colors to true / false                
                        r.Add(bmpMin.GetPixel(x, y).GetBrightness() < 0.5f);
                    }
                }
                return r;
            }
            List<bool> h1 = GetHash(img1);
            List<bool> h2 = GetHash(img2);
            int e = h1.Zip(h2, (x, y) => x == y).Count(eq => eq);
            return e;

        }

        private static void DisposeImgs()
        {
            if (img1 != null) { img1.Dispose(); }
            if (img2 != null) { img2.Dispose(); }
        }

    }
}

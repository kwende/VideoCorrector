using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.CV
{
    public static class PhotoshopSDToHD
    {
        public static Bitmap DoIt(Bitmap bmp)
        {
            GaussianBlur blur = new GaussianBlur(10, 50);
            Bitmap blurred = blur.Apply(bmp);

            Bitmap ret = new Bitmap(blurred.Width, blurred.Height, blurred.PixelFormat);

            for (int y = 0; y < blurred.Height; y++)
            {
                for (int x = 0; x < blurred.Width; x++)
                {
                    Color blurredColor = blurred.GetPixel(x, y);
                    Color orinalColor = bmp.GetPixel(x, y);

                    Color newColor = Color.FromArgb((blurredColor.R + orinalColor.R * 2) / 3,
                        (blurredColor.G + orinalColor.G * 2) / 3,
                        (blurredColor.B + orinalColor.B * 2) / 3);

                    ret.SetPixel(x, y, newColor);
                }
            }

            GammaCorrection gc = new GammaCorrection(.8);
            gc.ApplyInPlace(ret);

            Sharpen sharpen = new Sharpen();
            sharpen.ApplyInPlace(ret);

            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap gray = filter.Apply(ret);

            CannyEdgeDetector canny = new CannyEdgeDetector();
            gray = canny.Apply(gray);

            for (int y = 0; y < gray.Height; y++)
            {
                for (int x=0;x < gray.Width; x++)
                {
                    if(gray.GetPixel(x,y).R > 0)
                    {
                        Color retColor = ret.GetPixel(x, y);
                        Color newColor = Color.FromArgb(
                            (int)(retColor.R * .7), 
                            (int)(retColor.G * .7), 
                            (int)(retColor.B * .7));

                        ret.SetPixel(x, y, newColor); 
                    }
                }
            }

            return ret;
        }
    }
}

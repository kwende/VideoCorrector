using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.LearningAlgorithms.Specific.GrayscaleCorrectorTree
{
    [Serializable]
    public class GrayPixel
    {
        public Point Coordinate { get; set; }
        public int OriginalIntensity { get; set; }
        public byte[] ImageData { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
    }
}

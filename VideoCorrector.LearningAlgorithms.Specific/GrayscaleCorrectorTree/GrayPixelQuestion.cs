using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.LearningAlgorithms.Specific.GrayscaleCorrectorTree
{
    [Serializable]
    public class GrayPixelQuestion : GenericDecisionTreeQuestion
    {
        public PointF OffsetVector { get; set; }
    }
}

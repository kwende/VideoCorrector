using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.LearningAlgorithms.Specific.GrayscaleCorrectorTree
{
    [Serializable]
    public class GrayscaleDecisionTreeParameters : DecisionTreeBuilderParameters
    {
        public int NumberOfQuestions { get; set; }
        public int MaximumOffset { get; set; }
        public int MinimumOffset { get; set; }
    }
}

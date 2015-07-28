using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCorrector.LearningAlgorithms
{
    [Serializable]
    public class DecisionTreeBuilderParameters
    {
        public int MaxRecursionLevels { get; set; }
        public double SufficientGainLevel { get; set; }
    }
}

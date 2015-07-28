using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCorrector.LearningAlgorithms
{
    [Serializable]
    public class GenericTrainedDecisionTree
    {
        public DecisionTreeBuilderParameters Parameters { get; set; }
        public GenericTrainedDecisionTreeNode Root { get; set; }
    }
}

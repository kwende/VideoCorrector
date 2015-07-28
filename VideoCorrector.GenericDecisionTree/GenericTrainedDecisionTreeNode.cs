using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCorrector.LearningAlgorithms
{
    [Serializable]
    public class GenericTrainedDecisionTreeNode
    {
        public double Entropy { get; set; }
        public GenericTrainedDecisionTreeNode LeftChild { get; set; }
        public GenericTrainedDecisionTreeNode RightChild { get; set; }
        public GenericDecisionTreeQuestion SplittingRule { get; set; }
        public bool IsLeaf { get; set; }
        public int Class { get; set; }
        public double Certainty { get; set; }
    }
}

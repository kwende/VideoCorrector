using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.LearningAlgorithms
{
    public static class GenericDecisionTreeStatistics
    {
        public static double ComputeEntropy<T>(List<GenericDecisionTreeTrainingData<T>> trainingPoints)
        {
            IEnumerable<IGrouping<int, GenericDecisionTreeTrainingData<T>>> groupings =
                trainingPoints.GroupBy(m => m.Class);

            double entropy = 0;
            double groupingsLength = trainingPoints.Count;
            foreach (IGrouping<int, GenericDecisionTreeTrainingData<T>> group in groupings)
            {
                double ratio = (group.Count() / groupingsLength);
                entropy += -(ratio * System.Math.Log(ratio, 2));
            }

            return entropy;
        }

        public static double ComputeGain<T>(double totalEntropy, List<GenericDecisionTreeTrainingData<T>> left, List<GenericDecisionTreeTrainingData<T>> right)
        {
            double subsetEntropy = 0;
            double totalNumberOfItems = left.Count + right.Count;
            if (totalNumberOfItems > 0)
            {
                double ratio = left.Count / totalNumberOfItems;
                double entropy = ComputeEntropy(left);

                subsetEntropy += (ratio * entropy);

                ratio = right.Count / totalNumberOfItems;
                entropy = ComputeEntropy(right);

                subsetEntropy += (ratio * entropy);

                return totalEntropy - subsetEntropy;
            }
            else
            {
                return 0;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.LearningAlgorithms
{
    public class GenericDecisionTree<T>
    {
        public GenericTrainedDecisionTree Train(IDecisionTreeBuilder<T> builder, DecisionTreeBuilderParameters parameters,
            List<GenericDecisionTreeTrainingData<T>> trainingData)
        {
            List<GenericDecisionTreeQuestion> questions = builder.BuildTrainingQuestions(parameters);

            GenericTrainedDecisionTree tree = new GenericTrainedDecisionTree();
            tree.Root = new GenericTrainedDecisionTreeNode();

            Partition(builder, parameters, trainingData, questions, 0, tree.Root);
            tree.Parameters = parameters;

            return tree;
        }

        private int AskNode(IDecisionTreeBuilder<T> builder, DecisionTreeBuilderParameters parameters, GenericTrainedDecisionTreeNode treeNode, T sample)
        {
            if (!treeNode.IsLeaf)
            {
                switch (builder.ComputeSplitDirection(parameters, new GenericDecisionTreeTrainingData<T> { Data = sample }, treeNode.SplittingRule))
                {
                    case TreeSplitDirectionEnum.Left:
                        return AskNode(builder, parameters, treeNode.LeftChild, sample);
                    case TreeSplitDirectionEnum.Right:
                        return AskNode(builder, parameters, treeNode.RightChild, sample);
                    default:
                        throw new Exception("Unknown splitting type.");
                }
            }
            else
            {
                return treeNode.Class;
            }
        }

        public int ClassifyWithTree(IDecisionTreeBuilder<T> builder, GenericTrainedDecisionTree tree, T sample)
        {
            return AskNode(builder, tree.Parameters, tree.Root, sample);
        }

        private void Partition(IDecisionTreeBuilder<T> builder, DecisionTreeBuilderParameters parameters,
            List<GenericDecisionTreeTrainingData<T>> trainingData,
            List<GenericDecisionTreeQuestion> trainingQuestions, int recursionLevel, GenericTrainedDecisionTreeNode thisNode)
        {
            double fullEntropy = GenericDecisionTreeStatistics.ComputeEntropy(trainingData);

            double highestGain = 0.0;
            GenericDecisionTreeQuestion bestSplittingRule = null;
            List<GenericDecisionTreeTrainingData<T>> bestLeft = null, bestRight = null;

            Parallel.ForEach<GenericDecisionTreeQuestion>(trainingQuestions, candidate =>
            {
                List<GenericDecisionTreeTrainingData<T>> left = new List<GenericDecisionTreeTrainingData<T>>();
                List<GenericDecisionTreeTrainingData<T>> right = new List<GenericDecisionTreeTrainingData<T>>();

                foreach (GenericDecisionTreeTrainingData<T> trainingPoint in trainingData)
                {
                    switch (builder.ComputeSplitDirection(parameters, trainingPoint, candidate))
                    {
                        case TreeSplitDirectionEnum.Left:
                            left.Add(trainingPoint);
                            break;
                        case TreeSplitDirectionEnum.Right:
                            right.Add(trainingPoint);
                            break;
                    }
                }

                double gain = GenericDecisionTreeStatistics.ComputeGain(fullEntropy, left, right);

                lock (this)
                {
                    if (gain > highestGain)
                    {
                        highestGain = gain;
                        bestSplittingRule = candidate;

                        bestLeft = left;
                        bestRight = right;
                    }
                }
            });

            if (bestSplittingRule != null && recursionLevel < parameters.MaxRecursionLevels && highestGain >= parameters.SufficientGainLevel)
            {
                thisNode.Entropy = fullEntropy;
                thisNode.SplittingRule = bestSplittingRule;

                thisNode.LeftChild = new GenericTrainedDecisionTreeNode();
                thisNode.RightChild = new GenericTrainedDecisionTreeNode();

                List<GenericDecisionTreeQuestion> newQuestions = builder.BuildTrainingQuestions(parameters);

                Partition(builder, parameters, bestLeft, newQuestions, recursionLevel + 1, thisNode.LeftChild);
                Partition(builder, parameters, bestRight, newQuestions, recursionLevel + 1, thisNode.RightChild);
            }
            else
            {
                IGrouping<int, GenericDecisionTreeTrainingData<T>>[] parts =
                    trainingData.GroupBy(m => m.Class).OrderByDescending(m => m.Count()).ToArray();

                thisNode.Certainty = (int)((parts[0].Count() / (trainingData.Count * 1.0)) * 100);

                thisNode.IsLeaf = true;
                thisNode.Class = parts[0].Key;
            }
        }
    }
}

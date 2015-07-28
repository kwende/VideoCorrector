using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.LearningAlgorithms
{
    public interface IDecisionTreeBuilder<T>
    {
        List<GenericDecisionTreeQuestion> BuildTrainingQuestions(DecisionTreeBuilderParameters options);
        TreeSplitDirectionEnum ComputeSplitDirection(DecisionTreeBuilderParameters options, GenericDecisionTreeTrainingData<T> trainingDataPoint,
            GenericDecisionTreeQuestion candidate);
    }
}

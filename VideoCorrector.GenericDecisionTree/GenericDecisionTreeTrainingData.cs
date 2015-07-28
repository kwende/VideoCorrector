using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCorrector.LearningAlgorithms
{
    public class GenericDecisionTreeTrainingData<T>
    {
        public int Class { get; set; }
        public T Data { get; set; }
    }
}

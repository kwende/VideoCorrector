using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCorrector.LearningAlgorithms.Specific.GrayscaleCorrectorTree
{
    [Serializable]
    public class GrayscaleDecisionTreeBuilder : IDecisionTreeBuilder<GrayPixel>
    {
        public List<GenericDecisionTreeQuestion> BuildTrainingQuestions(DecisionTreeBuilderParameters options)
        {
            List<GenericDecisionTreeQuestion> pixelQuestions = 
                new List<GenericDecisionTreeQuestion>();

            GrayscaleDecisionTreeParameters grayOptions = 
                options as GrayscaleDecisionTreeParameters;

            Random rand = new Random();
            for (int c = 0; c < grayOptions.NumberOfQuestions; c++)
            {
                int xOffset = rand.Next(grayOptions.MinimumOffset, grayOptions.MaximumOffset);
                int yOffset = rand.Next(grayOptions.MinimumOffset, grayOptions.MaximumOffset);

                pixelQuestions.Add(new GrayPixelQuestion
                    {
                        OffsetVector = new PointF(xOffset, yOffset)
                    });
            }

            return pixelQuestions; 
        }

        public TreeSplitDirectionEnum ComputeSplitDirection(DecisionTreeBuilderParameters options, 
            GenericDecisionTreeTrainingData<GrayPixel> trainingDataPoint, 
            GenericDecisionTreeQuestion candidate)
        {
            GrayscaleDecisionTreeParameters grayOptions =
                options as GrayscaleDecisionTreeParameters;

            GrayPixelQuestion grayCandidateQuestion = candidate as GrayPixelQuestion;
            byte[] imageData = trainingDataPoint.Data.ImageData;

            Point baseCoordinate = trainingDataPoint.Data.Coordinate;
            PointF offsetCoordinate = new PointF(
                baseCoordinate.X + grayCandidateQuestion.OffsetVector.X,
                baseCoordinate.Y + grayCandidateQuestion.OffsetVector.Y);

            int offsetIndex = (int)(offsetCoordinate.Y *
                trainingDataPoint.Data.ImageWidth + offsetCoordinate.X);
            int baseCoordinateIndex = (int)(baseCoordinate.Y *
                trainingDataPoint.Data.ImageWidth + baseCoordinate.X); 

            if(offsetIndex < 0 || offsetIndex > imageData.Length ||
                baseCoordinateIndex < 0 || baseCoordinateIndex > imageData.Length ||
                imageData[offsetIndex] > imageData[baseCoordinateIndex])
            {
                return TreeSplitDirectionEnum.Left; 
            }
            else
            {
                return TreeSplitDirectionEnum.Right; 
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using VideoCorrector.LearningAlgorithms;
using VideoCorrector.LearningAlgorithms.Specific.GrayscaleCorrectorTree;

namespace VideoCorrector.Corrector
{
    class Program
    {
        static void Train()
        {
            Bitmap lowRes = (Bitmap)Bitmap.FromFile("c:/users/ben/desktop/feynman.jpg");
            Bitmap highRes = (Bitmap)Bitmap.FromFile("c:/users/ben/desktop/feynman2.jpg");

            BitmapData lowResBitmapData = lowRes.LockBits(
                new Rectangle(0, 0, lowRes.Width, lowRes.Height),
                ImageLockMode.ReadOnly,
                lowRes.PixelFormat);

            byte[] lowResBytes = new byte[lowResBitmapData.Stride * lowResBitmapData.Height];
            Marshal.Copy(lowResBitmapData.Scan0, lowResBytes, 0, lowResBytes.Length);

            lowRes.UnlockBits(lowResBitmapData);

            List<GenericDecisionTreeTrainingData<GrayPixel>> trainingData =
                new List<GenericDecisionTreeTrainingData<GrayPixel>>();

            for (int y = 0; y < lowRes.Height; y++)
            {
                for (int x = 0; x < lowRes.Width; x++)
                {
                    GenericDecisionTreeTrainingData<GrayPixel> trainingDataPoint =
                        new GenericDecisionTreeTrainingData<GrayPixel>();

                    trainingDataPoint.Data = new GrayPixel();
                    trainingDataPoint.Data.Coordinate = new Point(x, y);
                    trainingDataPoint.Data.ImageData = lowResBytes;
                    trainingDataPoint.Data.ImageWidth = lowRes.Width;
                    trainingDataPoint.Data.ImageHeight = lowRes.Height;
                    trainingDataPoint.Data.OriginalIntensity = highRes.GetPixel(x, y).R;
                    trainingDataPoint.Class = (trainingDataPoint.Data.OriginalIntensity -
                        lowRes.GetPixel(x, y).R);

                    trainingData.Add(trainingDataPoint);
                }
            }

            GrayscaleDecisionTreeParameters parameters = new GrayscaleDecisionTreeParameters
            {
                MaxRecursionLevels = 20,
                SufficientGainLevel = 0,
                MaximumOffset = 50,
                MinimumOffset = 5,
                NumberOfQuestions = 1000
            };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            GenericDecisionTree<GrayPixel> decisionTree = new GenericDecisionTree<GrayPixel>();
            GenericTrainedDecisionTree tree = decisionTree.Train(
                new GrayscaleDecisionTreeBuilder(), parameters, trainingData);

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fout = File.Create("C:/users/ben/desktop/trained.dat"))
            {
                formatter.Serialize(fout, tree);
            }
            sw.Stop();

            Console.WriteLine("Took " + sw.Elapsed.TotalSeconds + " seconds.");
            Console.ReadLine();
            return; 
        }

        static void Test()
        {
            List<GenericDecisionTreeTrainingData<GrayPixel>> trainingData =
                new List<GenericDecisionTreeTrainingData<GrayPixel>>();
            GenericDecisionTree<GrayPixel> decisionTree = new GenericDecisionTree<GrayPixel>(); 

            GenericTrainedDecisionTree trainedTree = null;

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fin = File.OpenRead("C:/users/ben/desktop/trained.dat"))
            {
                trainedTree = formatter.Deserialize(fin) as GenericTrainedDecisionTree;
            }

            Bitmap lowRes = (Bitmap)Bitmap.FromFile("C:/users/ben/desktop/feynman2.jpg");
            BitmapData lowResBitmapData = lowRes.LockBits(
                new Rectangle(0, 0, lowRes.Width, lowRes.Height),
                ImageLockMode.ReadOnly,
                lowRes.PixelFormat);

            byte[] lowResBytes = new byte[lowResBitmapData.Stride * lowResBitmapData.Height];
            Marshal.Copy(lowResBitmapData.Scan0, lowResBytes, 0, lowResBytes.Length);

            lowRes.UnlockBits(lowResBitmapData);
            Bitmap newBitmap = new Bitmap(lowRes.Width, lowRes.Height, lowRes.PixelFormat); 

            for (int y = 0; y < lowRes.Height; y++)
            {
                for (int x = 0; x < lowRes.Width; x++)
                {
                    GrayPixel pixel = new GrayPixel(); 

                    pixel.Coordinate = new Point(x, y);
                    pixel.ImageData = lowResBytes;
                    pixel.ImageWidth = lowRes.Width;
                    pixel.ImageHeight = lowRes.Height;

                    int pixelClass = decisionTree.ClassifyWithTree(new GrayscaleDecisionTreeBuilder(),
                        trainedTree, pixel); 

                    Color lowResColor = lowRes.GetPixel(x,y);

                    int pixelValue = lowResColor.R + pixelClass; 
                    if(pixelValue < 0) pixelValue = 0; 
                    if(pixelValue > 255) pixelValue = 255; 

                    newBitmap.SetPixel(x, y, Color.FromArgb(pixelValue, pixelValue, pixelValue)); 
                }
            }

            newBitmap.Save("C:/users/ben/desktop/new.jpg"); 
        }

        static void Main(string[] args)
        {
            //Train(); 
            Test(); 
        }
    }
}

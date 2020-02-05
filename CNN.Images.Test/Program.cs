using CNN.Images.Model;
using System;

namespace CNN.Images.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Network initialize:
            ServiceCNNImages evaNetwork = new ServiceCNNImages();

            NetworkStructure netStructure = new NetworkStructure
            {
                InputVectorLength = 81,
                NeuronsByLayers = new[] { 75, 75, 75, 50, 1 }
            };

            evaNetwork.CreateNetwork(netStructure, 2, "testDatasets.txt");

            // Main training:
            TrainConfiguration trainConfig = new TrainConfiguration
            {
                StartIteration = 0,
                EndIteration = 10000,
                InputDatasetFilename = "inputSets.txt",
                OutputDatasetFilename = "outputSets.txt",
                SourceFolderName = "images",
                MemoryFolder = ""
            };

            evaNetwork.Train(trainConfig, 10000);

            double[] sssss = evaNetwork.Handle("testImages//img.png");

            Console.ReadKey();
        }
    }
}

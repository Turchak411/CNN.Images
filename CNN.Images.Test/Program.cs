using CNN.Images.Model;
using System;
using System.IO;

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
                InputVectorLength = 192,
                NeuronsByLayers = new[] { 350, 350, 1 }
            };

            evaNetwork.CreateNetwork(netStructure, 3, "testDatasets.txt");

            // Main training:
            TrainConfiguration trainConfig = new TrainConfiguration
            {
                StartIteration = 10300,
                EndIteration = 10000000,
                InputDatasetFilename = "inputSets.txt",
                OutputDatasetFilename = "outputSets.txt",
                SourceFolderName = "images",
                MemoryFolder = ""
            };

            //evaNetwork.Train(trainConfig, 9989700);

            // TODO: Temp
            DirectoryInfo dirInfo = new DirectoryInfo("testImages");
            double[] sssss = evaNetwork.Handle(dirInfo.GetFiles()[0].FullName);

            Console.ReadKey();
        }
    }
}

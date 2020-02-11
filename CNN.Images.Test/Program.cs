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
                NeuronsByLayers = new[] { 200, 200, 1 }
            };

            evaNetwork.CreateNetwork(netStructure, 3, "testDatasets.txt");

            // Main training:
            TrainConfiguration trainConfig = new TrainConfiguration
            {
                StartIteration = 32000000,
                EndIteration = 38500000,
                InputDatasetFilename = "inputSets.txt",
                OutputDatasetFilename = "outputSets.txt",
                SourceFolderName = "images",
                MemoryFolder = ""
            };

            //evaNetwork.Train(trainConfig, 6500000);

            // TODO: Temp
            DirectoryInfo dirInfo = new DirectoryInfo("testImages");
            double[] sssss = evaNetwork.HandleSingleFrame(dirInfo.GetFiles()[0].FullName);
            //evaNetwork.Handle(dirInfo.GetFiles()[0].FullName);

            Console.ReadKey();
        }
    }
}

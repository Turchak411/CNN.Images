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

            //NetworkStructure netStructure = new NetworkStructure
            //{
            //    InputVectorLength = 81,
            //    NeuronsByLayers = new[] { 75, 75, 75, 50, 1 }
            //};

            NetworkStructure netStructure = new NetworkStructure
            {
                InputVectorLength = 108,
                NeuronsByLayers = new[] { 200, 200, 1 }
            };

            evaNetwork.CreateNetwork(netStructure, 3, "testDatasets.txt");

            // Main training:
            TrainConfiguration trainConfig = new TrainConfiguration
            {
                StartIteration = 1000000,
                EndIteration = 1000100,
                InputDatasetFilename = "inputSets.txt",
                OutputDatasetFilename = "outputSets.txt",
                SourceFolderName = "images",
                MemoryFolder = ""
            };

            evaNetwork.Train(trainConfig, 100);

            // TODO: Убрать потом
            DirectoryInfo dirInfo = new DirectoryInfo("testImages");
            double[] sssss = evaNetwork.Handle(dirInfo.GetFiles()[0].FullName);

            Console.ReadKey();
        }
    }
}

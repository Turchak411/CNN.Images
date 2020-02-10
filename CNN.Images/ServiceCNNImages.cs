using CNN.Images.Core;
using CNN.Images.Model;
using CNN.Images.Services;
using System;
using System.Collections.Generic;

namespace CNN.Images
{
    public class ServiceCNNImages
    {
        private FileManager _fileManager;
        private NetworksTeacher _networkTeacher;

        // TODO: Выделить структуру Extraction-зоны в отдельный объект
        public void CreateNetwork(NetworkStructure networkStructure,
                                  int netsCountInAssembly = 1,
                                  string testDatasetsPath = null)
        {
            _fileManager = new FileManager(networkStructure);

            // TODO: [WARP] Тут настраивается схема слоев convolution и pooling
            Extractor extractor = new Extractor("cpcppp", CreateConvFiltersScheme());

            _networkTeacher = new NetworksTeacher(extractor, networkStructure, netsCountInAssembly, _fileManager);

            // TODO: Сделать тестовое, но по изображению
            //if (testDatasetsPath != null)
            //{
            //    _networkTeacher.TestVectors = _fileManager.LoadDatasets(testDatasetsPath);
            //}
        }

        private static List<List<FilterName>> CreateConvFiltersScheme()
        {
            // TODO: [WARP] Тут настраивается список kernel'ов (фильтров) для каждого convolution слоя
            List<List<FilterName>> convFilters = new List<List<FilterName>>();

            // Conv 0:
            List<FilterName> filtersConv0 = new List<FilterName>();
            filtersConv0.Add(FilterName.SobelHorizontal);
            filtersConv0.Add(FilterName.SobelVertical);

            // Conv 0:
            List<FilterName> filtersConv1 = new List<FilterName>();
            filtersConv1.Add(FilterName.SobelHorizontal);
            filtersConv1.Add(FilterName.SobelVertical);

            convFilters.Add(filtersConv0);
            convFilters.Add(filtersConv1);

            return convFilters;
        }

        public void Train(TrainConfiguration trainConfiguration, int iterationToPause = 100)
        {
            if (_networkTeacher.CheckMemory(trainConfiguration.MemoryFolder))
            {
                _networkTeacher.TrainNets(trainConfiguration, iterationToPause);

                _networkTeacher.PrintLearnStatistic(trainConfiguration, true);

                if (_networkTeacher.CheckMemory(trainConfiguration.MemoryFolder))
                {
                    _networkTeacher.BackupMemory();
                }
            }
            else
            {
                Console.WriteLine("Train failed! Invalid memory!");
            }
        }

        public double[] HandleSingleFrame(string imageFilename)
        {
            return _networkTeacher.HandleSingleFrame(imageFilename);
        }

        public void Handle(string imageFilename)
        {
            _networkTeacher.Handle(imageFilename);
        }
    }
}

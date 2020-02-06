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

        public void CreateNetwork(NetworkStructure networkStructure,
                                  int netsCountInAssembly = 1,
                                  string testDatasetsPath = null)
        {
            _fileManager = new FileManager(networkStructure);

            Extractor extractor = new Extractor("cppp", CreateConvFiltersScheme());

            _networkTeacher = new NetworksTeacher(extractor, networkStructure, netsCountInAssembly, _fileManager);

            // TODO: Сделать тестовое, но по изображению
            //if (testDatasetsPath != null)
            //{
            //    _networkTeacher.TestVectors = _fileManager.LoadDatasets(testDatasetsPath);
            //}
        }

        private static List<List<FilterName>> CreateConvFiltersScheme()
        {
            List<List<FilterName>> convFilters = new List<List<FilterName>>();

            // Conv 0:
            List<FilterName> filtersConv0 = new List<FilterName>();
            filtersConv0.Add(FilterName.Blur);
            filtersConv0.Add(FilterName.Clarity);
            filtersConv0.Add(FilterName.Relief);

            // Conv 1:
            List<FilterName> filtersConv1 = new List<FilterName>();
            filtersConv1.Add(FilterName.Blur);
            filtersConv1.Add(FilterName.Clarity);
            filtersConv1.Add(FilterName.Relief);

            convFilters.Add(filtersConv0);
            //convFilters.Add(filtersConv1);

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

        public double[] Handle(string imageFilename)
        {
            return _networkTeacher.Handle(imageFilename);
        }
    }
}

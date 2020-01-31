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

            // TODO: Перенести схемы экстракт слоев в отдельный объект, а пока
            string filtersFilename = "filters.txt";
            Extractor extractor = new Extractor("cpcp", CreateConvFiltersScheme(), filtersFilename);

            _networkTeacher = new NetworksTeacher(extractor, networkStructure, netsCountInAssembly, _fileManager);

            // TODO: Сделать тестовое, но по изображению
            //if (testDatasetsPath != null)
            //{
            //    _networkTeacher.TestVectors = _fileManager.LoadDatasets(testDatasetsPath);
            //}
        }

        // TODO: Потом перенести куда-нибудь, но не сюда
        private static List<List<Filter>> CreateConvFiltersScheme()
        {
            List<List<Filter>> convFilters = new List<List<Filter>>();

            // Conv 0:
            List<Filter> filtersConv0 = new List<Filter>();
            filtersConv0.Add(Filter.Blur);
            filtersConv0.Add(Filter.Clarity);
            filtersConv0.Add(Filter.Relief);

            // Conv 1:
            List<Filter> filtersConv1 = new List<Filter>();
            filtersConv1.Add(Filter.Blur);
            filtersConv1.Add(Filter.Clarity);
            filtersConv1.Add(Filter.Relief);

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

        public double[] Handle(string imageFilename)
        {
            return _networkTeacher.Handle(imageFilename);
        }
    }
}

using CNN.Images.Model;
using CNN.Images.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CNN.Images.Core
{
    public class NetworksTeacher
    {
        private List<ConvolutionalNeuralNetwork> _netsList;

        /// <summary>
        /// Services
        /// </summary>
        private FileManager _fileManager;
        private ImageLoader _imageLoader;
        private Extractor _extractor;
        private MemoryChecker _memoryChecker;
        private Logger _logger;

        /// <summary>
        /// Global current iterations
        /// </summary>
        public int Iteration { get; set; } = 0;

        /// <summary>
        /// Testing objects
        /// </summary>
        public List<TrainObject> TestVectors { get; set; }

        public NetworksTeacher(Extractor extractor, NetworkStructure netStructure, int netsCount, FileManager fileManager)
        {
            _netsList = new List<ConvolutionalNeuralNetwork>();

            _fileManager = fileManager;
            _extractor = extractor;
            // TODO: [WARP] Тут настраивается размеры исходного обрабатываемого изображения
            // TODO: Выделить размеры изображения возможно в отдельную структуру, чтобы вызывать выше
            _imageLoader = new ImageLoader(50, 50); // 20,20);
            _logger = new Logger();

            try
            {
                // Ицициализация сети по одинаковому шаблону:
                for (int i = 0; i < netsCount; i++)
                {
                    _netsList.Add(new ConvolutionalNeuralNetwork(_extractor, netStructure.InputVectorLength, netStructure.NeuronsByLayers, _fileManager, "memory_" + i + ".txt"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorType.MemoryInitializeError, ex);
            }
        }

        public void CommonTest()
        {
            if (TestVectors == null) return;

            var result = new StringBuilder();
            TestVectors.ForEach(vector => result.Append($"   {vector._content}     "));
            result.Append('\n');

            for (int i = 0; i < _netsList.Count; i++)
            {
                for (int k = 0; k < TestVectors.Count; k++)
                {
                    // Получение ответа:
                    var outputVector = _netsList[i].Handle(TestVectors[k]._vectorValues);

                    result.Append($"{outputVector[0]:f5}\t");
                }
                result.Append('\n');
            }

            Console.WriteLine(result);
        }

        public void CommonTestColorized()
        {
            if (TestVectors == null) return;

            var result = new StringBuilder();
            TestVectors.ForEach(vector => result.Append($"   {vector._content}     "));
            result.Append('\n');

            for (int i = 0; i < _netsList.Count; i++)
            {
                for (int k = 0; k < TestVectors.Count; k++)
                {
                    // Получение ответа:
                    var outputVector = _netsList[i].Handle(TestVectors[k]._vectorValues);

                    try
                    {
                        // Костыль: для корректного теста сетям нужна по крайней мере одна итерация обучения:
                        _netsList[i].Teach(TestVectors[k]._vectorValues, new double[1] { 1 }, 0.01); //0.000000000000001);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ErrorType.TrainError, ex);
                    }

                    Console.ForegroundColor = GetColorByActivation(outputVector[0]);
                    Console.Write($"{outputVector[0]:f5}\t");
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write('\n');
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private ConsoleColor GetColorByActivation(double value)
        {
            if (value > 0.95)
            {
                return ConsoleColor.Red;
            }

            if (value > 0.8)
            {
                return ConsoleColor.Magenta;
            }

            if (value > 0.5)
            {
                return ConsoleColor.Yellow;
            }

            return ConsoleColor.Gray;
        }

        public void PrintLearnStatistic(TrainConfiguration trainConfig, bool withLogging = false)
        {
            Console.WriteLine("Start calculating statistic...");

            int testPassed = 0;
            int testFailed = 0;
            int testFailed_lowActivationCause = 0;

            #region Load data from file

            List<double[]> inputDataSets;
            List<double[]> outputDataSets;

            try
            {
                inputDataSets = _fileManager.LoadSingleDataset(trainConfig.InputDatasetFilename);
                outputDataSets = _fileManager.LoadSingleDataset(trainConfig.OutputDatasetFilename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorType.SetMissing, ex);
                return;
            }

            #endregion

            for (int i = 0; i < inputDataSets.Count; i++)
            {
                List<double> netResults = new List<double>();

                for (int k = 0; k < _netsList.Count; k++)
                {
                    // Получение ответа:
                    netResults.Add(_netsList[k].Handle(inputDataSets[i])[0]);
                }

                // Поиск максимально активирующейся сети (класса) с заданным порогом активации:
                int maxIndex = FindMaxIndex(netResults, 0.8);

                if (maxIndex == -1)
                {
                    testFailed++;
                    testFailed_lowActivationCause++;
                }
                else
                {
                    if (outputDataSets[i][maxIndex] != 1)
                    {
                        testFailed++;
                    }
                    else
                    {
                        testPassed++;
                    }
                }
            }

            // Logging (optional):
            if (withLogging)
            {
                _logger.LogTrainResults(testPassed, testFailed, testFailed_lowActivationCause, Iteration);
            }

            Console.WriteLine("Test passed: {0}\nTest failed: {1}\n     - Low activation causes: {2}\nPercent learned: {3:f2}", testPassed,
                                                                                                                           testFailed,
                                                                                                                           testFailed_lowActivationCause,
                                                                                                                           (double)testPassed * 100 / (testPassed + testFailed));
        }

        public bool CheckMemory(string memoryFolder = "")
        {
            bool isValid = true;

            Console.WriteLine("Start memory cheking...");

            _memoryChecker = new MemoryChecker();

            for (int i = 0; i < _netsList.Count; i++)
            {
                string pathAd = "";

                if (memoryFolder != "")
                {
                    pathAd = "//";
                }

                if (_memoryChecker.IsValid(memoryFolder + pathAd + "memory_" + i + ".txt"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("memory_" + i + " - is valid.");
                }
                else
                {
                    isValid = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("memory_" + i + " - is invalid!");
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;

            return isValid;
        }

        private int FindMaxIndex(List<double> netResults, double threshold = 0.8)
        {
            int maxIndex = -1;
            double maxValue = -1;

            for (int i = 0; i < netResults.Count; i++)
            {
                if (maxValue < netResults[i] && netResults[i] >= threshold)
                {
                    maxIndex = i;
                    maxValue = netResults[i];
                }
            }

            return maxIndex;
        }

        public void BackupMemory(string backupsDirectoryName = ".memory_backups")
        {
            // Check for existing main backups-directory:
            if (!Directory.Exists(backupsDirectoryName))
            {
                Directory.CreateDirectory(backupsDirectoryName);
            }

            // Check for already-existing sub-directory (trainCount-named):
            if (!Directory.Exists(backupsDirectoryName + "/ " + Iteration))
            {
                Directory.CreateDirectory(backupsDirectoryName + "/" + Iteration);
            }

            // Saving memory:
            for (int i = 0; i < _netsList.Count; i++)
            {
                _netsList[i].SaveMemory(backupsDirectoryName + "/" + Iteration + "/memory_" + i + ".txt");
            }

            Console.WriteLine("Memory backuped!");
        }

        /// <summary>
        /// Обучение сети
        /// </summary>
        /// <param name="startIteration"></param>
        /// <param name="withSort"></param>
        public void TrainNets(TrainConfiguration trainConfig, int iterationsToPause)
        {
            Iteration = trainConfig.EndIteration;

            // 1. Создание обучающих сетов из указанных папок:
            _fileManager.CreateTrainSets(_extractor, _imageLoader, trainConfig);

            #region Load data from file

            Console.WriteLine("Load train sets...");

            List<double[]> inputDataSets;
            List<double[]> outputDataSets;

            try
            {
                inputDataSets = _fileManager.LoadSingleDataset(trainConfig.InputDatasetFilename);
                outputDataSets = _fileManager.LoadSingleDataset(trainConfig.OutputDatasetFilename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorType.SetMissing, ex);
                return;
            }

            #endregion

            Console.WriteLine("Training nets...");
            try
            {
                SingleNetworkTeacher[] netTeachers = new SingleNetworkTeacher[_netsList.Count];

                List<TrainConfiguration> trainConfigs = InitializeTrainConfigs(trainConfig, iterationsToPause);

                // Initialize teachers:
                for (int i = 0; i < netTeachers.Length; i++)
                {
                    netTeachers[i] = new SingleNetworkTeacher
                    {
                        Id = i,
                        Network = _netsList[i],
                        TrainConfiguration = trainConfig,
                        InputDatasets = inputDataSets,
                        OutputDatasets = outputDataSets,
                        Logger = _logger
                    };
                }

                List<Task> tasks;

                // Iteration multithreading train:
                for (int j = 0; j < trainConfigs.Count; j++)
                {
                    tasks = new List<Task>();

                    for (int i = 0; i < netTeachers.Length; i++)
                    {
                        var teacherNumber = i;
                        var task = Task.Run(() => { netTeachers[teacherNumber].Train(); });
                        tasks.Add(task);
                    };

                    Task.WaitAll(tasks.ToArray());

                    if (j != trainConfigs.Count - 1)
                    {
                        Console.WriteLine("Iterations already finished: " + iterationsToPause * (j + 1));
                    }
                    else
                    {
                        Console.WriteLine("Iterations already finished: " + trainConfig.EndIteration);
                    }

                    CommonTestColorized();
                }

                Console.WriteLine("Training success!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorType.TrainError, ex);
            }
        }

        private List<TrainConfiguration> InitializeTrainConfigs(TrainConfiguration trainConfig, int iterationsToPause)
        {
            List<TrainConfiguration> trainConfigs = new List<TrainConfiguration>();

            int currentIterPosition = trainConfig.StartIteration;
            while (true)
            {
                if (trainConfig.EndIteration - currentIterPosition - 1 >= iterationsToPause)
                {
                    var trainConfigItem = new TrainConfiguration()
                    {
                        StartIteration = currentIterPosition,
                        EndIteration = currentIterPosition + iterationsToPause,
                        InputDatasetFilename = trainConfig.InputDatasetFilename,
                        OutputDatasetFilename = trainConfig.OutputDatasetFilename
                    };

                    trainConfigs.Add(trainConfigItem);

                    currentIterPosition += iterationsToPause;
                }
                else
                {
                    var trainConfigItem = new TrainConfiguration()
                    {
                        StartIteration = currentIterPosition,
                        EndIteration = trainConfig.EndIteration,
                        InputDatasetFilename = trainConfig.InputDatasetFilename,
                        OutputDatasetFilename = trainConfig.OutputDatasetFilename
                    };

                    trainConfigs.Add(trainConfigItem);

                    break;
                }
            }

            Console.WriteLine("Train configuration object created!");

            return trainConfigs;
        }

        public void Handle(string imageFileName)
        {
            // Получение подкартинок картинки:
            Framer framer = new Framer();
            List<FrameObject> allImageObjects = framer.GetFrameBitmapsAsObject(_imageLoader.LoadImage(imageFileName));

            // Получение матриц подкартинок:
            List<List<double[,]>> imagesMatrixList = new List<List<double[,]>>();

            for (int i = 0; i < allImageObjects.Count; i++)
            {
                imagesMatrixList.Add(_imageLoader.LoadImageDataRGB(allImageObjects[i].BitmapImage));
            }

            // Дополнение к полученным объектам выше данных матрицы их фрейма:
            for(int i = 0; i < allImageObjects.Count; i++)
            {
                allImageObjects[i].MatrixListRGB = imagesMatrixList[i];
            }

            // Дополнение к полученным объектам выше данных векторов переведенных из матриц:
            for (int i = 0; i < allImageObjects.Count; i++)
            {
                allImageObjects[i].VectorData = _extractor.Extract(imagesMatrixList[i]);
            }

            // Получение результатов:
            List<double[]> results = new List<double[]>();

            for (int i = 0; i < allImageObjects.Count; i++)
            {
                double[] result = new double[_netsList.Count];

                for (int k = 0; k < _netsList.Count; k++)
                {
                    result[k] = _netsList[k].Handle(allImageObjects[i].VectorData)[0];
                }

                results.Add(result);
            }

            // Подсчет индексов максимальных из них и отбор только тех результатов, которые выше порога:
            List<int> maxResultsIndexes = new List<int>();

            for (int i = 0; i < results.Count; i++)
            {
                int higherResultIndex = GetHigherResultIndex(results[i], 0.55);

                // Если порог рассматриваемых по объекту результатов не пройден, то удаление этого объекта и результатов:
                if (higherResultIndex == -1)
                {
                    allImageObjects.RemoveAt(i);
                    results.RemoveAt(i);
                    i--;
                }
                else
                {
                    maxResultsIndexes.Add(higherResultIndex);
                }
            }

            // Создание картинки, на которой будут все найденные объекты, обозначенные рамками:
            _imageLoader.CreateOutputImage(maxResultsIndexes, results, allImageObjects, imageFileName);
        }

        private int GetHigherResultIndex(double[] result, double threshold)
        {
            double maxValue = -1;
            int maxValueIndex = -1;

            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] > maxValue && result[i] > threshold)
                {
                    maxValue = result[i];
                    maxValueIndex = i;
                }
            }

            if (maxValueIndex == -1)
            {
                return -1;
            }
            else
            {
                return maxValueIndex;
            }
        }

        public double[] HandleSingleFrame(string imageFileName)
        {
            List<double[,]> imgMatrixList = _imageLoader.LoadImageDataRGB(imageFileName);
            double[] data = _extractor.Extract(imgMatrixList);

            double[] result = new double[_netsList.Count];

            for (int i = 0; i < _netsList.Count; i++)
            {
                result[i] = _netsList[i].Handle(data)[0];
            }

            return result;
        }
    }
}

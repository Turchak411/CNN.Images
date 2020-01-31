using System;
using System.Collections.Generic;
using System.Text;

namespace CNN.Images.Model
{
    public class TrainConfiguration
    {
        public int StartIteration { get; set; }

        public int EndIteration { get; set; }

        public string InputDatasetFilename { get; set; }

        public string OutputDatasetFilename { get; set; }

        public string SourceFolderName { get; set; }

        public string MemoryFolder { get; set; }
    }
}

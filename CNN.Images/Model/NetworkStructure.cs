using System;
using System.Collections.Generic;
using System.Text;

namespace CNN.Images.Model
{
    public class NetworkStructure
    {
        public int InputVectorLength { get; set; }

        public int[] NeuronsByLayers { get; set; }
    }
}

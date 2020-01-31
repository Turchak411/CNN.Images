using System.Collections.Generic;

namespace CNN.Images.Model.Interfaces
{
    public interface IExtractLayer
    {
        List<double[,]> Handle(List<double[,]> inputMatrix);
    }
}

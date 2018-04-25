using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureBitsData
{
    public interface IFeatureBitsRepo
    {
        IEnumerable<FeatureBitDefinition> GetAll();
        FeatureBitDefinition Add(FeatureBitDefinition definition);
    }
}

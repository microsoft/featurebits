using System.Collections.Generic;

namespace FeatureBitsData
{
    public interface IFeatureBitsRepo
    {
        IEnumerable<FeatureBitDefinition> GetAll();
        FeatureBitDefinition Add(FeatureBitDefinition definition);
        void Update(FeatureBitDefinition definition);
    }
}

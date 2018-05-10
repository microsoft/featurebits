using Newtonsoft.Json;

namespace FeatureBits.Core
{
    /// <summary>
    /// This class is the default implementation of IFeatureBitsReader and reads all the feature bits from the data store.
    /// </summary>
    // TODO: Move this to the FeatureBits.Data package
    public class FeatureBitsJsonReader : IFeatureBitsReader
    {
        private readonly string _featureBitsFile;

        public FeatureBitsJsonReader(string featureBitsFile)
        {
            _featureBitsFile = featureBitsFile;
        }

        /// <summary>
        /// This is the method that returns all the feature bits in the data store
        /// </summary>
        /// <returns>All Feature BitsData</returns>
        public FeatureBitsData ReadFeatureBits()
        {
            FeatureBitsData featureBitsData = JsonConvert.DeserializeObject<FeatureBitsData>(FileSystemContext.ReadAllText(_featureBitsFile));
            return featureBitsData;
        }
    }
}

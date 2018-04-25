using Dotnet.FBit.CommandOptions;
using FeatureBitsData;

namespace Dotnet.FBit.Command
{
    public class AddCommand
    {
        private readonly AddOptions _opts;
        private readonly IFeatureBitsRepo _repo;

        public AddCommand(AddOptions opts, IFeatureBitsRepo repo)
        {
            _opts = opts;
            _repo = repo;
        }

        public int Run()
        {
            throw new System.NotImplementedException();
        }
    }
}

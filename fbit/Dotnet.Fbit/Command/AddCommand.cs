using System;
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
            _opts = opts ?? throw new ArgumentNullException(nameof(opts));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public int Run()
        {
            int returnValue = 0;
            try
            {
                var newBit = BuildBit();
                _repo.Add(newBit);
            }
            catch (Exception e)
            {
                SystemContext.ConsoleErrorWriteLine(e.ToString());
                returnValue = 1;
            }

            SystemContext.ConsoleWriteLine("Feature bit added.");
            return returnValue;
        }

        public FeatureBitDefinition BuildBit()
        {
            var now = SystemContext.Now();
            var username = SystemContext.GetEnvironmentVariable("USERNAME");
            return new FeatureBitDefinition
            {
                Name = _opts.Name,
                CreatedDateTime = now,
                LastModifiedDateTime = now,
                CreatedByUser = username,
                LastModifiedByUser = username,
                OnOff = _opts.OnOff,
                ExcludedEnvironments = _opts.ExcludedEnvironments,
                MinimumAllowedPermissionLevel = _opts.PermissionLevel
            };
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Core;
using FeatureBits.Data;

namespace Dotnet.FBit.Command
{
    /// <summary>
    /// Class that represents the command to Remove a feature bit
    /// </summary>
    public class RemoveCommand
    {
        private readonly RemoveOptions _opts;
        private readonly IFeatureBitsRepo _repo;

        public RemoveCommand(RemoveOptions opts, IFeatureBitsRepo repo)
        {
            _opts = opts ?? throw new ArgumentNullException(nameof(opts), "RemoveOptions object is required.");
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "FeatureBits repository is required.");
        }

        public async Task<int> RunAsync()
        {
            int returnValue = 0;
            var name = _opts.Name;
            var def = await _repo.GetByNameAsync(name);
            if (def == null)
            {
                SystemContext.ConsoleErrorWriteLine($"Feature bit '{_opts.Name}' could not be found.");
                returnValue = 1;
            }
            else
            {
                returnValue = await HandleFeatureBitHasDependency(def);
            }

            return returnValue;
        }

        private async Task<int> HandleFeatureBitHasDependency(IFeatureBitDefinition definition)
        {
            int returnValue;
            var features = await _repo.GetAllAsync();
            bool filterDependencies(IFeatureBitDefinition w)
            {
                var result = false;
                if (!string.IsNullOrEmpty(w.Dependencies))
                {
                    var dependencyNames = w.Dependencies.SplitToStrings();
                    result = (dependencyNames.Any(name => name == definition.Name));
                }
                return result;
            }
            var dependencyFeatures = features.Where(filterDependencies);

            var hasDependency = dependencyFeatures.Any();
            if (hasDependency)
            {
                returnValue = !_opts.Force ? FailWithoutForce() : await ForceRemoveWithDependents(dependencyFeatures, definition);
            }
            else
            {
                returnValue = await ForceRemove(definition);
            }

            return returnValue;
        }

        private int FailWithoutForce()
        {
            SystemContext.ConsoleErrorWriteLine(
                $"Feature bit '{_opts.Name}' has a dependency. Use --force to remove feature bit dependencies.");
            return 1;
        }

        private async Task<int> ForceRemoveWithDependents(IEnumerable<IFeatureBitDefinition> features, IFeatureBitDefinition definition)
        {
            features.ToList().ForEach(async feature =>
            {
                var bits = feature.Dependencies.SplitToStrings().Where(name => name != definition.Name);
                var modifiedBit = BuildBit(feature, bits);
                await _repo.UpdateAsync(modifiedBit);
            });
            return await ForceRemove(definition);
        }

        private async Task<int> ForceRemove(IFeatureBitDefinition definition)
        {
            await _repo.RemoveAsync(definition);
            SystemContext.ConsoleWriteLine("Feature bit removed.");
            return 0;
        }

        public IFeatureBitDefinition BuildBit(IFeatureBitDefinition featureBitDefinition, IEnumerable<string> names)
        {
            var now = SystemContext.Now();
            var username = SystemContext.GetEnvironmentVariable("USERNAME");
            return new CommandFeatureBitDefintion
            {
                Name = featureBitDefinition.Name,
                CreatedDateTime = featureBitDefinition.CreatedDateTime,
                LastModifiedDateTime = now,
                CreatedByUser = featureBitDefinition.CreatedByUser,
                LastModifiedByUser = username,
                OnOff = featureBitDefinition.OnOff,
                ExcludedEnvironments = featureBitDefinition.ExcludedEnvironments,
                MinimumAllowedPermissionLevel = featureBitDefinition.MinimumAllowedPermissionLevel,
                ExactAllowedPermissionLevel = featureBitDefinition.ExactAllowedPermissionLevel,
                Dependencies = string.Join(",", names.Select(s => s.Trim()))
            };
        }
    }
}

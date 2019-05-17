// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Core;
using FeatureBits.Data;
using System.Linq;

namespace Dotnet.FBit.Command
{
    /// <summary>
    /// Class that represents the command to add (or update) a feature bit
    /// </summary>
    public class AddCommand
    {
        private readonly AddOptions _opts;
        private readonly IFeatureBitsRepo _repo;

        public AddCommand(AddOptions opts, IFeatureBitsRepo repo)
        {
            _opts = opts ?? throw new ArgumentNullException(nameof(opts), "AddOptions object is required.");
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "FeatureBits repository is required.");
        }

        public async Task<int> RunAsync()
        {
            int returnValue = 0;
            try
            {
                IFeatureBitDefinition newBit = await BuildBit();
                await _repo.AddAsync(newBit);
                SystemContext.ConsoleWriteLine("Feature bit added.");
            }
            catch (DataException e)
            {
                returnValue = await HandleFeatureBitAlreadyExists(e);
            }
            catch (FeatureBitException e)
            {
                returnValue = 1;
                SystemContext.ConsoleErrorWriteLine(e.Message);
            }
            catch (Exception e)
            {
                returnValue = 1;
                SystemContext.ConsoleErrorWriteLine(e.ToString());
            }

            return returnValue;
        }

        public async Task<IFeatureBitDefinition> BuildBit()
        {
            var now = SystemContext.Now();
            var username = SystemContext.GetEnvironmentVariable("USERNAME");
            return new CommandFeatureBitDefintion
            {
                Name = _opts.Name,
                CreatedDateTime = now,
                LastModifiedDateTime = now,
                CreatedByUser = username,
                LastModifiedByUser = username,
                OnOff = ParseOnOff(),
                ExcludedEnvironments = _opts.ExcludedEnvironments,
                MinimumAllowedPermissionLevel = _opts.MinimumPermissionLevel,
                ExactAllowedPermissionLevel = _opts.ExactPermissionLevel,
                Dependencies = await ValidateHierarchyAndEnsureIds()
            };
        }

        private bool ParseOnOff()
        {
            bool success = bool.TryParse(_opts.OnOff, out var onOffFlag);
            if (!success)
                onOffFlag = false;
            return onOffFlag;
        }

        private async Task<int> HandleFeatureBitAlreadyExists(DataException e)
        {
            int returnValue;
            if (e.Message == ($"Cannot add. Feature bit with name '{_opts.Name}' already exists."))
            {
                returnValue = !_opts.Force ? FailWithoutForce() : await ForceUpdate();
            }
            else
            {
                returnValue = 1;
                SystemContext.ConsoleErrorWriteLine(e.ToString());
            }

            return returnValue;
        }

        private async Task<int> ForceUpdate()
        {
            var newBit = await BuildBit();
            await _repo.UpdateAsync(newBit);
            SystemContext.ConsoleWriteLine("Feature bit updated.");
            return 0;
        }

        private int FailWithoutForce()
        {
            SystemContext.ConsoleErrorWriteLine(
                $"Feature bit '{_opts.Name}' already exists. Use --force to overwrite existing feature bits.");
            return 1;
        }

        const int MaxEvaluations = 3;
        private async Task<string> ValidateHierarchyAndEnsureIds()
        {
            if (!string.IsNullOrEmpty(_opts.Dependents))
            {
                var features = await _repo.GetAllAsync();
                var bits = _opts.Dependents.SplitToStrings();
                if (bits.Any(fbit => !features.Any(feature => fbit == feature.Name)))
                {
                    throw new FeatureBitException($"Feature bit '{_opts.Name}' has an invalid dependency [{_opts.Dependents}].");
                }

                var Dependencies = features.Where(feature => bits.Any(bit => bit == feature.Name)).Select(s => s.Id);
                if (CheckRecursiveDependents(features, Dependencies))
                {
                    return string.Join(",", features.Where(s => bits.Any(name => name == s.Name)).Select(s => s.Name));
                }
                else
                {
                    throw new FeatureBitException($"Feature bit '{_opts.Name}' has a recursive loop [{_opts.Dependents}].");
                }
            }
            return "";
        }

        private bool CheckRecursiveDependents(IEnumerable<IFeatureBitDefinition> features, IEnumerable<int> ids)
        {
            var filtered = features.Where(fbit => ids.Any(id => id == fbit.Id) && !string.IsNullOrEmpty(fbit.Dependencies));
            if (filtered.Any())
            {
                foreach (var feature in filtered)
                {
                    var dependentNames = feature.Dependencies.SplitToStrings();
                    if (dependentNames.Any())
                    {
                        // First Level Dependencies
                        var models = new List<DependencyModel>();
                        foreach (var dependentName in dependentNames)
                        {
                            models.AddRange(CheckRecursiveDependents(features, dependentName, dependentName, 0));
                        }

                        if (models.Distinct(new DependencyComparer()).Count() != models.Count())
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private IEnumerable<DependencyModel> CheckRecursiveDependents(IEnumerable<IFeatureBitDefinition> features, string owningBitName, string dependentBitName, int checkLevel)
        {
            var recursionResult = new List<DependencyModel>();
            var childDefinition = features.FirstOrDefault(i => i.Name == dependentBitName);
            var dependentNames = childDefinition.Dependencies.SplitToStrings();
            if (dependentNames?.Any() == true && checkLevel <= MaxEvaluations)
            {
                foreach (var bitName in dependentNames)
                {
                    recursionResult.Add(new DependencyModel { OwningId = owningBitName, ParentId = dependentBitName, ChildId = bitName });
                    checkLevel += 1;
                    var tempResults = CheckRecursiveDependents(features, owningBitName, bitName, checkLevel);
                    if (tempResults.Any())
                    {
                        recursionResult.AddRange(tempResults);
                    }
                }
            }
            return recursionResult;
        }
    }
}
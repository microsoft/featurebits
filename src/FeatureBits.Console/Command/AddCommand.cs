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
                DependentIds = await ValidateHierarchyAndEnsureIds()
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

                var dependentIds = features.Where(feature => bits.Any(bit => bit == feature.Name)).Select(s => s.Id);
                if (CheckRecursiveDependents(features, dependentIds))
                {
                    return string.Join(",", features.Where(s => bits.Any(name => name == s.Name)).Select(s => s.Id));
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
            var filtered = features.Where(fbit => ids.Any(id => id == fbit.Id) && !string.IsNullOrEmpty(fbit.DependentIds));
            if (filtered.Any())
            {
                foreach (var feature in filtered)
                {
                    HasRecursion = false;
                    var items = CheckRecursiveDependents(features, 0, feature);
                    if (HasRecursion) // if a path reached max threshold [possible recursion]
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool HasRecursion { get; set; } = false;

        private IEnumerable<DependencyModel> CheckRecursiveDependents(IEnumerable<IFeatureBitDefinition> features, int checkLevel, IFeatureBitDefinition definition)
        {
            var recursionResult = new List<DependencyModel>();

            if (checkLevel > MaxEvaluations)
            {
                HasRecursion = true;
            }

            var dependentIds = definition.DependentIds.SplitToInts();
            if (dependentIds.Any() && checkLevel <= MaxEvaluations)
            {
                foreach (var dependentId in dependentIds)
                {
                    var childDefinition = features.FirstOrDefault(i => i.Id == dependentId);
                    recursionResult.Add(new DependencyModel { ParentId = definition.Id, ChildId = dependentId });
                    checkLevel += 1;
                    var tempResults = CheckRecursiveDependents(features, checkLevel, childDefinition);
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
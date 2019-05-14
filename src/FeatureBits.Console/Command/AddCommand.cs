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
                DependantIds = await ValidateHierarchyAndEnsureIds()
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
            if (!string.IsNullOrEmpty(_opts.Dependants))
            {
                var features = await _repo.GetAllAsync();
                var bits = _opts.Dependants.GetDependantNames();
                if (bits.Any(fbit => !features.Any(feature => fbit == feature.Name)))
                {
                    throw new Exception($"Feature bit '{_opts.Name}' has 1 or more invalid dependancies {_opts.Dependants}.");
                }

                var dependantIds = features.Where(feature => bits.Any(bit => bit == feature.Name)).Select(s => s.Id);
                if (CheckRecursiveDependants(features, dependantIds))
                {
                    return string.Join(",", features.Where(s => bits.Any(name => name == s.Name)).Select(s => s.Id));
                }
                else
                {
                    throw new Exception($"Feature bit '{_opts.Name}' has a recursive loop {_opts.Dependants}.");
                }
            }
            return "";
        }

        private bool CheckRecursiveDependants(IEnumerable<IFeatureBitDefinition> features, IEnumerable<int> ids)
        {
            var filtered = features.Where(fbit => ids.Any(id => id == fbit.Id) && !string.IsNullOrEmpty(fbit.DependantIds));
            if (filtered.Any())
            {
                foreach (var feature in filtered)
                {

                    var items = CheckRecursiveDependants(features, 0, feature);
                    if (items.Any(lineage => !lineage.Flipped)) // if a path reached max threshold [possible recursion]
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private IEnumerable<RecursionItem> CheckRecursiveDependants(IEnumerable<IFeatureBitDefinition> features, int checkLevel, IFeatureBitDefinition definition)
        {
            var recursionResult = new List<RecursionItem>();

            var dependantIds = definition.DependantIds.GetDependantIds();
            if (dependantIds.Any() && checkLevel <= MaxEvaluations)
            {
                foreach (var dependantId in dependantIds)
                {
                    var childDefinition = features.FirstOrDefault(i => i.Id == dependantId);
                    var item = new RecursionItem
                    {
                        ParentId = definition.Id,
                        ChildId = dependantId,
                        Flipped = checkLevel < MaxEvaluations
                    };
                    recursionResult.Add(item);
                    var tempResults = CheckRecursiveDependants(features, checkLevel++, childDefinition);
                    if (tempResults.Count() > 0)
                    {
                        recursionResult.AddRange(tempResults);
                    }
                }
            }
            return recursionResult;
        }

        internal class RecursionItem
        {
            internal int ParentId { get; set; }
            internal int ChildId { get; set; }
            internal bool Flipped { get; set; }
        }
    }
}
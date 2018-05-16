// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Data;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using Microsoft.WindowsAzure.Storage;

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

        public int Run()
        {
            int returnValue = 0;
            try
            {
                FeatureBitDefinition newBit = BuildBit();
                _repo.Add(newBit);
                SystemContext.ConsoleWriteLine("Feature bit added.");
            }
            catch (Exception e)
            {
                returnValue = 1;
                if (e is DataException || e.InnerException is StorageException)
                {
                    returnValue = HandleFeatureBitAlreadyExists(e);
                } else {
                    SystemContext.ConsoleErrorWriteLine(e.ToString());
                }
            }

            return returnValue;
        }

        public FeatureBitDefinition BuildBit()
        {
            var now = SystemContext.Now();
            var username = SystemContext.GetEnvironmentVariable("USERNAME");
            return new FeatureBitDefinition
            {
                Name = _opts.Name,
                RowKey = _opts.Name,
                PartitionKey = "featurebits",
                CreatedDateTime = now,
                LastModifiedDateTime = now,
                CreatedByUser = username,
                LastModifiedByUser = username,
                OnOff = _opts.OnOff,
                ExcludedEnvironments = _opts.ExcludedEnvironments,
                MinimumAllowedPermissionLevel = _opts.PermissionLevel
            };
        }

        private int HandleFeatureBitAlreadyExists(Exception e)
        {
            int returnValue;
            if (e.Message == $"Cannot add. Feature bit with name '{_opts.Name}' already exists." || e.InnerException.Message == "Conflict")
            {
                returnValue = !_opts.Force ? FailWithoutForce() : ForceUpdate();
            }
            else
            {
                returnValue = 1;
                SystemContext.ConsoleErrorWriteLine(e.ToString());
            }

            return returnValue;
        }

        private int ForceUpdate()
        {
            var newBit = BuildBit();
            _repo.Update(newBit);
            SystemContext.ConsoleWriteLine("Feature bit updated.");
            return 0;
        }

        private int FailWithoutForce()
        {
            SystemContext.ConsoleErrorWriteLine(
                $"Feature bit '{_opts.Name}' already exists. Use --force to overwrite existing feature bits.");
            return 1;
        }
    }
}

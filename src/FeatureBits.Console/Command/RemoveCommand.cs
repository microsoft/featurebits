// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Dotnet.FBit.CommandOptions;
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

        public async Task<int> Run()
        {
            int returnValue = 0;
            var name = _opts.Name;
            var def = _repo.GetByNameAsync(name).Result;
            if (def == null)
            {
                SystemContext.ConsoleErrorWriteLine($"Feature bit '{_opts.Name}' could not be found.");
                returnValue = 1;
            }
            else
            {
                await _repo.RemoveAsync(def);
                SystemContext.ConsoleWriteLine("Feature bit removed.");
            }

            return returnValue;
        }
    }
}

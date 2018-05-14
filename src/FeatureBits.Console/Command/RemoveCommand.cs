// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Data;
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

        public int Run()
        {
            int returnValue = 0;
            // TODO 001: Make this work
            //try
            //{
                
            //    // _repo.Remove(_opts.);
            ////    SystemContext.ConsoleWriteLine("Feature bit Removeed.");
            //}
            //catch (DataException e)
            //{
            ////    returnValue = HandleFeatureBitAlreadyExists(e);
            //}
            //catch (Exception e)
            ////{
            ////    returnValue = 1;
            ////    SystemContext.ConsoleErrorWriteLine(e.ToString());
            //}

            return returnValue;
        }
    }
}

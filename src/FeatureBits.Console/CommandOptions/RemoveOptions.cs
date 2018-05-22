// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace Dotnet.FBit.CommandOptions
{
    /// <summary>
    /// Options for the 'remove' command
    /// </summary>
    [Verb(name: "remove", HelpText = "Remove a feature bit from the data store")]
    public class RemoveOptions : CommonOptions
    {
        /// <summary>
        /// Name of the feature bit to remove
        /// </summary>
        [Option('n', Required = true, HelpText = "Name of the feature bit")]
        public string Name { get; set; }
    }
}

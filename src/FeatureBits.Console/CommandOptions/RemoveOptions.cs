// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace Dotnet.FBit.CommandOptions
{
    /// <summary>
    /// Options for the 'remove' command
    /// </summary>
    public class RemoveOptions
    {
        /// <summary>
        /// Connection string to the database storing the feature bits
        /// </summary>
        [Option('c', "connectionstring", Required = true, HelpText = "Connection string to the database storing the feature bits")]
        public string DatabaseConnectionString { get; set; }

        /// <summary>
        /// Name of the feature bit to remove
        /// </summary>
        [Option('n', Required = true, HelpText = "Name of the feature bit")]
        public string Name { get; set; }
    }
}

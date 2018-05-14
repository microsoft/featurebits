// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace Dotnet.FBit.CommandOptions
{
    /// <summary>
    /// Options for the 'add' command
    /// </summary>
    [Verb(name: "add", HelpText = "Add a feature bit to the data store")]
    public class AddOptions
    {
        /// <summary>
        /// Connection string to the database storing the feature bits
        /// </summary>
        [Option('c', "connectionstring", Required = true, HelpText = "Connection string to the database storing the feature bits")]
        public string DatabaseConnectionString { get; set; }

        /// <summary>
        /// Name of the feature bit to add
        /// </summary>
        [Option('n', Required = true, HelpText = "Name of the feature bit")]
        public string Name { get; set; }

        /// <summary>
        /// Specifies whether the feature bit should be blanket on or off.
        /// </summary>
        [Option('o', HelpText = "Specifies whether the feature bit should be blanket on or off.")]
        public bool OnOff { get; set; }

        /// <summary>
        /// Comma-separated list of environments on which to turn off this feature.
        /// </summary>
        [Option('e', "excluded-environments", HelpText = "Comma-separated list of environments on which to turn off this feature.")]
        public string ExcludedEnvironments { get; set; }

        /// <summary>
        /// Minimum permission level required for this feature to be turned on. (integer)
        /// </summary>
        [Option('p', "permission-level", HelpText = "Minimum permission level required for this feature to be turned on. (integer)")]
        public int PermissionLevel { get; set; }

        /// <summary>
        /// Specifies whether the feature bit should be blanket on or off.
        /// </summary>
        [Option('f', HelpText = "If the feature bit already exist, overwrite it.")]
        public bool Force { get; set; }
    }
}

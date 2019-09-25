// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace Dotnet.FBit.CommandOptions
{
    /// <summary>
    /// Options for the 'add' command
    /// </summary>
    [Verb(name: "add", HelpText = "Add a feature bit to the data store")]
    public class AddOptions : CommonOptions
    {
        /// <summary>
        /// Name of the feature bit to add
        /// </summary>
        [Option('n', "name", Required = true, HelpText = "Name of the feature bit")]
        public string Name { get; set; }

        /// <summary>
        /// Specifies whether the feature bit should be blanket on or off.
        /// </summary>
        [Option('o', "onoff", HelpText = "Specifies whether the feature bit should be totally on or off. Boolean 'true' and 'false' are the only acceptable values (not case sensitive)")]
        public string OnOff { get; set; }

        /// <summary>
        /// Comma-separated list of environments on which to turn off this feature.
        /// </summary>
        [Option("excluded-environments", HelpText = "Comma-separated list of environments on which to turn off this feature.")]
        public string ExcludedEnvironments { get; set; }

        /// <summary>
        /// Minimum permission level required for this feature to be turned on. (integer)
        /// </summary>
        [Option('m', "minimum-permission-level", HelpText = "Minimum permission level required for this feature to be turned on. (integer)")]
        public int MinimumPermissionLevel { get; set; }

        /// <summary>
        /// Exact permission level required for this feature to be turned on. (integer)
        /// </summary>
        [Option("exact-permission-level", HelpText = "Exact permission level required for this feature to be turned on. (integer)")]
        public int ExactPermissionLevel { get; set; }

        /// <summary>
        /// Comma-separated list of environments on which to turn on this feature.
        /// </summary>
        [Option("included-environments", HelpText = "Comma-separated list of environments on which to turn on this feature.")]
        public string IncludedEnvironments { get; set; }

        /// <summary>
        /// Specifies whether the feature bit should be blanket on or off.
        /// </summary>
        [Option('f', "force", HelpText = "If the feature bit already exist, overwrite it.")]
        public bool Force { get; set; }
    }
}

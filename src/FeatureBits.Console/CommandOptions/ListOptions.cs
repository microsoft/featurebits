// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace Dotnet.FBit.CommandOptions
{
    [Verb(name: "list", HelpText = "shows ID and Name FeatureBitDefinition fields")]
    public class ListOptions : CommonOptions
    {
        /// <summary>
        /// Specifies whether it should display all fields.
        /// </summary>
        [Option('l', "long ", Default = false, HelpText = "shows all FeatureBitDefinition fields")]
        public bool Long { get; set; }
    }
}

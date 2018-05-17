// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace Dotnet.FBit.CommandOptions
{
    /// <summary>
    /// Common options for all commands
    /// </summary>
    public class CommonOptions
    {
        /// <summary>
        /// Connection string to the SQL database storing the feature bits
        /// </summary>
        [Option('c', "sqlconnectionstring", Required = true, HelpText = "Connection string to the database storing the feature bits", SetName = "SQL")]
        public string DatabaseConnectionString { get; set; }

        /// <summary>
        /// Connection string to the Azure Table storing the feature bits
        /// </summary>
        [Option('c', "tableconnectionstring", Required = true, HelpText = "Connection string to the database storing the feature bits", SetName = "ATS")]
        public string AzureTableConnectionString { get; set; }
    }
}
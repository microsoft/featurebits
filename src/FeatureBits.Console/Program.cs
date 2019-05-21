// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CommandLine;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using FeatureBits.Data.AzureTableStorage;
using FeatureBits.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dotnet.FBit
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            List<string> argsAsList = new List<string>(args);
            argsAsList.AddRange(FindUserSecrets());
            return Parser.Default.ParseArguments<GenerateOptions, AddOptions, RemoveOptions, ListOptions>(argsAsList)
                .MapResult(
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts).Result,
                    (AddOptions opts) => RunAddAndReturnExitCode(opts).Result,
                    (RemoveOptions opts) => RunRemoveAndReturnExitCode(opts).Result,
                    (ListOptions opts) => RunListAndReturnExitCode(opts).Result,
                    errs => 1);
        }

        private static async Task<int> RunGenerateAndReturnExitCode(GenerateOptions opts)
        {
            var repo = GetCorrectRepository(opts);
            var cmd = new GenerateCommand(opts, repo, new FileSystem());
            var result = await cmd.RunAsync();
            return !result ? 1 : 0;
        }

        private static async Task<int> RunAddAndReturnExitCode(AddOptions opts)
        {
            var repo = GetCorrectRepository(opts);
            var cmd = new AddCommand(opts, repo);
            int result = await cmd.RunAsync();
            return result;
        }

        private static async Task<int> RunListAndReturnExitCode(ListOptions opts)
        {
            var repo = GetCorrectRepository(opts);
            var cmd = new ListCommand(opts, repo, new ConsoleTableWrapper());
            int result = await cmd.RunAsync();
            return result;
        }

        private static async Task<int> RunRemoveAndReturnExitCode(RemoveOptions opts)
        {
            var repo = GetCorrectRepository(opts);
            var cmd = new RemoveCommand(opts, repo);
            int result = await cmd.RunAsync();
            return result;
        }

        private static IFeatureBitsRepo GetCorrectRepository(CommonOptions opts)
        {
            IFeatureBitsRepo repo;
            bool useTable = string.IsNullOrEmpty(opts.DatabaseConnectionString);
            var dbConnStr = useTable ? opts.AzureTableConnectionString : opts.DatabaseConnectionString;

            if (!useTable)
            {
                System.Diagnostics.Trace.TraceInformation($"Database mode {dbConnStr}");
                DbContextOptionsBuilder<FeatureBitsEfDbContext> options =
                    new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
                options.UseSqlServer(dbConnStr);
                var context = new FeatureBitsEfDbContext(options.Options);
                repo = new FeatureBitsEfRepo(context);
            }
            else
            {
                System.Diagnostics.Trace.TraceInformation($"Azure Table mode {dbConnStr}");
                repo = new FeatureBitsTableStorageRepo(dbConnStr, opts.AzureTableName);
            }

            return repo;
        }

        /// <summary>
        /// Add the user secrets from the project referencing the NuGet package
        /// </summary>
        /// <returns>Additional strings to be considered as program arguments</returns>
        private static IEnumerable<string> FindUserSecrets()
        {
            const string fbitDelimiter = "fbit:";
            List<string> fbitUserSecrets = new List<string>();
            string userSecretsId = ProjectFileHelper.GetUserSecretsId(Environment.CurrentDirectory);
            if (!string.IsNullOrWhiteSpace(userSecretsId))
            {
                ConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddUserSecrets(userSecretsId);
                IConfiguration configuration = builder.Build();
                IConfigurationSection fbitSection = configuration.GetSection("fbit");
                foreach (KeyValuePair<string, string> kvp in fbitSection.AsEnumerable())
                {
                    if (!kvp.Key.StartsWith(fbitDelimiter))
                    {
                        continue;
                    }

                    string argument = kvp.Key.Substring(fbitDelimiter.Length);
                    fbitUserSecrets.AddRange(new[] { argument, kvp.Value });
                }
            }

            return fbitUserSecrets;
        }
    }
}

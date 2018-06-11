// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Threading.Tasks;
using CommandLine;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using FeatureBits.Data.AzureTableStorage;
using FeatureBits.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.FBit
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            List<string> argsAsList = new List<string>(args);
            argsAsList.AddRange(FindUserSecrets());
            return Parser.Default.ParseArguments<GenerateOptions, AddOptions, RemoveOptions>(argsAsList)
                .MapResult(
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts).Result,
                    (AddOptions opts) => RunAddAndReturnExitCode(opts).Result,
                    (RemoveOptions opts) => RunRemoveAndReturnExitCode(opts).Result,
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
                DbContextOptionsBuilder<FeatureBitsEfDbContext> options =
                    new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
                options.UseSqlServer(dbConnStr);
                var context = new FeatureBitsEfDbContext(options.Options);
                repo = new FeatureBitsEfRepo(context);
            }
            else
            {
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
            Process p = new Process();
            p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "dotnet";
            p.StartInfo.Arguments = "user-secrets list";
            p.Start();
            p.WaitForExit();
            string cmdOutput = p.StandardOutput.ReadToEnd();
            string[] userSecrets = cmdOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            List<string> fbitUserSecrets = new List<string>(userSecrets.Length);
            const string equalDelimiter = " = ";
            const string fbitDelimiter = "fbit:";
            foreach (string secret in userSecrets)
            {
                if(!secret.StartsWith(fbitDelimiter))
                {
                    continue;
                }

                int equalDelimiterIndex = secret.IndexOf(equalDelimiter);
                string argument = secret.Substring(fbitDelimiter.Length, equalDelimiterIndex - fbitDelimiter.Length);
                string argumentValue = secret.Substring(equalDelimiterIndex + equalDelimiter.Length);
                fbitUserSecrets.AddRange(new[] { argument, argumentValue});
            }

            return fbitUserSecrets;
        }
    }
}

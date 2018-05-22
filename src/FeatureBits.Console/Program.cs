// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Abstractions;
using System.Threading.Tasks;
using CommandLine;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using FeatureBits.Data.AzureTableStorage;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.FBit
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<GenerateOptions, AddOptions>(args)
                .MapResult(
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts).Result,
                    (AddOptions opts) => RunAddAndReturnExitCode(opts, true).Result,
                    (RemoveOptions opts) => RunRemoveAndReturnExitCode(opts, true).Result,
                    errs => 1);
        }

        private static async Task<int> RunGenerateAndReturnExitCode(GenerateOptions opts)
        {
            var options = GetDbContextOptionsBuilder(opts);

            using (var context = new FeatureBitsEfDbContext(options.Options))
            {
                var repo = new FeatureBitsEfRepo(context);
                var cmd = new GenerateCommand(opts, repo, new FileSystem());
                var result = await cmd.RunAsync();
                return result == false ? 1 : 0;
            }
        }

        private static async Task<int> RunAddAndReturnExitCode(AddOptions opts, bool useTable)
        {
            var dbConnStr = opts.DatabaseConnectionString;
            // TODO - this looks an awful lot like a job for dependency injection
            var repo = GetCorrectRepo(useTable, dbConnStr, opts.AzureStorageTableName);

            var cmd = new AddCommand(opts, repo);
            int result = await cmd.RunAsync();
            return result;
        }

        private static async Task<int> RunRemoveAndReturnExitCode(RemoveOptions opts, bool useTable)
        {
            var dbConnStr = opts.DatabaseConnectionString;
            // TODO - this looks an awful lot like a job for dependency injection
            var repo = GetCorrectRepo(useTable, dbConnStr, opts.AzureStorageTableName);
            var cmd = new RemoveCommand(opts, repo);
            int result = await cmd.RunAsync();
            return result;
        }

        private static IFeatureBitsRepo GetCorrectRepo(bool useTable, string dbConnStr, string tableName)
        {
            IFeatureBitsRepo repo;
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
                repo = new FeatureBitsTableStorageRepo(dbConnStr, tableName);
            }

            return repo;
        }

        private static DbContextOptionsBuilder<FeatureBitsEfDbContext> GetDbContextOptionsBuilder(GenerateOptions opts)
        {
            DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
            options.UseSqlServer(opts.DatabaseConnectionString);
            return options;
        }
    }
}

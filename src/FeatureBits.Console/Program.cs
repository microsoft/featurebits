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
            //var addo = new AddOptions {
            //    DatabaseConnectionString = "DefaultEndpointsProtocol=https;AccountName=mtabletest;AccountKey=Ubb4pX/Lp9NHFWm4uxmj4dRTNMvqlHdH1A6kQ33gC+ZRYNXKu0UK6rFofUQalR7CQzGr4MVS3ezsqjNMlAfkHw==;BlobEndpoint=https://mtabletest.blob.core.windows.net/;QueueEndpoint=https://mtabletest.queue.core.windows.net/;TableEndpoint=https://mtabletest.table.core.windows.net/;FileEndpoint=https://mtabletest.file.core.windows.net/;",
            //    OnOff = true,
            //    Name = "mitchtest",
            //    ExcludedEnvironments = null,
            //    PermissionLevel = 0,
            //    Force = false
            //};

            //RunAddAndReturnExitCode(addo, true).Result;
            
            return Parser.Default.ParseArguments<GenerateOptions, AddOptions>(args)
                .MapResult(
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts).Result,
                    (AddOptions opts) => RunAddAndReturnExitCode(opts, true).Result,
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
            IFeatureBitsRepo repo;
            if (!useTable)
            {
                DbContextOptionsBuilder<FeatureBitsEfDbContext> options =
                    new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
                options.UseSqlServer(opts.DatabaseConnectionString);
                var context = new FeatureBitsEfDbContext(options.Options);
                repo = new FeatureBitsEfRepo(context);
            }
            else
            {
                repo = new FeatureBitsTableStorageRepo(opts.DatabaseConnectionString);
            }
            var cmd = new AddCommand(opts, repo);
            int result = await cmd.Run();
            return result;
        }

        private static DbContextOptionsBuilder<FeatureBitsEfDbContext> GetDbContextOptionsBuilder(GenerateOptions opts)
        {
            DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
            options.UseSqlServer(opts.DatabaseConnectionString);
            return options;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Abstractions;
using CommandLine;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.FBit
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<GenerateOptions, AddOptions>(args)
                .MapResult(
                    (GenerateOptions opts) => RunGenerateAndReturnExitCode(opts),
                    (AddOptions opts) => RunAddeAndReturnExitCode(opts),
                    errs => 1);
        }

        private static int RunGenerateAndReturnExitCode(GenerateOptions opts)
        {
            var options = GetDbContextOptionsBuilder(opts);

            using (var context = new FeatureBitsEfDbContext(options.Options))
            {
                var repo = new FeatureBitsEfRepo(context);
                var cmd = new GenerateCommand(opts, repo, new FileSystem());
                var result = cmd.Run();
                return result == false ? 1 : 0;
            }
        }

        private static int RunAddeAndReturnExitCode(AddOptions opts)
        {
            DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
            options.UseSqlServer(opts.DatabaseConnectionString);

            using (var context = new FeatureBitsEfDbContext(options.Options))
            {
                var repo = new FeatureBitsEfRepo(context);
                var cmd = new AddCommand(opts, repo);
                int result = cmd.Run();
                return result;
            }
        }

        private static DbContextOptionsBuilder<FeatureBitsEfDbContext> GetDbContextOptionsBuilder(GenerateOptions opts)
        {
            DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
            options.UseSqlServer(opts.DatabaseConnectionString);
            return options;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using FeatureBits.Data;
using FeatureBits.Data.EF;
using FeatureBitsData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleTestClient
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private const string ConnectionSecretName = "connStr";

        static void Main()
        {
            BootstrapConfiguration();
            string connStr = _configuration[ConnectionSecretName];

            DbContextOptionsBuilder<FeatureBitsEfDbContext> options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
            options.UseSqlServer(connStr);
            
            using (var context = new FeatureBitsEfDbContext(options.Options))
            {
                var count = context.FeatureBitDefinitions.Count();
                Console.WriteLine(count);    
            }
            
            Console.ReadKey();
        }

        private static void BootstrapConfiguration()
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
            {
                env = "Development";
            }

            var builder = new ConfigurationBuilder();

            if (env == "Development" || env == "LocalDevelopment")
            {
                builder.AddUserSecrets<Program>();
            }

            _configuration = builder.Build();
        }
    }
}

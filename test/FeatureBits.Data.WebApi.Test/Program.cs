// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FeatureBits.Data.WebApi.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
#if NET452
            new WebHostBuilder()
#else
            WebHost.CreateDefaultBuilder(args)
#endif

                .UseStartup<Startup>()
                .Build();
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using FeatureBits.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FeatureBits.Console.Test
{
    public class FeatureBitEfHelper
    {
        public static DbContextOptions<FeatureBitsEfDbContext> GetFakeDbOptions(bool enableSensitiveDataLogging = false)
        {
            var guidDbNameForUniqueness = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<FeatureBitsEfDbContext>()
                .UseInMemoryDatabase(guidDbNameForUniqueness)
#if !NET452
                .EnableSensitiveDataLogging(enableSensitiveDataLogging)
#endif
                .Options;

            return options;
        }

        public static FeatureBitsEfDbContext GetFakeDbContext() => GetFakeDbContext(GetFakeDbOptions());

        public static FeatureBitsEfDbContext GetFakeDbContext(DbContextOptions<FeatureBitsEfDbContext> options)
        {
            return new FeatureBitsEfDbContext(options);
        }
    }
}
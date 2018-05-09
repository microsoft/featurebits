// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace FeatureBits
{
    // TODO: Eliminate this in favor of the FeatureBits.Data package
    public class FeatureBitsDbContext : DbContext
    {
        public FeatureBitsDbContext(DbContextOptions<FeatureBitsDbContext> options) : base(options) { }

        public DbSet<FeatureBitDefinition> FeatureBitDefinitions { get; set; }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace FeatureBits.Data.EF
{
    public class FeatureBitsEfDbContext : DbContext
    {
        public FeatureBitsEfDbContext(DbContextOptions<FeatureBitsEfDbContext> options) : base(options) { }

        public DbSet<FeatureBitEfDefinition> FeatureBitDefinitions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FeatureBitEfDefinition>()
                .HasIndex(col => col.Name)
                .IsUnique()
                .HasName("IX_FeatureBit_Name");
        }
    }
}

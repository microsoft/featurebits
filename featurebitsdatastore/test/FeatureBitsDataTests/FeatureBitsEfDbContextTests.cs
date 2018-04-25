// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using FeatureBitsData;
using FluentAssertions;
using Xunit;

namespace FeatureBitsDataTests
{
    public class FeatureBitsEfDbContextTests : IDisposable
    {
        private readonly FeatureBitsEfDbContext _it;

        public FeatureBitsEfDbContextTests()
        {
            _it = FeatureBitEfHelper.SetupDbContext().Item1;
        }

        public void Dispose()
        {
            _it?.Dispose();
        }

        
        [Fact]
        public void ItCanBeCreated()
        {
            _it.Should().NotBeNull();
        }

        [Fact]
        public void ItHasAFeatureBitDefinitionsDbSet()
        {
            _it.FeatureBitDefinitions.Should().NotBeNull();
        }
    }
}

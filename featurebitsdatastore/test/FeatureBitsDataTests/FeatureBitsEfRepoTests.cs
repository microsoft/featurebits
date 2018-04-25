// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using FeatureBitsData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FeatureBitsDataTests
{
    public class FeatureBitsEfRepoTests : IDisposable
    {
        private readonly FeatureBitsEfRepo _it;
        private readonly FeatureBitsEfDbContext _context;
        private readonly DbContextOptions<FeatureBitsEfDbContext> _options;

        public FeatureBitsEfRepoTests()
        {
            Tuple<FeatureBitsEfDbContext, DbContextOptions<FeatureBitsEfDbContext>> testConfig = FeatureBitEfHelper.SetupDbContext();
            _context = testConfig.Item1;
            _options = testConfig.Item2;
            _it = new FeatureBitsEfRepo(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }



        [Fact]
        public void ItCanBeCreated()
        {
            _it.Should().NotBe(null);
        }

        [Fact]
        public void ItHasADbContext()
        {
            _it.DbContext.Should().NotBeNull();
        }

        [Fact]
        public void ItCanGetAllFeatureBitDefinitions()
        {
            // Arrange
            _context.FeatureBitDefinitions.Add(new FeatureBitDefinition{Name = "item1"});
            _context.FeatureBitDefinitions.Add(new FeatureBitDefinition{Name = "item2"});
            _context.FeatureBitDefinitions.Add(new FeatureBitDefinition{Name = "item3"});
            _context.SaveChanges();

            // Act
            FeatureBitDefinition[] result = _it.GetAll().ToArray();

            // Assert
            result.Length.Should().Be(3);
        }

        [Fact]
        public void ItCanAddFeatureBitDefinitions()
        {
            // Arrange
            var item1 = new FeatureBitDefinition {Name = "item1", CreatedByUser = "foo", LastModifiedByUser = "foo"};

            // Act
            FeatureBitDefinition result = _it.Add(item1);

            // Assert
            result.Name.Should().Be("item1");
            result.Id.Should().NotBe(0);
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                context.FeatureBitDefinitions.Should().Contain(f => f.Name == "item1");
            }
        }

        [Fact]
        public void It_throws_if_you_try_to_add_an_invalid_entity()
        {
            // Arrange
            var item1 = new FeatureBitDefinition {Name = "item1"};

            // Act
            Action action = () => _it.Add(item1);

            // Assert
            action.Should().Throw<InvalidDataException>();
        }
    }
}

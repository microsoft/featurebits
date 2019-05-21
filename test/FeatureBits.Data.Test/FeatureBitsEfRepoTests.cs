// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FeatureBits.Data.EF;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

namespace FeatureBits.Data.Test
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
        public async Task ItCanGetAllIFeatureBitDefinitions()
        {
            // Arrange
            AddThreeDefinitions();

            // Act
            IFeatureBitDefinition[] result = (await _it.GetAllAsync()).ToArray();

            // Assert
            result.Length.Should().Be(3);
        }

        [Fact]
        public async Task ItCanAddIFeatureBitDefinitions()
        {
            // Arrange
            var item1 = new FeatureBitEfDefinition {Name = "item1", CreatedByUser = "foo", LastModifiedByUser = "foo", OnOff = false};

            // Act
            IFeatureBitDefinition result = await _it.AddAsync(item1);

            // Assert
            result.Name.Should().Be("item1");
            result.Id.Should().NotBe(0);
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                context.FeatureBitDefinitions.Should().Contain(f => f.Name == "item1" && f.OnOff == false);
            }
        }

        [Fact]
        public async Task It_throws_if_you_try_to_add_an_invalid_entity()
        {
            // Arrange
            var item1 = new FeatureBitEfDefinition { Name = "item1"};

            // Act

            // Assert
            await Assert.ThrowsAsync<InvalidDataException>(async () => await _it.AddAsync(item1));
        }

        [Fact]
        public async Task It_throws_if_you_try_to_add_an_entity_with_a_duplicate_name()
        {
            // Arrange
            AddThreeDefinitions();
            var item1 = new FeatureBitEfDefinition { Name = "item1", CreatedByUser = "foo", LastModifiedByUser = "foo"};

            // Act

            // Assert
            await Assert.ThrowsAsync<FeatureBitDataException>(async () => await _it.AddAsync(item1));
        }

        [Fact]
        public async Task ItCanUpdateIFeatureBitDefinitions()
        {
            // Arrange
            var entities = AddThreeDefinitions();
            var defToUpdate = entities[1].Entity;
            defToUpdate.AllowedUsers = "Updated Value";

            // Act
            await _it.UpdateAsync(defToUpdate);

            // Assert
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                context.FeatureBitDefinitions.Count().Should().Be(3);
                context.FeatureBitDefinitions.Should().Contain(f => f.AllowedUsers == "Updated Value");
            }
        }

        [Fact]
        public async Task ItCanUpsertIFeatureBitDefinitions()
        {
            // Arrange
            AddThreeDefinitions();
            var defToUpsert = new FeatureBitEfDefinition
            {
                Name = "New feature bit",
                CreatedByUser = "foo",
                LastModifiedByUser = "foo"
            };

            // Act
            await _it.AddAsync(defToUpsert);

            // Assert
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                context.FeatureBitDefinitions.Count().Should().Be(4);
                context.FeatureBitDefinitions.Should().Contain(f => f.Name == "New feature bit");
            }
        }

        [Fact]
        public async Task ItCanRemoveIFeatureBitDefinitions()
        {
            // Arrange
            var entities = AddThreeDefinitions();
            var entityToRemove = entities[1].Entity;

            // Act
            await _it.RemoveAsync(entityToRemove);

            // Assert
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                 context.FeatureBitDefinitions.Count().Should().Be(2);
                 context.FeatureBitDefinitions.Should().NotContain(f => f.Name == "item2");
            }
        }


        [Fact]
        public async Task ItCanGetASpecificIFeatureBitDefinitionByName()
        {
            // Arrange
            AddThreeDefinitions();

            // Act
            IFeatureBitDefinition result = (await _it.GetByNameAsync("item2"));

            // Assert
            result.MinimumAllowedPermissionLevel.Should().Be(10);
        }

        private IList<EntityEntry<FeatureBitEfDefinition>> AddThreeDefinitions()
        {
            var entities = new List<EntityEntry<FeatureBitEfDefinition>>
            {
                _context.FeatureBitDefinitions.Add(new FeatureBitEfDefinition {Name = "item1"}),
                _context.FeatureBitDefinitions.Add(new FeatureBitEfDefinition {Name = "item2", MinimumAllowedPermissionLevel = 10}),
                _context.FeatureBitDefinitions.Add(new FeatureBitEfDefinition {Name = "item3"})
            };
            _context.SaveChanges();

            return entities;
        }
    }
}

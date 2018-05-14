// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
        public void ItCanGetAllFeatureBitDefinitions()
        {
            // Arrange
            AddThreeDefinitions();

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

        [Fact]
        public void It_throws_if_you_try_to_add_an_entity_with_a_duplicate_name()
        {
            // Arrange
            AddThreeDefinitions();
            var item1 = new FeatureBitDefinition {Name = "item1", CreatedByUser = "foo", LastModifiedByUser = "foo"};

            // Act
            Action action = () => { _it.Add(item1); };

            // Assert
            action.Should().Throw<DataException>().WithMessage("Cannot add. Feature bit with name 'item1' already exists.");
        }

        [Fact]
        public void ItCanUpdateFeatureBitDefinitions()
        {
            // Arrange
            var entities = AddThreeDefinitions();
            var defToUpdate = entities[1].Entity;
            defToUpdate.AllowedUsers = "Updated Value";

            // Act
            _it.Update(defToUpdate);

            // Assert
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                context.FeatureBitDefinitions.Count().Should().Be(3);
                context.FeatureBitDefinitions.Should().Contain(f => f.AllowedUsers == "Updated Value");
            }
        }

        [Fact]
        public void ItCanUpsertFeatureBitDefinitions()
        {
            // Arrange
            AddThreeDefinitions();
            var defToUpsert = new FeatureBitDefinition
            {
                Name = "New feature bit",
                CreatedByUser = "foo",
                LastModifiedByUser = "foo"
            };

            // Act
            _it.Update(defToUpsert);

            // Assert
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                context.FeatureBitDefinitions.Count().Should().Be(4);
                context.FeatureBitDefinitions.Should().Contain(f => f.Name == "New feature bit");
            }
        }

        [Fact]
        public void ItCanRemoveFeatureBitDefinitions()
        {
            // Arrange
            var entities = AddThreeDefinitions();
            var entityToRemove = entities[1].Entity;

            // Act
            _it.Remove(entityToRemove);

            // Assert
            using (var context = new FeatureBitsEfDbContext(_options))
            {
                 context.FeatureBitDefinitions.Count().Should().Be(2);
                 context.FeatureBitDefinitions.Should().NotContain(f => f.Name == "item2");
            }
        }


        private IList<EntityEntry<FeatureBitDefinition>> AddThreeDefinitions()
        {
            var entities = new List<EntityEntry<FeatureBitDefinition>>
            {
                _context.FeatureBitDefinitions.Add(new FeatureBitDefinition {Name = "item1"}),
                _context.FeatureBitDefinitions.Add(new FeatureBitDefinition {Name = "item2"}),
                _context.FeatureBitDefinitions.Add(new FeatureBitDefinition {Name = "item3"})
            };
            _context.SaveChanges();

            return entities;
        }
    }
}

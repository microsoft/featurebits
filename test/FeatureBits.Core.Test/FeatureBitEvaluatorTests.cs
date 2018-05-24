// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FeatureBits.Data;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace FeatureBits.Core.Test
{
    public class FeatureBitEvaluatorTests
    {
        [Fact]
        public async Task It_can_evaluate_a_Simple_FeatureBit_to_true() 
        {
            // Arrange
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition {Id = 1, OnOff = true});

            // Act
            var result = it.IsEnabled(1);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task It_can_evaluate_a_Simple_FeatureBit_to_false()
        {
            // Arrange
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition {Id = 0});

            // Act
            var result = it.IsEnabled(0);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task It_throws_when_the_bit_cant_be_found()
        {
            // Arrange
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition {Id = 10});

            // Act / Assert
            Assert.Throws<KeyNotFoundException>(() => { it.IsEnabled(0); });
        }

        [Fact]
        public async Task It_can_evaluate_an_Environment_FeatureBit_to_false()
        {
            // Arrange
            // Excluded Environment trumps "OnOff"
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition { Id = 0, OnOff = true, ExcludedEnvironments = "LocalDevelopment" } );

            // Act
            var result = it.IsEnabled(0);

            // Assert
            result.Should().Be(false);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(20)]
        [InlineData(10)]
        [InlineData(1)]
        public async Task It_can_evaluate_a_Role_FeatureBit_to_true(int permissionLevel)
        {
            // Arrange
            // Excluded Environment trumps "OnOff"
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition { Id = 0, OnOff = false, MinimumAllowedPermissionLevel = permissionLevel});

            // Act
            var result = it.IsEnabled(0, permissionLevel);

            // Assert
            result.Should().Be(true);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(10)]
        [InlineData(1)]
        public async Task It_can_evaluate_a_Role_FeatureBit_to_false(int permissionLevel)
        {
            // Arrange
            // Excluded Environment trumps "OnOff"
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition { Id = 0, OnOff = false, MinimumAllowedPermissionLevel = 30 });

            // Act
            var result = it.IsEnabled(0, permissionLevel);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task It_can_evaluate_a_list_of_Simple_FeatureBits()
        {
            // Arrange            
            var featureBitDefinitions = new List<FeatureBitDefinition>
            {
                new FeatureBitDefinition { Id = 1, OnOff = true },
                new FeatureBitDefinition { Id = 2, OnOff = false },
                new FeatureBitDefinition { Id = 3, OnOff = true },
            };

            // Arrange
            var it = await SetupFeatureBitEvaluator(featureBitDefinitions);

            // Act
            var flags = it.GetEvaluatedFeatureBits(new List<int> { 1, 2, 3 });

            // Assert
            flags[0].Value.Should().Be(true);
            flags[1].Value.Should().Be(false);
            flags[2].Value.Should().Be(true);
        }

        [Fact]
        public async Task It_can_evaluate_a_list_of_Role_FeatureBits_to_false()
        {
            // Arrange
            // MinimumAllowedPermissionLevel trumps "OnOff"
            var featureBitDefinitions = new List<FeatureBitDefinition>
            {
                new FeatureBitDefinition { Id = 1, OnOff = true, MinimumAllowedPermissionLevel = 20 },
                new FeatureBitDefinition { Id = 2, OnOff = true, MinimumAllowedPermissionLevel = 30 },
                new FeatureBitDefinition { Id = 3, OnOff = true, MinimumAllowedPermissionLevel = 40 },
            };

            // Arrange
            var it = await SetupFeatureBitEvaluator(featureBitDefinitions);

            // Act
            var flags = it.GetEvaluatedFeatureBits(new List<int> { 1, 2, 3 }, 10);

            // Assert
            flags[0].Value.Should().Be(false);
            flags[1].Value.Should().Be(false);
            flags[2].Value.Should().Be(false);
        }

        private static async Task<FeatureBitEvaluator> SetupFeatureBitEvaluator(FeatureBitDefinition bitDefinition)
        {           
            return await SetupFeatureBitEvaluator(new List<FeatureBitDefinition> { bitDefinition });
        }

        private static async Task<FeatureBitEvaluator> SetupFeatureBitEvaluator(IList<FeatureBitDefinition> bitDefinitions)
        {
            var response = new List<FeatureBitDefinition>();

            foreach (var bitDefinition in bitDefinitions)
            {
                if (bitDefinition != null)
                {
                    response.Add(bitDefinition);
                }
            }

            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.GetAllAsync().Returns(Task.FromResult((IEnumerable<FeatureBitDefinition>) response));
            var it = await FeatureBitEvaluator.BuildEvaluatorAsync(repo);

            return it;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public async Task It_can_evaluate_an_exact_Role_FeatureBit_to_true()
        {
            int userPermission = 30;
            // Arrange
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition { Id = 0, OnOff = false, ExactAllowedPermissionLevel = 30 });

            // Act
            var result = it.IsEnabled(0, userPermission);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task It_can_evaluate_an_exact_Role_FeatureBit_to_false()
        {
            int userPermission = 30;
            // Arrange
            var it = await SetupFeatureBitEvaluator(new FeatureBitDefinition { Id = 0, OnOff = false, ExactAllowedPermissionLevel = 20 });

            // Act
            var result = it.IsEnabled(0, userPermission);

            // Assert
            result.Should().Be(false);
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

        private static async Task<FeatureBitEvaluator> SetupFeatureBitEvaluator(FeatureBitDefinition bitDefinition)
        {
            var response = new List<FeatureBitDefinition>();
            if (bitDefinition != null)
            {
                response.Add(bitDefinition);
            }
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.GetAllAsync().Returns(Task.FromResult((IEnumerable<FeatureBitDefinition>) response));
            var it = await FeatureBitEvaluator.BuildEvaluatorAsync(repo);

            return it;
        }
    }
}

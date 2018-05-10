// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FeatureBits.Core.Test
{
    public class FeatureBitTests
    {
        [Fact]
        public void It_can_evaluate_a_Simple_FeatureBit_to_true() 
        {
            // Arrange
            var it = SetupFeatureBit(new FeatureBitDefinition {Id = 1, OnOff = true});

            // Act
            var result = it.IsEnabled(1);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void It_can_evaluate_a_Simple_FeatureBit_to_false()
        {
            // Arrange
            var it = SetupFeatureBit(new FeatureBitDefinition {Id = 0});

            // Act
            var result = it.IsEnabled(0);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void It_throws_when_the_bit_cant_be_found()
        {
            // Arrange
            var it = SetupFeatureBit(new FeatureBitDefinition {Id = 10});

            // Act / Assert
            Assert.Throws<KeyNotFoundException>(() => { it.IsEnabled(0); });
        }

        [Fact]
        public void It_has_a_recent_date_on_the_FeatureBitData()
        {
            // Arrange
            var it = new FeatureBitsData();

            // Act / Assert
            it.LastModified.Should().BeAfter(new DateTime(2018, 3, 12));
        }

        [Fact]
        public void It_can_evaluate_an_Environment_FeatureBit_to_false()
        {
            // Arrange
            // Excluded Environment trumps "OnOff"
            var it = SetupFeatureBit(new FeatureBitDefinition { Id = 0, OnOff = true, ExcludedEnvironments = "LocalDevelopment" } );

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
        public void It_can_evaluate_a_Role_FeatureBit_to_true(int permissionLevel)
        {
            // Arrange
            // Excluded Environment trumps "OnOff"
            var it = SetupFeatureBit(new FeatureBitDefinition { Id = 0, OnOff = false, MinimumRole = permissionLevel});

            // Act
            var result = it.IsEnabled(0, permissionLevel);

            // Assert
            result.Should().Be(true);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(10)]
        [InlineData(1)]
        public void It_can_evaluate_a_Role_FeatureBit_to_false(int permissionLevel)
        {
            // Arrange
            // Excluded Environment trumps "OnOff"
            var it = SetupFeatureBit(new FeatureBitDefinition { Id = 0, OnOff = false, MinimumRole = 30 });

            // Act
            var result = it.IsEnabled(0, permissionLevel);

            // Assert
            result.Should().Be(false);
        }

        private static FeatureBitEvaluator SetupFeatureBit(FeatureBitDefinition bitDefinition)
        {
            var featureBitsData = new FeatureBitsData
            {
                Definitions = new List<FeatureBitDefinition>()
            };
            featureBitsData.Definitions.Add(bitDefinition);
            IFeatureBitsReader featureBitsReader = Substitute.For<IFeatureBitsReader>();
            featureBitsReader.ReadFeatureBits().Returns(featureBitsData);
            var it = new FeatureBitEvaluator(featureBitsReader);
            return it;
        }
    }
}

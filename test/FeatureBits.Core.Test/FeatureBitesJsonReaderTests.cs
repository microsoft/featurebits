// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace FeatureBits.Core.Test
{
    public class FeatureBitesJsonReaderTests
    {
        [Fact]
        public void It_can_be_created()
        {
            var it = new FeatureBitsJsonReader("Not a real filename");
            Assert.NotNull(it);
        }

        [Fact]
        public void It_can_ReadFeatureBits()
        {
            // Arrange
            FileSystemContext.ReadAllText = (filename) => @"{""definitions"":[{""name"":""DummyOn"",""id"":22,""onoff"":true,""excludedEnvironments"":""test"",""minimumRole"":30}]}";
            var it = new FeatureBitsJsonReader("Not a real filename");

            // Act
            FeatureBitsData result = it.ReadFeatureBits();

            // Assert
            IList<FeatureBitDefinition> definitions = result.Definitions;
            definitions.Count.Should().Be(1);
            FeatureBitDefinition def = definitions[0];
            def.Name.Should().Be("DummyOn");
            def.ExcludedEnvironments.Should().Be("test");
            def.Id.Should().Be(22);
            def.MinimumRole.Should().Be(30);
            def.OnOff.Should().BeTrue();
        }
    }
}

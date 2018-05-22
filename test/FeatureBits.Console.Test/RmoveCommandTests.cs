// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text;
using Dotnet.FBit;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FeatureBits.Console.Test
{
    public class RmoveCommandTests
    {
        [Fact]
        public void It_can_be_created()
        {
            var opts = new RemoveOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var it = new RemoveCommand(opts, repo);

            it.Should().NotBeNull();
        }


        [Fact]
        public void It_throws_if_the_options_are_null()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => { new RemoveCommand(null, null); };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void It_throws_if_the_repo_is_null()
        {
            var opts = new RemoveOptions();
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => { new RemoveCommand(opts, null); };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void It_should_run_and_remove_a_FeatureBit()
        {
            // Arrange
            var opts = new RemoveOptions{Name = "foo"};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.Remove(Arg.Any<FeatureBitDefinition>());
            
            var it = new RemoveCommand(opts, repo);

            // Act
            var result = it.Run();

            // Assert
            repo.Received().Remove(Arg.Any<FeatureBitDefinition>());
        }

    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Dotnet.FBit;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FeatureBits.Console.Test
{
    public class RemoveCommandTests
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
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.Append(s);
            var opts = new RemoveOptions{Name = "foo"};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.GetByNameAsync("foo").Returns(Task.FromResult((IFeatureBitDefinition)new CommandFeatureBitDefintion { Name = "foo", Id = 5}));
            repo.RemoveAsync(Arg.Any<IFeatureBitDefinition>());
            var it = new RemoveCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(0);
            repo.Received().GetByNameAsync("foo");
            repo.Received().RemoveAsync(Arg.Any<IFeatureBitDefinition>());
            sb.ToString().Should().Be("Feature bit removed.");
        }

        [Fact]
        public void It_should_run_and_show_an_error_if_it_cannot_be_found_()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var opts = new RemoveOptions{Name = "foo"};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.GetByNameAsync("foo").Returns(Task.FromResult( (IFeatureBitDefinition) null ));
            var it = new RemoveCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(1);
            repo.Received().GetByNameAsync("foo");
            sb.ToString().Should().Be("Feature bit 'foo' could not be found.");
        }

    }
}

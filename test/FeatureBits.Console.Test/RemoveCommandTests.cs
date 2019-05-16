// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Core;
using FeatureBits.Data;
using FeatureBits.Data.EF;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var opts = new RemoveOptions { Name = "foo" };
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.GetByNameAsync("foo").Returns(Task.FromResult((IFeatureBitDefinition)new CommandFeatureBitDefintion { Name = "foo", Id = 5 }));
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
            var opts = new RemoveOptions { Name = "foo" };
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.GetByNameAsync("foo").Returns(Task.FromResult((IFeatureBitDefinition)null));
            var it = new RemoveCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(1);
            repo.Received().GetByNameAsync("foo");
            sb.ToString().Should().Be("Feature bit 'foo' could not be found.");
        }

        [Theory]
        [InlineData("test4", 0, "Feature bit removed.")]
        [InlineData("test3", 1, "Feature bit 'test3' has a dependency. Use --force to remove feature bit dependencies.")]
        [InlineData("test3", 0, "Feature bit removed.", true)]
        [InlineData("test8", 1, "Feature bit 'test8' could not be found.")]
        public async Task It_should_run_FeatureBit_remove_and_validate_dependencies(string featureBitName, int expectedResult, string expectedMessage, bool force = false)
        {
            var options = FeatureBitEfHelper.GetFakeDbOptions(true);
            var context = FeatureBitEfHelper.GetFakeDbContext(options);

            var initialSet = new List<FeatureBitEfDefinition>
            {
                new FeatureBitEfDefinition { Id = 1, Name = "test1"},
                new FeatureBitEfDefinition { Id = 2, Name = "test2"},
                new FeatureBitEfDefinition { Id = 3, Name = "test3", DependentIds = "2,1" },
                new FeatureBitEfDefinition { Id = 4, Name = "test4"},
                new FeatureBitEfDefinition { Id = 5, Name = "test5", DependentIds = "3" },
                new FeatureBitEfDefinition { Id = 6, Name = "test6", DependentIds = "3" },
                new FeatureBitEfDefinition { Id = 7, Name = "test7", DependentIds = "6"},
            };
            context.FeatureBitDefinitions.AddRange(initialSet);
            var dbresults = await context.SaveChangesAsync();
            System.Diagnostics.Trace.TraceInformation($"Records {dbresults} persisted");

            var repo = new FeatureBitsEfRepo(context);


            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.Append(s);
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var opts = new RemoveOptions { Name = featureBitName, Force = force };
            var it = new RemoveCommand(opts, repo);

            var results = await it.RunAsync();
            results.Should().Be(expectedResult);

            var entities = await repo.GetAllAsync();
            sb.ToString().Should().Be(expectedMessage);
            if (expectedResult == 0)
            {
                entities?.Count().Should().BeLessThan(initialSet.Count());
                if (force)
                {
                    var definitionId = initialSet.FirstOrDefault(fd => fd.Name == featureBitName)?.Id;
                    var entityDependencyCount = entities.Count((w) =>
                    {
                        var result = false;
                        if (!string.IsNullOrEmpty(w.DependentIds))
                        {
                            var dependency = w.DependentIds.SplitToInts();
                            result = (dependency.Any(id => id == definitionId));
                        }
                        return result;
                    });
                    entityDependencyCount.Should().Be(0);
                }
            }
            else
            {
                entities?.Count().Should().Be(initialSet.Count());
            }
        }
    }
}

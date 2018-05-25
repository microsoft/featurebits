// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
    public class AddCommandTests
    {
        [Fact]
        public void It_can_be_created()
        {
            var opts = new AddOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var it = new AddCommand(opts, repo);

            it.Should().NotBeNull();
        }

        [Fact]
        public void It_throws_if_the_options_are_null()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => { new AddCommand(null, null); };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void It_throws_if_the_repo_is_null()
        {
            var opts = new AddOptions();
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => { new AddCommand(opts, null); };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void It_should_run_and_create_a_FeatureBit()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.Append(s);
            var bit = new FeatureBitDefinition {Name = "foo"};
            var opts = new AddOptions{Name = "foo"};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.AddAsync(bit).Returns(Task.FromResult(bit));
            
            var it = new AddCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(0);
            sb.ToString().Should().Be("Feature bit added.");
        }

        [Fact]
        public void It_should_write_errors_to_the_console_if_theres_an_error()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var opts = new AddOptions{Name = "foo"};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.AddAsync(Arg.Any<FeatureBitDefinition>()).Returns<Task<FeatureBitDefinition>>(x => throw new Exception("Yow!"));
            
            var it = new AddCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(1);
            sb.ToString().Should().StartWith("System.Exception: Yow!");
        }

        [Fact]
        public void It_can_BuildBit_that_is_valid()
        {
            // Arrange
            DateTime expectedDateTime = new DateTime(1966, 11, 9);
            SystemContext.Now = () => expectedDateTime;
            string expectedUsername = "bar";
            SystemContext.GetEnvironmentVariable = s => expectedUsername;
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var opts = new AddOptions
            {
                Name = "foo",
                OnOff = true,
                ExcludedEnvironments = "QA,Production",
                MinimumPermissionLevel = 20
            };
            var repo = Substitute.For<IFeatureBitsRepo>();
            
            var it = new AddCommand(opts, repo);

            // Act
            var result = it.BuildBit();

            // Assert
            result.CreatedDateTime.Should().Be(expectedDateTime);
            result.LastModifiedDateTime.Should().Be(expectedDateTime);
            result.CreatedByUser.Should().Be(expectedUsername);
            result.LastModifiedByUser.Should().Be(expectedUsername);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(result, new ValidationContext(result), validationResults, true);
            validationResults.Count.Should().Be(0);
        }

        [Fact]
        public void It_should_run_and_throw_by_default_if_the_featurebit_name_already_exists()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var opts = new AddOptions{Name = "foo"};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.AddAsync(Arg.Any<FeatureBitDefinition>()).Returns<Task<FeatureBitDefinition>>(x => throw new DataException("Cannot add. Feature bit with name 'foo' already exists."));

            var it = new AddCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(1);
            sb.ToString().Should().Be("Feature bit 'foo' already exists. Use --force to overwrite existing feature bits.");
        }

        [Fact]
        public void It_should_run_and_throw_by_default_if_some_other_DataException_occurs_on_add()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var opts = new AddOptions{Name = "foo"};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.AddAsync(Arg.Any<FeatureBitDefinition>()).Returns<Task<FeatureBitDefinition>>(x => throw new DataException("Some random DataException."));

            var it = new AddCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(1);
            sb.ToString().Should().StartWith("System.Data.DataException: Some random DataException.");
        }

        [Fact]
        public void It_should_run_and_update_a_FeatureBit_if_it_already_exists_and_force_is_specified()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.Append(s);
            var opts = new AddOptions{Name = "foo", Force = true};
            var repo = Substitute.For<IFeatureBitsRepo>();
            repo.AddAsync(Arg.Any<FeatureBitDefinition>()).Returns<Task<FeatureBitDefinition>>(x => throw new DataException("Cannot add. Feature bit with name 'foo' already exists."));

            int counter = 0;
            repo.When(x => x.UpdateAsync(Arg.Any<FeatureBitDefinition>())).Do((x => counter++));
            
            var it = new AddCommand(opts, repo);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            sb.ToString().Should().Be("Feature bit updated.");
            result.Should().Be(0);
            counter.Should().Be(1);
        }
    }
}

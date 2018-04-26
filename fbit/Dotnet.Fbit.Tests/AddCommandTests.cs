using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Dotnet.FBit;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBitsData;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Dotnet.Fbit.Tests
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
            repo.Add(bit).Returns(bit);
            
            var it = new AddCommand(opts, repo);

            // Act
            var result = it.Run();

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
            repo.Add(Arg.Any<FeatureBitDefinition>()).Returns(x => throw new Exception("Yow!"));
            
            var it = new AddCommand(opts, repo);

            // Act
            var result = it.Run();

            // Assert
            result.Should().Be(1);
            sb.ToString().Should().StartWith("System.Exception: Yow!");
        }

        [Fact]
        public void It_can_BuildBit_that_is_valid_with_only_required_options()
        {
            // Arrange
            DateTime expectedDateTime = new DateTime(1966, 11, 9);
            SystemContext.Now = () => expectedDateTime;
            string expectedUsername = "bar";
            SystemContext.GetEnvironmentVariable = s => expectedUsername;
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var opts = new AddOptions{Name = "foo"};
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
    }
}

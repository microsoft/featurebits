using System.IO.Abstractions;
using System.Linq;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBitsData;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Dotnet.Fbit.Tests
{
    public class GenerateCommandTests
    {
        [Fact]
        public void It_can_be_created()
        {
            // Arrange
            var opts = new GenerateOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();

            // Act
            var it = new GenerateCommand(opts, repo, filesystem);

            // Assert
            it.Should().NotBeNull();
        }

        [Fact]
        public void It_can_GetBits()
        {
            // Arrange
            var opts = new GenerateOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var it = new GenerateCommand(opts, repo, filesystem);

            repo.GetAll().Returns(info => new[]
            {
                new FeatureBitDefinition{Name = "foo", Id = 1}, 
                new FeatureBitDefinition{Name = "bar", Id = 2}, 
                new FeatureBitDefinition{Name = "bat", Id = 3}, 
            } );

            // Act
            (string Name, int Id)[] result = it.GetBits().ToArray();

            // Assert
            result[0].Name.Should().Be("foo");
            result[0].Id.Should().Be(1);
            result[2].Name.Should().Be("bat");
            result[2].Id.Should().Be(3);
        }

        
        [Fact]
        public void It_can_GetOutpuFileInfo()
        {
            // Arrange
            var opts = new GenerateOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var it = new GenerateCommand(opts, repo, filesystem);
            var fileinfo = Substitute.For<FileInfoBase>();

            filesystem.FileInfo.FromFileName(Arg.Any<string>()).Returns(fileinfo);

            // Act
            FileInfoBase result = it.GetOutputFileInfo();

            // Assert
            result.Should().NotBeNull();
            result.Directory.Should().Be("foo"); 
        }

        //[Theory]
        //[InlineData("foo")]
        //public void RunTests(string foo)
        //{
        //    // Arrange
        //    var opts = new GenerateOptions();
        //    var repo = Substitute.For<IFeatureBitsRepo>();
        //    var filesystem = Substitute.For<IFileSystem>();

        //    // Act
        //    var it = new GenerateCommand(opts, repo, filesystem);

        //    // Assert
        //    it.Should().NotBeNull();
        //}
    }
}

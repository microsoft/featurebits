// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Core;
using FeatureBits.Data;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FeatureBits.Console.Test
{
    public class GenerateCommandTests
    {
        readonly IFeatureBitDefinition[] _featureBitDefinitions = {
            new CommandFeatureBitDefintion {Name = "foo", Id = 1},
            new CommandFeatureBitDefintion {Name = "bar", Id = 2},
            new CommandFeatureBitDefintion {Name = "bat", Id = 3},
        };

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
        public async Task It_can_GetBits()
        {
            // Arrange
            var opts = new GenerateOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var it = new GenerateCommand(opts, repo, filesystem);

            repo.GetAllAsync().Returns(info => _featureBitDefinitions);

            // Act
            (string Name, int Id)[] result = (await it.GetBits()).ToArray();

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

        }

        [Fact]
        public void It_returns_false_with_error_message_if_it_cannotOverwriteFile()
        {
            // Arrange
            var opts = new GenerateOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var fileinfo = Substitute.For<FileInfoBase>();
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            fileinfo.Exists.Returns(true);
            filesystem.FileInfo.FromFileName(Arg.Any<string>()).Returns(fileinfo);

            IEnumerable<(string, int)> input = _featureBitDefinitions.Select(x => (x.Name, x.Id));

            var it = new GenerateCommand(opts, repo, filesystem);

            // Act
            var result = it.WriteDataToFile(input, "");

            // Assert
            result.Should().BeFalse();
            sb.ToString().Should().Be("Output file already exists.");
        }

        [Fact]
        public void It_returns_false_if_WriteDataToFile_cannot_find_csproj_and_no_namespace_given()
        {
            // Arrange
            var opts = new GenerateOptions();
            IEnumerable<(string, int)> input = _featureBitDefinitions.Select(x => (x.Name, x.Id));

            // Arrange Mocks
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();

            // Arrange IT
            var it = new GenerateCommand(opts, repo, filesystem);

            // Act
            var result = it.WriteDataToFile(input, null);

            // Assert
            result.Should().BeFalse();
            sb.ToString().Should().Contain("No csproj file found for default namespace. Please specify a namespace as a command argument.");
            sb.ToString().Should().Contain("Could not find default namespace for .CSPROJ");
        }

        [Fact]
        public void It_can_WriteDataToFile()
        {
            // Arrange
            using (var ms = new MemoryStream())
            {
                var opts = new GenerateOptions();
                IEnumerable<(string, int)> input = _featureBitDefinitions.Select(x => (x.Name, x.Id));

                // Arrange Mocks
                var sb = new StringBuilder();
                SystemContext.ConsoleWriteLine = s => sb.Append(s);
                var repo = Substitute.For<IFeatureBitsRepo>();
                var filesystem = Substitute.For<IFileSystem>();
                var directory = Substitute.For<DirectoryInfoBase>();
                var csprojFile = Substitute.For<FileInfoBase>();
                var outputFile = Substitute.For<FileInfoBase>();

                filesystem.FileInfo.FromFileName(Arg.Any<string>()).Returns(outputFile);
                directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] { csprojFile });
                csprojFile.OpenRead().Returns(info => StringToStream("Sorry no namespace here"));
                csprojFile.Name.Returns("bar.csproj");
                filesystem.Path.GetFileNameWithoutExtension("bar.csproj").Returns("bar");
                csprojFile.Directory.Returns(directory);
                outputFile.Directory.Returns(directory);
                outputFile.FullName.Returns("Features.cs");
                // ReSharper disable once AccessToDisposedClosure
                outputFile.CreateText().Returns(info => new StreamWriter(ms));


                // Arrange IT
                var it = new GenerateCommand(opts, repo, filesystem);

                // Act
                var result = it.WriteDataToFile(input, null);

                // Assert
                result.Should().BeTrue();
                sb.ToString().Should().Be("Feature bit enum successfully written to Features.cs.");
                it.FileContent.ToString().Should().Contain("namespace bar");
            }
        }

        [Fact]
        public async Task It_can_Run()
        {
            // Arrange
            using (var ms = new MemoryStream())
            {
                var opts = new GenerateOptions();

                // Arrange Mocks
                var sb = new StringBuilder();
                SystemContext.ConsoleWriteLine = s => sb.Append(s);
                var repo = Substitute.For<IFeatureBitsRepo>();
                var filesystem = Substitute.For<IFileSystem>();
                var directory = Substitute.For<DirectoryInfoBase>();
                var csprojFile = Substitute.For<FileInfoBase>();
                var outputFile = Substitute.For<FileInfoBase>();

                repo.GetAllAsync().Returns(_featureBitDefinitions);
                filesystem.FileInfo.FromFileName(Arg.Any<string>()).Returns(outputFile);
                directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] { csprojFile });
                csprojFile.OpenRead().Returns(info => StringToStream("Sorry no namespace here"));
                csprojFile.Name.Returns("bar.csproj");
                filesystem.Path.GetFileNameWithoutExtension("bar.csproj").Returns("bar");
                csprojFile.Directory.Returns(directory);
                outputFile.Directory.Returns(directory);
                outputFile.FullName.Returns("Features.cs");
                // ReSharper disable once AccessToDisposedClosure
                outputFile.CreateText().Returns(info => new StreamWriter(ms));


                // Arrange IT
                var it = new GenerateCommand(opts, repo, filesystem);

                // Act
                var result = await it.RunAsync();

                // Assert
                result.Should().BeTrue();
                sb.ToString().Should().Be("Feature bit enum successfully written to Features.cs.");
                it.FileContent.ToString().Should().Contain("namespace bar");
            }
        }

        private static Stream StringToStream(string stringToStream)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(stringToStream);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}

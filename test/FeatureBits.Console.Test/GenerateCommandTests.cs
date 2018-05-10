// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
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
    public class GenerateCommandTests
    {
        readonly FeatureBitDefinition[] _featureBitDefinitions = {
            new FeatureBitDefinition {Name = "foo", Id = 1},
            new FeatureBitDefinition {Name = "bar", Id = 2},
            new FeatureBitDefinition {Name = "bat", Id = 3},
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
        public void It_can_GetBits()
        {
            // Arrange
            var opts = new GenerateOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var it = new GenerateCommand(opts, repo, filesystem);

            repo.GetAll().Returns(info => _featureBitDefinitions);

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
        public void It_throws_FileNotFoundException_if_GetDefaultNamespace_cannot_find_csproj()
        {
            // Arrange
            var opts = new GenerateOptions();

            // Arrange Mocks
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var directory = Substitute.For<DirectoryInfoBase>();
            directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new FileInfoBase[] { });

            // Arrange IT
            var it = new GenerateCommand(opts, repo, filesystem);

            // Act
            Action act = () => it.GetDefaultNamespace(directory);
            

            // Assert
            act.Should().Throw<FileNotFoundException>();
            sb.ToString().Should().Be("No csproj file found for default namespace. Please specify a namespace as a command argument.");
        }

        [Fact]
        public void It_can_GetDefaultNamespace_even_if_it_finds_too_many_csproj_files()
        {
            // Arrange
            var opts = new GenerateOptions();

            // Arrange Mocks
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.Append(s);
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var directory = Substitute.For<DirectoryInfoBase>();
            var result1 = Substitute.For<FileInfoBase>();
            var result2 = Substitute.For<FileInfoBase>();
            var filecontent = "<RootNamespace>Foo</RootNamespace>";
            directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] { result1, result2 });
            result1.OpenRead().Returns(info => StringToStream(filecontent));

            // Arrange IT
            var it = new GenerateCommand(opts, repo, filesystem);

            // Act
            var result = it.GetDefaultNamespace(directory);

            // Assert
            result.Should().Be("Foo");
            sb.ToString().Should().Be("Multiple csproj files found for namespace, using the first one.");
        }

        [Fact]
        public void It_can_GetDefaultNamespace_even_if_the_rootNamespace_is_not_found_in_csproj()
        {
            // Arrange
            var opts = new GenerateOptions();

            // Arrange Mocks
            var repo = Substitute.For<IFeatureBitsRepo>();
            var filesystem = Substitute.For<IFileSystem>();
            var directory = Substitute.For<DirectoryInfoBase>();
            var result1 = Substitute.For<FileInfoBase>();
            var filecontent = "Sorry no namespace here";
            directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] { result1  });
            result1.OpenRead().Returns(info => StringToStream(filecontent));
            result1.Name.Returns("bar.csproj");
            filesystem.Path.GetFileNameWithoutExtension("bar.csproj").Returns("bar");

            // Arrange IT
            var it = new GenerateCommand(opts, repo, filesystem);

            // Act
            var result = it.GetDefaultNamespace(directory);

            // Assert
            result.Should().Be("bar");
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
                directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] {csprojFile});
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
        public void It_can_Run()
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

                repo.GetAll().Returns(info => _featureBitDefinitions);
                filesystem.FileInfo.FromFileName(Arg.Any<string>()).Returns(outputFile);
                directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] {csprojFile});
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
                var result = it.Run();

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

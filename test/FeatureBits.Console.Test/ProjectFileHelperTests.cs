// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Dotnet.FBit;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FeatureBits.Console.Test
{
    public class ProjectFileHelperTests
    {
        [Fact]
        public void It_throws_FileNotFoundException_if_GetDefaultNamespace_cannot_find_csproj()
        {
            // Arrange Mocks
            var sb = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => sb.Append(s);
            var directory = Substitute.For<DirectoryInfoBase>();
            directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new FileInfoBase[] { });
            
            // Act
            Action act = () => ProjectFileHelper.GetDefaultNamespace(directory);

            // Assert
            act.Should().Throw<FileNotFoundException>();
            sb.ToString().Should().Be("No csproj file found for default namespace. Please specify a namespace as a command argument.");
        }

        [Fact]
        public void It_can_GetDefaultNamespace_even_if_it_finds_too_many_csproj_files()
        {
            // Arrange Mocks
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.Append(s);
            var directory = Substitute.For<DirectoryInfoBase>();
            var result1 = Substitute.For<FileInfoBase>();
            var result2 = Substitute.For<FileInfoBase>();
            var filecontent = "<RootNamespace>Foo</RootNamespace>";
            directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] { result1, result2 });
            result1.OpenRead().Returns(info => StringToStream(filecontent));

            // Act
            var result = ProjectFileHelper.GetDefaultNamespace(directory);

            // Assert
            result.Should().Be("Foo");
            sb.ToString().Should().Be("Multiple csproj files found for namespace, using the first one.");
        }

        [Fact]
        public void It_can_GetDefaultNamespace_even_if_the_rootNamespace_is_not_found_in_csproj()
        {
            // Arrange Mocks
            var filesystem = Substitute.For<IFileSystem>();
            var directory = Substitute.For<DirectoryInfoBase>();
            var result1 = Substitute.For<FileInfoBase>();
            var filecontent = "Sorry no namespace here";
            directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly).Returns(info => new[] { result1 });
            result1.OpenRead().Returns(info => StringToStream(filecontent));
            result1.Name.Returns("bar.csproj");
            filesystem.Path.GetFileNameWithoutExtension("bar.csproj").Returns("bar");

            // Act
            var result = ProjectFileHelper.GetDefaultNamespace(directory);

            // Assert
            result.Should().Be("bar");
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

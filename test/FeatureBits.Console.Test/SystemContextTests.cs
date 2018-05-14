// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text;
using Dotnet.FBit;
using Xunit;

namespace Dotnet.Fbit.Tests
{
    public class SystemContextTests
    {
        [Fact]
        public void ItShouldBeAbleToUseASubstituteForConsoleDotWriteLine()
        {
            // Arrange
            var builder = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => builder.Append(s);

            // Act
            SystemContext.ConsoleWriteLine("foo" + Environment.NewLine);

            // Assert
            Assert.Equal("foo" + Environment.NewLine, builder.ToString());
            
            // Afterwards
            SystemContext.ConsoleWriteLine = null;
        }

        [Fact]
        public void ItShouldBeAbleToUseASubstituteForConsoleDotErrorDotWriteLine()
        {
            // Arrange
            var builder = new StringBuilder();
            SystemContext.ConsoleErrorWriteLine = s => builder.Append(s);

            // Act
            SystemContext.ConsoleErrorWriteLine("foo" + Environment.NewLine);

            // Assert
            Assert.Equal("foo" + Environment.NewLine, builder.ToString());
            
            // Afterwards
            SystemContext.ConsoleErrorWriteLine = null;
        }
    }
}

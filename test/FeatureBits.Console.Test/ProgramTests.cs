// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using Dotnet.FBit;
using Xunit;

namespace FeatureBits.Console.Test
{
    public class ProgramTests
    {
        [Fact]
        public void ItReturns1OnInvalidParams()
        {
            // arrange 
            var builder = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => builder.Append(s);
            SystemContext.ConsoleErrorWriteLine = s => builder.Append(s);
            var args = "foo";

            // Act
            var result = Program.Main(args.Split(' '));

            // assert
            Assert.Equal(1, result);
        }
    }
}

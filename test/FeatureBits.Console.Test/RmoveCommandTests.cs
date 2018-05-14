// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FeatureBits.Console.Test
{
    public class RmoveCommandTests
    {
        [Fact]
        public void It_can_be_created()
        {
            var opts = new RemoveOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var it = new RemoveCommand(opts, repo);

            it.Should().NotBeNull();
        }


        // TODO: What happens when the DB doesn't exist?
    }
}

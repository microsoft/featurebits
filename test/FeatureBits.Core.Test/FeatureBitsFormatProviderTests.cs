// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using FluentAssertions;
using Xunit;

namespace FeatureBits.Core.Test
{
    public class FeatureBitsFormatProviderTests
    {
        [Fact]
        public void It_should_return_Enum()
        {
            var provider = new FeatureBitFormatProvider();
            provider.GetFormat(typeof(Enum)).Should().Be(typeof(Enum));
        }
    }
}

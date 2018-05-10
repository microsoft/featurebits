// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace FeatureBits.Core
{
    /// <summary>
    /// This is the interface used to describe how we read feature bits from the data store and can be used for mocking purposes in unit testing.
    /// </summary>
    // TODO: Eliminate this in favor of the FeatureBits.Data package
    public interface IFeatureBitsReader
    {
        FeatureBitsData ReadFeatureBits();
    }
}

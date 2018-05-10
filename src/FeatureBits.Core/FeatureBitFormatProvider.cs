// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace FeatureBits.Core
{
    /// <summary>
    /// This class allows us to do a comparison against the integer, Id, from the Feature Bits data store against 
    /// either another integer or against the Features.cs enum that can be generated from the CLI 'generate' command.
    /// </summary>
    public class FeatureBitFormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            return typeof(Enum);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace FeatureBits.Core
{
    public class FeatureBitFormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            return typeof(Enum);
        }
    }
}

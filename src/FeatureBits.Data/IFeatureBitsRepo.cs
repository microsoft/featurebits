// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace FeatureBitsData
{
    public interface IFeatureBitsRepo
    {
        IEnumerable<FeatureBitDefinition> GetAll();
        FeatureBitDefinition Add(FeatureBitDefinition definition);
        void Update(FeatureBitDefinition definition);
    }
}

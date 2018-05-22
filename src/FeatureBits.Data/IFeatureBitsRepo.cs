// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureBits.Data
{
    public interface IFeatureBitsRepo
    {
        /// <summary>
        /// Get all feature bit definitions
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FeatureBitDefinition>> GetAllAsync();

        /// <summary>
        /// Add a new feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to add.</param>
        /// <returns>The completed new feature bit definition</returns>
        Task<FeatureBitDefinition> AddAsync(FeatureBitDefinition definition);

        /// <summary>
        /// Update an existing feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to update.</param>
        Task UpdateAsync(FeatureBitDefinition definition);

        /// <summary>
        /// Remove an existing feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to remove.</param>
        Task RemoveAsync(FeatureBitDefinition definition);
    }
}

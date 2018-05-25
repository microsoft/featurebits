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
        /// <returns>Task that gets all feature bit definitions upon completion</returns>
        Task<IEnumerable<IFeatureBitDefinition>> GetAllAsync();

        /// <summary>
        /// Gets a single feature bit definition by its name
        /// </summary>
        /// <returns>Task that gets the feature bit with the matching Name upon completion, or null if none foound.</returns>
        Task<IFeatureBitDefinition> GetByNameAsync(string featureBitName);

        /// <summary>
        /// Add a new feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to add.</param>
        /// <returns>The completed new feature bit definition</returns>
        Task<IFeatureBitDefinition> AddAsync(IFeatureBitDefinition definition);

        /// <summary>
        /// Update an existing feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to update.</param>
        Task UpdateAsync(IFeatureBitDefinition definition);

        /// <summary>
        /// Remove an existing feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to remove.</param>
        Task RemoveAsync(IFeatureBitDefinition definition);
    }
}

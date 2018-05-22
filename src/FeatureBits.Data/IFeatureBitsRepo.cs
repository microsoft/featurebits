// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace FeatureBits.Data
{
    public interface IFeatureBitsRepo
    {
        /// <summary>
        /// Get all feature bit definitions
        /// </summary>
        /// <returns>List of feture bit definitions</returns>
        IEnumerable<FeatureBitDefinition> GetAll();

        /// <summary>
        /// Gets a single feature bit definition by its name
        /// </summary>
        /// <returns>The feature bit with the matching Name, or throws exeption if none foound</returns>
        IEnumerable<FeatureBitDefinition> GetByName(string featureBitName);

        /// <summary>
        /// Add a new feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to add.</param>
        /// <returns>The completed new feature bit definition</returns>
        FeatureBitDefinition Add(FeatureBitDefinition definition);

        /// <summary>
        /// Update an existing feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to update.</param>
        void Update(FeatureBitDefinition definition);

        /// <summary>
        /// Remove an existing feature bit definition
        /// </summary>
        /// <param name="definition">The feature bit definition to remove.</param>
        void Remove(FeatureBitDefinition definition);
    }
}

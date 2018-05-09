// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace FeatureBits
{
    public interface IFeatureBitEvaluator
    {
        /// <summary>
        /// Determine if a feature should be enabled or disabled
        /// </summary>
        /// <param name="feature">Feature to be chedked</param>
        /// <returns>True if the feature is enabled.</returns>
        bool IsEnabled<T>(T feature) where T : struct, IConvertible;

        /// <summary>
        /// Determine if a feature should be enabled or disabled
        /// </summary>
        /// <param name="feature">Feature to be chedked</param>
        /// <param name="currentPermissionLevel">The permission level of the current user</param>
        /// <typeparam name="T">An enumeration or an integer</typeparam>
        /// <returns>True if the feature is enabled.</returns>
        bool IsEnabled<T>(T feature, int currentPermissionLevel) where T : struct, IConvertible;

        /// <summary>
        /// Gets a copy of the list of feature bits from the data store
        /// </summary>
        IList<FeatureBitDefinition> Definitions { get; }
    }
}

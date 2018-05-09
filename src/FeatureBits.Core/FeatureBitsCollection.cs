// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace FeatureBits
{
    /// <summary>
    /// This class contains overall information on all defined feature bits.
    /// </summary>
    public class FeatureBitsData
    {
        /// <summary>
        /// This collection lists all the feature bits.
        /// </summary>
        public IList<FeatureBitDefinition> Definitions { get; set; }

        /// <summary>
        /// The last date and time the data store (currently just a file) was modified
        /// </summary>
        // TODO: Pull from Data Store
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Will be dynamically pulled from file in the future.")]
        public DateTime LastModified => new DateTime(2018, 3, 12, 14, 17, 0);
    }
}

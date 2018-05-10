// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace FeatureBits.Core
{
    /// <summary>
    /// This represents the data structure used to define Feature BitsData
    /// </summary>
    // TODO: Eliminate this in favor of the FeatureBits.Data package
    public class FeatureBitDefinition
    {
        /// <summary>
        /// The unique ID of the feature bit
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The unique name of the feature bit
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If no other rules are set on the feature bit, this attribute tells whether the feature bit should be on or off.
        /// </summary>
        public bool OnOff { get; set; }

        /// <summary>
        /// Comma separated list of environments in which the feature should be turned OFF.
        /// </summary>
        public string ExcludedEnvironments { get; set; }

        /// <summary>
        /// The minimum allowed Role for this feature
        /// </summary>
        public int MinimumRole { get; set; }
    }
}


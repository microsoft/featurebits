// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace FeatureBits.Data
{
    /// <summary>
    /// This represents the data structure used to define Feature BitsData
    /// </summary>
    public class FeatureBitDefinition
    {
        /// <summary>
        /// The unique ID of the feature bit
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The unique name of the feature bit
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// If no other rules are set on the feature bit, this attribute tells whether the feature bit should be turned on or off.
        /// Since bools default to false, the feature would default to off.
        /// </summary>
        public bool OnOff { get; set; }

        /// <summary>
        /// Comma-separated list of environments in which the feature should be turned OFF.
        /// Feature-bit check must pass in the current environment. If no environment is passed in, the feature would be turned ON since no environment would 
        /// be excluded.
        /// </summary>
        [MaxLength(300)]
        public string ExcludedEnvironments { get; set; }

        /// <summary>
        /// The minimum allowed permission level for this feature to be enabled. Permissions are cumulative. A lower permission means lower access.
        /// </summary>
        public int MinimumAllowedPermissionLevel { get; set; }

        /// <summary>
        /// Comma-separated list of users for which the feature should be enabled. If it's not NULL it will be checked against the user for which the request 
        /// is being performed.
        /// </summary>
        [MaxLength(2048)]
        public string AllowedUsers { get; set; }

        /// <summary>
        /// The date and time the feature bit definition was created.
        /// </summary>
        [Required]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// The user that created the feature bit.
        /// </summary>
        [Required, MaxLength(100)]
        public string CreatedByUser { get; set; }

        /// <summary>
        /// The date and time the feature bit was last modified
        /// </summary>
        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        /// <summary>
        /// The user that last modified the feature bit.
        /// </summary>
        [Required, MaxLength(100)]
        public string LastModifiedByUser { get; set; }
    }
}

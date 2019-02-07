using System;

namespace FeatureBits.Data
{
    public interface IFeatureBitDefinition
    {
        /// <summary>
        /// The unique ID of the feature bit
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The unique name of the feature bit
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// If no other rules are set on the feature bit, this attribute tells whether the feature bit should be turned on or off.
        /// Since bools default to false, the feature would default to off.
        /// </summary>
        bool OnOff { get; set; }

        /// <summary>
        /// Comma-separated list of environments in which the feature should be turned OFF.
        /// Feature-bit check must pass in the current environment. If no environment is passed in, the feature would be turned ON since no environment would 
        /// be excluded.
        /// </summary>
        string ExcludedEnvironments { get; set; }

        /// <summary>
        /// The minimum allowed permission level for this feature to be enabled. Permissions are cumulative. A lower permission means lower access.
        /// </summary>
        int MinimumAllowedPermissionLevel { get; set; }

        /// <summary>
        /// The exact allowed permission level for this feature to be enabled.
        /// This takes precedent over MinimumAllowedPermissionLevel
        /// </summary>
        int? ExactAllowedPermissionLevel { get; set; }

        /// <summary>
        /// Comma-separated list of users for which the feature should be enabled. If it's not NULL it will be checked against the user for which the request 
        /// is being performed.
        /// </summary>
        string AllowedUsers { get; set; }

        /// <summary>
        /// The date and time the feature bit definition was created.
        /// </summary>
        DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// The user that created the feature bit.
        /// </summary>
        string CreatedByUser { get; set; }

        /// <summary>
        /// The date and time the feature bit was last modified
        /// </summary>
        DateTime LastModifiedDateTime { get; set; }

        /// <summary>
        /// The user that last modified the feature bit.
        /// </summary>
        string LastModifiedByUser { get; set; }


        /// <summary>
        /// Update the feature bit entity
        /// </summary>
        /// <param name="newEntity">Update values</param>
        void Update(IFeatureBitDefinition newEntity);
    }
}
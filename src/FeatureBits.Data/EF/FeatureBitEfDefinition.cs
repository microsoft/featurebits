using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FeatureBits.Data.EF
{
    public class FeatureBitEfDefinition: IIFeatureBitDefinition
    {
        /// <summary>
        /// <see cref="IIFeatureBitDefinition.Id"/>
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.Name"/>
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.OnOff"/>
        /// </summary>
        public bool OnOff { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.ExcludedEnvironments"/>
        /// </summary>
        [MaxLength(300)]
        public string ExcludedEnvironments { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.MinimumAllowedPermissionLevel"/>
        /// </summary>
        public int MinimumAllowedPermissionLevel { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.ExactAllowedPermissionLevel"/>
        /// </summary>
        public int ExactAllowedPermissionLevel { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.AllowedUsers"/>
        /// </summary>
        [MaxLength(2048)]
        public string AllowedUsers { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.CreatedDateTime"/>
        /// </summary>
        [Required]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.CreatedByUser"/>
        /// </summary>
        [Required, MaxLength(100)]
        public string CreatedByUser { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.LastModifiedDateTime"/>
        /// </summary>
        [Required]
        public DateTime LastModifiedDateTime { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.LastModifiedByUser"/>
        /// </summary>
        [Required, MaxLength(100)]
        public string LastModifiedByUser { get; set; }

        /// <summary>
        /// <see cref="IIFeatureBitDefinition.Update"/>
        /// </summary>
        public void Update(IIFeatureBitDefinition newEntity)
        {
            AllowedUsers = newEntity.AllowedUsers;
            LastModifiedByUser = newEntity.LastModifiedByUser;
            ExcludedEnvironments = newEntity.ExcludedEnvironments;
            LastModifiedDateTime = newEntity.LastModifiedDateTime;
            MinimumAllowedPermissionLevel = newEntity.MinimumAllowedPermissionLevel;
            OnOff = newEntity.OnOff;
        }
    }
}

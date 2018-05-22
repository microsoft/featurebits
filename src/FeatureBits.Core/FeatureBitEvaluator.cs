// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FeatureBits.Data;

namespace FeatureBits.Core
{
    /// <summary>
    /// This static class allows the user of the class to determine if a particular feature should be turned on or off.
    /// </summary>
    public class FeatureBitEvaluator : IFeatureBitEvaluator
    {
        private readonly IFeatureBitsRepo _repo;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="reader">Object used to read the Feature Bits</param>
        internal FeatureBitEvaluator(IFeatureBitsRepo reader) => _repo = reader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static async Task<FeatureBitEvaluator> BuildEvaluatorAsync(IFeatureBitsRepo reader) => new FeatureBitEvaluator(reader)
        {
            Definitions = (await reader.GetAllAsync()).ToList()
        };

        /// <summary>
        /// Feature Bits Data/Definitions
        /// </summary>
        public IList<FeatureBitDefinition> Definitions { get; set; }

        /// <summary>
        /// Determine if a feature should be enabled or disabled
        /// </summary>
        /// <param name="feature">Feature to be chedked</param>
        /// <returns>True if the feature is enabled.</returns>
        public bool IsEnabled<T>(T feature) where T : struct, IConvertible => IsEnabled(feature, 0);

        /// <summary>
        /// Determine if a feature should be enabled or disabled
        /// </summary>
        /// <param name="feature">Feature to be chedked</param>
        /// <param name="currentPermissionLevel">The permission level of the current user</param>
        /// <typeparam name="T">An enumeration or an integer</typeparam>
        /// <returns>True if the feature is enabled.</returns>
        public bool IsEnabled<T>(T feature, int currentPermissionLevel) where T: struct, IConvertible
        {
            Type tType = typeof(T);
            if (!(tType.IsEnum || tType == typeof(int)))
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            FeatureBitDefinition bitDef = Definitions.FirstOrDefault(x => x.Id == feature.ToInt32(new FeatureBitFormatProvider()));
            if (bitDef != null)
            {
                return EvaluateBitValue(bitDef, currentPermissionLevel);
            }
            
            string featureString = tType.IsEnum ? Enum.GetName(tType, feature) : feature.ToString(CultureInfo.InvariantCulture);
            throw new KeyNotFoundException($"Unable to find Feature {featureString}");
        }

        private static bool EvaluateBitValue(FeatureBitDefinition bitDef, int currentUsersPermissionLevel)
        {
            bool result;
            if (bitDef.ExcludedEnvironments?.Length > 0)
            {
                result = EvaluateEnvironmentBasedFeatureState(bitDef);
            }
            else if (bitDef.MinimumAllowedPermissionLevel > 0)
            {
                result = CheckMinimumPermission(bitDef, currentUsersPermissionLevel);
            }
            else
            {
                result = bitDef.OnOff;
            }

            return result;
        }

        private static bool CheckMinimumPermission(FeatureBitDefinition bitDef, int currentUsersPermissionLevel) => currentUsersPermissionLevel >= bitDef.MinimumAllowedPermissionLevel;

        private static bool EvaluateEnvironmentBasedFeatureState(FeatureBitDefinition bitDef)
        {
            bool featureState;
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToUpperInvariant();
            featureState = !bitDef.ExcludedEnvironments.ToUpperInvariant().Contains(env);
            return featureState;
        }

    }
}

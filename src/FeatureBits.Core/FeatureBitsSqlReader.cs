// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;

namespace FeatureBits
{
    /// <summary>
    /// This class is an implementation of the IFeatureBitsReader that reads the data from a SQL backend.
    /// </summary>
    // TODO: Eliminate this in favor of the FeatureBits.Data package
    public class FeatureBitsSqlReader : IFeatureBitsReader
    {
        /// <summary>
        /// DB Context for the feature bits data
        /// </summary>
        public FeatureBitsDbContext Context { get; }

        /// <summary>
        /// SQL Reader constructor
        /// </summary>
        /// <param name="context">DB Context from which to read the feature bits</param>
        public FeatureBitsSqlReader(FeatureBitsDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// This is the method that returns all the feature bits in the data store
        /// </summary>
        /// <returns>All Feature BitsData</returns>
        public FeatureBitsData ReadFeatureBits()
        {
            var featureBitDefinitions = Context.FeatureBitDefinitions;

            var returnVal = new FeatureBitsData {Definitions = featureBitDefinitions.ToList()};

            return returnVal;
        }
    }
}

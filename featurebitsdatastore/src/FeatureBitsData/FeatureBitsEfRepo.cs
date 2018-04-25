// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace FeatureBitsData
{
    public class FeatureBitsEfRepo : IFeatureBitsRepo
    {
        public FeatureBitsEfDbContext DbContext { get; }

        public FeatureBitsEfRepo(FeatureBitsEfDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IEnumerable<FeatureBitDefinition> GetAll()
        {
            return DbContext.FeatureBitDefinitions;
        }

        public FeatureBitDefinition Add(FeatureBitDefinition definition)
        {
            ValidateDefinition(definition);

            var entity = DbContext.FeatureBitDefinitions.Add(definition);
            DbContext.SaveChanges();

            return entity.Entity;
        }

        private static void ValidateDefinition(FeatureBitDefinition definition)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(definition, new ValidationContext(definition), validationResults, true);

            if (validationResults.Count > 0)
            {
                var errorStringBuilder = new StringBuilder();
                validationResults.ForEach(e => errorStringBuilder.Append(e.ErrorMessage + Environment.NewLine));
                throw new InvalidDataException(errorStringBuilder.ToString());
            }
        }
    }
}

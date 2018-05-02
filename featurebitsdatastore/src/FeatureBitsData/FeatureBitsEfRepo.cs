// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
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

            MakeSureAFeatureBitWithThatNameDoesNotExist(definition);

            var entity = DbContext.FeatureBitDefinitions.Add(definition);
            DbContext.SaveChanges();

            return entity.Entity;
        }

        private void MakeSureAFeatureBitWithThatNameDoesNotExist(FeatureBitDefinition definition)
        {
            var existenceCheck = DbContext.FeatureBitDefinitions.FirstOrDefault(fb => fb.Name == definition.Name);
            if (existenceCheck != null)
            {
                throw new DataException($"Cannot add. Feature bit with name '{definition.Name}' already exists.");
            }
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

        public void  Update(FeatureBitDefinition definition)
        {
            DbContext.Update(definition);
            DbContext.SaveChanges();
        }

        public void Remove(FeatureBitDefinition definitionToRemove)
        {
            DbContext.Remove(definitionToRemove);
            DbContext.SaveChanges();
        }
    }
}

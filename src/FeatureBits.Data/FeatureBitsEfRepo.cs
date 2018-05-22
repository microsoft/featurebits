// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FeatureBits.Data
{
    public class FeatureBitsEfRepo : IFeatureBitsRepo
    {
        public FeatureBitsEfDbContext DbContext { get; }

        public FeatureBitsEfRepo(FeatureBitsEfDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IEnumerable<FeatureBitDefinition>> GetAllAsync()
        {
            return await DbContext.FeatureBitDefinitions.ToListAsync();
        }

        public async  Task<FeatureBitDefinition> AddAsync(FeatureBitDefinition definition)
        {
            ValidateDefinition(definition);

            await MakeSureAFeatureBitWithThatNameDoesNotExist(definition);

            var entity = await DbContext.FeatureBitDefinitions.AddAsync(definition);
            await DbContext.SaveChangesAsync();

            return entity.Entity;
        }

        private async Task MakeSureAFeatureBitWithThatNameDoesNotExist(FeatureBitDefinition definition)
        {
            var existenceCheck = await DbContext.FeatureBitDefinitions.FirstOrDefaultAsync(fb => fb.Name == definition.Name);
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

        public async Task UpdateAsync(FeatureBitDefinition definition)
        {
            var entity = DbContext.Update(definition);
            await DbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(FeatureBitDefinition definitionToRemove)
        {
            DbContext.Remove(definitionToRemove);
            await DbContext.SaveChangesAsync();
        }
    }
}

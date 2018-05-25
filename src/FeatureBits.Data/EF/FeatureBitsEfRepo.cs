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

namespace FeatureBits.Data.EF
{
    public class FeatureBitsEfRepo : IFeatureBitsRepo
    {
        public FeatureBitsEfDbContext DbContext { get; }

        public FeatureBitsEfRepo(FeatureBitsEfDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IEnumerable<IFeatureBitDefinition>> GetAllAsync()
        {
            return await DbContext.FeatureBitDefinitions.ToListAsync();
        }

        public async Task<IFeatureBitDefinition> GetByNameAsync(string featureBitName)
        {
            var result = await DbContext.FeatureBitDefinitions.FirstOrDefaultAsync(definition =>
                definition.Name == featureBitName);
            return result;
        }

        public async  Task<IFeatureBitDefinition> AddAsync(IFeatureBitDefinition definition)
        {
            ValidateDefinition(definition);

            await MakeSureAFeatureBitWithThatNameDoesNotExist(definition);

            var entity = await DbContext.FeatureBitDefinitions.AddAsync(definition);
            await DbContext.SaveChangesAsync();

            return entity.Entity;
        }

        private async Task MakeSureAFeatureBitWithThatNameDoesNotExist(IFeatureBitDefinition definition)
        {
            var existenceCheck = await DbContext.FeatureBitDefinitions.FirstOrDefaultAsync(fb => fb.Name == definition.Name);
            if (existenceCheck != null)
            {
                throw new DataException($"Cannot add. Feature bit with name '{definition.Name}' already exists.");
            }
        }

        private static void ValidateDefinition(IFeatureBitDefinition definition)
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

        public async Task UpdateAsync(IFeatureBitDefinition definition)
        {
            DbContext.Update(definition);
            await DbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(IFeatureBitDefinition definitionToRemove)
        {
            DbContext.Remove(definitionToRemove);
            await DbContext.SaveChangesAsync();
        }
    }
}

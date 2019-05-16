// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
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

        public async Task<IFeatureBitDefinition> AddAsync(IFeatureBitDefinition definition)
        {
            FeatureBitEfDefinition newEntity = definition.ToEfDefinition();
            ValidateDefinition(newEntity);

            await MakeSureAFeatureBitWithThatNameDoesNotExist(newEntity);

            newEntity.Id = await GetNextId();
            var entity = await DbContext.FeatureBitDefinitions.AddAsync(newEntity);
            await DbContext.SaveChangesAsync();

            return entity.Entity;
        }

        private async Task MakeSureAFeatureBitWithThatNameDoesNotExist(FeatureBitEfDefinition definition)
        {
            var existenceCheck = await GetByNameAsync(definition.Name);
            if (existenceCheck != null)
            {
                throw new DataException($"Cannot add. Feature bit with name '{definition.Name}' already exists.");
            }
        }

        private static void ValidateDefinition(FeatureBitEfDefinition definition)
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
            try
            {
                var existing = await GetByNameAsync(definition.Name);
                if (existing == null)
                {
                    throw new DataException($"Could not update.  Feature bit with name '{definition.Name}' does not exist");
                }
                existing.Update(definition);
                DbContext.Update(existing);
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (ex.Entries.Count == 1 && ex.Entries[0].IsKeySet == false)
                {
                    DbContext.Add(definition);
                    await DbContext.SaveChangesAsync();
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task RemoveAsync(IFeatureBitDefinition definitionToRemove)
        {
            var toRemove = await GetByNameAsync(definitionToRemove.Name);
            if (toRemove != null)
            {
                DbContext.Remove(toRemove);
                await DbContext.SaveChangesAsync();
            }
        }

        private async Task<int> GetNextId()
        {
            var lastEntity = await DbContext.FeatureBitDefinitions.OrderByDescending(fn => fn.Id).FirstOrDefaultAsync();
            var maxInt = -1;
            var currentMax = lastEntity?.Id ?? 0;
            maxInt = Math.Max(currentMax, maxInt);

            return maxInt + 1;
        }
    }
}

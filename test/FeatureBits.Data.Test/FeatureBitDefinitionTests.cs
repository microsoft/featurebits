// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FeatureBits.Data.EF;
using FluentAssertions;
using Xunit;

namespace FeatureBits.Data.Test
{
    public class FeatureBitDefinitionTests
    {
        [Fact]
        public void It_has_certain_properties()
        {
            // Arrange
            var it = new FeatureBitEfDefinition
            {
                ExcludedEnvironments = "foo",
                LastModifiedByUser = "bar",
                CreatedDateTime = DateTime.Now.AddDays(-1),
                Name = "bat",
                CreatedByUser = "fizz",
                AllowedUsers = "buzz",
                Id = 22,
                LastModifiedDateTime = DateTime.Now,
                MinimumAllowedPermissionLevel = 22,
                OnOff = true
            };

            // Act
            var validationResults = new List<ValidationResult>();

            // Assert
            Validator.TryValidateObject(it, new ValidationContext(it), validationResults, true);
            validationResults.Count.Should().Be(0);
        }

        [Fact]
        public void It_requires_certain_properties()
        {
            // Arrange
            var it = new FeatureBitEfDefinition();
            var validationResults = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(it, new ValidationContext(it), validationResults, true);

            // Assert
            validationResults.Should().Contain(r => r.ErrorMessage == "The Name field is required.");
            validationResults.Should().Contain(r => r.ErrorMessage == "The CreatedByUser field is required.");
            validationResults.Should().Contain(r => r.ErrorMessage == "The LastModifiedByUser field is required.");
        }

        [Fact]
        public void It_has_MaxLength_for_certain_properties()
        {
            // Arrange
            var it = new FeatureBitEfDefinition { Name = "foo", CreatedByUser = "bar", LastModifiedByUser = "bat"};
            it.Name = new string('*', 101);
            it.ExcludedEnvironments = new string('*', 301);
            it.AllowedUsers = new string('*', 2049);
            it.CreatedByUser = new string('*', 101);
            it.LastModifiedByUser = new string('*', 101);

            // Act
            var validationResults = new List<ValidationResult>();

            // Assert
            Validator.TryValidateObject(it, new ValidationContext(it), validationResults, true);
            validationResults.Should().Contain(r => r.ErrorMessage == "The field Name must be a string or array type with a maximum length of '100'.");
            validationResults.Should().Contain(r => r.ErrorMessage == "The field ExcludedEnvironments must be a string or array type with a maximum length of '300'.");
            validationResults.Should().Contain(r => r.ErrorMessage == "The field AllowedUsers must be a string or array type with a maximum length of '2048'.");
            validationResults.Should().Contain(r => r.ErrorMessage == "The field CreatedByUser must be a string or array type with a maximum length of '100'.");
            validationResults.Should().Contain(r => r.ErrorMessage == "The field LastModifiedByUser must be a string or array type with a maximum length of '100'.");
        }
    }
}

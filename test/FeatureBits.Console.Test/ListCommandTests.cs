// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Dotnet.FBit;
using Dotnet.FBit.Command;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using FeatureBits.Core;
using Xunit;

namespace FeatureBits.Console.Test
{
    public class ListCommandTests
    {
        [Fact]
        public void It_should_run_and_list_short_FeatureBitDefinition()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.AppendLine(s);
            var bit1 = new CommandFeatureBitDefintion { Id = 1, Name = "foo1" };
            var bit2 = new CommandFeatureBitDefintion { Id = 1, Name = "foo2" };
            var opts = new ListOptions();
            var repo = Substitute.For<IFeatureBitsRepo>();
            var consoleTable = Substitute.For<IConsoleTable>();
            var fbits = new List<IFeatureBitDefinition> { bit1, bit2 };


            repo.GetAllAsync().Returns(Task.FromResult((IEnumerable<IFeatureBitDefinition>)fbits));

            var it = new ListCommand(opts, repo, consoleTable);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(0);

            consoleTable.Received().Print(Arg.Is<DataTable>(dt =>
                dt.Rows.Count == 2 &&
                dt.Columns.Count == 2 &&
                dt.Columns[0].Caption == "Id" &&
                dt.Columns[1].Caption == "Name"));
        }

        [Fact]
        public void It_should_run_and_list_long_FeatureBitDefinition()
        {
            // Arrange
            var sb = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => sb.AppendLine(s);
            var bit1 = new CommandFeatureBitDefintion { Id = 1, Name = "foo1", OnOff = true, ExcludedEnvironments = "prod1", MinimumAllowedPermissionLevel = 10, ExactAllowedPermissionLevel = 30, AllowedUsers = "me1", IncludedEnvironments = "Dev1,Dev3" };
            var bit2 = new CommandFeatureBitDefintion { Id = 2, Name = "foo2", OnOff = false, ExcludedEnvironments = "prod2", MinimumAllowedPermissionLevel = 20, ExactAllowedPermissionLevel = 40, AllowedUsers = "me2", IncludedEnvironments = "Dev1,Dev3" };
            var opts = new ListOptions { Long = true };
            var repo = Substitute.For<IFeatureBitsRepo>();
            var consoleTable = Substitute.For<IConsoleTable>();
            var fbits = new List<IFeatureBitDefinition> { bit1, bit2 };


            repo.GetAllAsync().Returns(Task.FromResult((IEnumerable<IFeatureBitDefinition>)fbits));

            var it = new ListCommand(opts, repo, consoleTable);

            // Act
            var result = it.RunAsync().Result;

            // Assert
            result.Should().Be(0);

            consoleTable.Received().Print(Arg.Is<DataTable>(dt =>
                dt.Rows.Count == 2 &&
                dt.Columns.Count == 8 &&
                dt.Columns[0].Caption == "Id" &&
                dt.Columns[1].Caption == "Name" &&
                dt.Columns[2].Caption == "OnOff" &&
                dt.Columns[3].Caption == "ExcludedEnvironments" &&
                dt.Columns[4].Caption == "MinimumAllowedPermissionLevel" &&
                dt.Columns[5].Caption == "ExactAllowedPermissionLevel" &&
                dt.Columns[6].Caption == "AllowedUsers" &&
                dt.Columns[7].Caption == "IncludedEnvironments"));
        }
    }
}

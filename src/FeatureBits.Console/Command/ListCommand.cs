// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.FBit.Command
{
    /// <summary>
    /// Class that represents the command to list feature bits
    /// </summary>
    public class ListCommand
    {
        private readonly IFeatureBitsRepo _repo;
        private readonly ListOptions _options;
        private readonly IConsoleTable _consoleTable;

        /// <summary>
        /// Class that represents the command to list feature bits
        /// </summary>
        /// <summary>
        /// List feature bit enumerations
        /// </summary>
        /// <param name="options">Command line options to list Features enumerations.</param>
        /// <param name="repo">Feature Bit repository</param>
        /// <param name="consoleTable">helper to write to console in table format</param>
        public ListCommand(ListOptions options, IFeatureBitsRepo repo, IConsoleTable consoleTable)
        {
            _repo = repo;
            _options = options;
            _consoleTable = consoleTable;
        }

        public async Task<int> RunAsync()
        {
            return await PrintBits();
        }

        private async Task<int> PrintBits()
        {
            var bits = await _repo.GetAllAsync();
            var bitsDataTable = GetDataTable(bits.ToList());

            _consoleTable.Print(bitsDataTable);

            return 0;
        }

        private DataTable GetDataTable(IList<IFeatureBitDefinition> featureBitDefinitions)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));

            if(_options.Long)
            {
                dataTable.Columns.Add("OnOff", typeof(bool));
                dataTable.Columns.Add("ExcludedEnvironments", typeof(string));
                dataTable.Columns.Add("MinimumAllowedPermissionLevel", typeof(int));
                dataTable.Columns.Add("ExactAllowedPermissionLevel", typeof(int));
                dataTable.Columns.Add("AllowedUsers", typeof(string));
                dataTable.Columns.Add("IncludedEnvironments", typeof(string));
            }

            foreach (var featureBitDefinition in featureBitDefinitions)
            {
                dataTable.Rows.Add(GetDataRow(featureBitDefinition));
            }

            return dataTable;
        }

        private object[] GetDataRow(IFeatureBitDefinition featureBitDefinition)
        {
            var cells = new List<object>
            {
                featureBitDefinition.Id,
                featureBitDefinition.Name
            };

            if (_options.Long)
            {
                cells.Add(featureBitDefinition.OnOff);
                cells.Add(featureBitDefinition.ExcludedEnvironments);
                cells.Add(featureBitDefinition.MinimumAllowedPermissionLevel);
                cells.Add(featureBitDefinition.ExactAllowedPermissionLevel?? -1);
                cells.Add(featureBitDefinition.AllowedUsers);
                cells.Add(featureBitDefinition.IncludedEnvironments);
            }

            return cells.ToArray();
        }
    }
}

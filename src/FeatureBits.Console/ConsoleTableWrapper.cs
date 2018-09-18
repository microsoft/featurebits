// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using ConsoleTableExt;
using System.Data;

namespace Dotnet.FBit
{
    /// <summary>
    /// <see cref="IConsoleTable"/>
    /// </summary>
    public class ConsoleTableWrapper : IConsoleTable
    {
        /// <summary>
        /// <see cref="IConsoleTable.Print(DataTable)"/>
        /// </summary>        
        public void Print(DataTable dataTable)
        {
            if (dataTable.Rows.Count <= 0)
            {
                Console.WriteLine("No data found in FeatureBits data store.");
            }
            else
            {
                ConsoleTableBuilder.From(dataTable).WithFormat(ConsoleTableBuilderFormat.Minimal).ExportAndWriteLine();
            }
        }
    }
}

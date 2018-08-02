// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;

namespace Dotnet.FBit
{
    /// <summary>
    /// The main function of this interface is to enable mock creation for unit testing
    /// </summary>
    public interface IConsoleTable
    {
        /// <summary>
        /// Writes to console in a table format
        /// </summary>
        /// <param name="dataTable">data to write to console</param>
        void Print(DataTable dataTable);
    }
}
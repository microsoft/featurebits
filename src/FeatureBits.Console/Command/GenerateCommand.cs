// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotnet.FBit.CommandOptions;
using FeatureBits.Data;

namespace Dotnet.FBit.Command
{
    /// <summary>
    /// Generate an enumeration based on the datasource
    /// </summary>
    public class GenerateCommand
    {
        private readonly IFeatureBitsRepo _repo;
        private readonly IFileSystem _fileSystem;
        private readonly GenerateOptions _options;
        public StringBuilder FileContent { get; set; } = new StringBuilder(600);

        /// <summary>
        /// File system interface to read/write files 
        /// </summary>
        /// <summary>
        /// Generate a file with feature bit enumerations
        /// </summary>
        /// <param name="options">Command line options to generate Features enumerations.</param>
        /// <param name="repo">Feature Bit repository</param>
        /// <param name="fileSystem">Interface to the File System</param>
        public GenerateCommand(GenerateOptions options, IFeatureBitsRepo repo, IFileSystem fileSystem)
        {
            _repo = repo;
            _fileSystem = fileSystem;
            _options = options;
        }

        public async Task<bool> RunAsync()
        {
            var fbitEnums = await GetBits();
            return WriteDataToFile(fbitEnums, _options.Namespace);
        }

        public async Task<IEnumerable<(string Name, int Id)>> GetBits()
        {
            var bits = await _repo.GetAllAsync();
            IEnumerable<(string Name, int Id)> fbitEnums = bits.Select(x => (x.Name, x.Id));
            return fbitEnums;
        }

        public bool WriteDataToFile(IEnumerable<(string Name, int Id)> featureBitData, string fileNamespace)
        {
            bool returnValue = false;

            try
            {
                var outputFile = GetOutputFileInfo();
                if (CannotWriteFile(outputFile))
                {
                    SystemContext.ConsoleErrorWriteLine("Output file already exists.");
                }
                else
                {
                    // If namespace is not specified, try to find a namespace from the local csproj file
                    fileNamespace = GetNamespace(fileNamespace, outputFile);

                    // Write the features.cs file
                    WriteFileContent(featureBitData, fileNamespace, outputFile);
                    returnValue = true;
                }
            }
            catch (FileNotFoundException)
            {
                SystemContext.ConsoleErrorWriteLine("Could not find default namespace for .CSPROJ");
            }

            return returnValue;
        }

        private void WriteFileContent(IEnumerable<(string Name, int Id)> featureBitData, string fileNamespace, 
            FileInfoBase outputFile)
        {
            const string headerInfoFormat = @"
namespace {0}
{{
    /// <summary>
    /// This is a generated list of Enums that list the names/ID numbers for the feature bits used in your application.
    /// </summary>
    public enum Features
    {{";
            const string tailInfo = @"    }
}";

            FileContent.AppendLine(string.Format(headerInfoFormat, fileNamespace));
            foreach (var featureBit in featureBitData)
            {
                FileContent.AppendLine($"        {featureBit.Name} = {featureBit.Id},");
            }

            FileContent.AppendLine(tailInfo);
            using (var writer = outputFile.CreateText())
            {
                writer.Write(FileContent.ToString());
            }

            SystemContext.ConsoleWriteLine($"Feature bit enum successfully written to {outputFile.FullName}.");
        }

        private string GetNamespace(string fileNamespace, FileInfoBase outputFile)
        {
            if (string.IsNullOrWhiteSpace(fileNamespace))
            {
                string namespaceToUse = ProjectFileHelper.GetDefaultNamespace(outputFile.Directory);
                fileNamespace = namespaceToUse;
            }

            return fileNamespace;
        }

        private bool CannotWriteFile(FileInfoBase outputFile)
        {
            return outputFile.Exists && !_options.Force;
        }

        public FileInfoBase GetOutputFileInfo()
        {
            FileInfoBase outputFile = _fileSystem.FileInfo.FromFileName(_options.OutputFileName);
            return outputFile;
        }
    }
}

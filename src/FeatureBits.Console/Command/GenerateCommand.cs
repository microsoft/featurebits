// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Dotnet.FBit.CommandOptions;
using FeatureBitsData;

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
        public StringBuilder FileContent = new StringBuilder(600);

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

        public bool Run()
        {
            var fbitEnums = GetBits();
            return WriteDataToFile(fbitEnums, _options.Namespace);
        }

        public IEnumerable<(string Name, int Id)> GetBits()
        {
            var bits = _repo.GetAll();
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
                string namespaceToUse = GetDefaultNamespace(outputFile.Directory);
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

        public string GetDefaultNamespace(DirectoryInfoBase directory)
        {
            FileInfoBase[] fileInfos = FindAllCsprojFiles(directory);

            FileInfoBase fileToSearch = ChooseCsproj(fileInfos);

            string nameSpace = GetNamespaceFromCsproj(fileToSearch);

            return nameSpace;
        }

        private static FileInfoBase[] FindAllCsprojFiles(DirectoryInfoBase directory)
        {
            FileInfoBase[] fileInfos = directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
            if (fileInfos.Length == 0)
            {
                SystemContext.ConsoleErrorWriteLine(
                    "No csproj file found for default namespace. Please specify a namespace as a command argument.");
                throw new FileNotFoundException();
            }

            return fileInfos;
        }

        private static FileInfoBase ChooseCsproj(FileInfoBase[] fileInfos)
        {
            if (fileInfos.Length > 1)
            {
                SystemContext.ConsoleWriteLine("Multiple csproj files found for namespace, using the first one.");
            }

            var fileToSearch = fileInfos[0];
            return fileToSearch;
        }

        private string GetNamespaceFromCsproj(FileInfoBase fileToSearch)
        {
            string nameSpace;
            using (var fr = fileToSearch.OpenRead())
            {
                var sr = new StreamReader(fr);
                string fileContents = sr.ReadToEnd();

                // Look for a <RootNamespace> tag
                string openingTag = "<RootNamespace>";
                string closingTag = "</RootNamespace>";
                int openingIndex = fileContents.IndexOf(openingTag, StringComparison.Ordinal);
                if (openingIndex > -1)
                {
                    int closingIndex =
                        fileContents.IndexOf(closingTag, openingIndex + openingTag.Length, StringComparison.Ordinal);
                    int substringStartingIndex = openingIndex + openingTag.Length;
                    nameSpace = fileContents.Substring(substringStartingIndex, closingIndex - substringStartingIndex);
                }
                else
                {
                    nameSpace = _fileSystem.Path.GetFileNameWithoutExtension(fileToSearch.Name);
                }
            }

            return nameSpace;
        }
    }
}

using System;
using System.Collections.Generic;
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

        private bool WriteDataToFile(IEnumerable<(string Name, int Id)> featureBitData, string fileNamespace)
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

            // Check if the file exists
            FileInfoBase outputFile = _fileSystem.FileInfo.FromFileName(_options.OutputFileName);
            if (outputFile.Exists && !_options.Force)
            {
                SystemContext.ConsoleErrorWriteLine("Output file already exists.");
                return false;
            }

            // If namespace is not specified, try to find a namespace from the local csproj file
            if (string.IsNullOrWhiteSpace(fileNamespace))
            {
                (string namespaceToUse, bool success) = GetDefaultNamespace(outputFile.Directory);
                if (!success)
                {
                    SystemContext.ConsoleErrorWriteLine("Could not find default namespace for .CSPROJ");
                    return false;
                }

                fileNamespace = namespaceToUse;
            }

            // Write the features.cs file
            StringBuilder builder = new StringBuilder(600);
            builder.AppendLine(string.Format(headerInfoFormat, fileNamespace));
            foreach (var featureBit in featureBitData)
            {
                builder.AppendLine($"        {featureBit.Name} = {featureBit.Id},");
            }

            builder.AppendLine(tailInfo);
            using (var writer = outputFile.CreateText())
            {
                writer.Write(builder.ToString());
            }

            SystemContext.ConsoleWriteLine($"Feature bit enum successfully written to {outputFile.FullName}.");
            return true;
        }

        private (string, bool) GetDefaultNamespace(DirectoryInfoBase directory)
        {
            // Find the CSPROJ file to search
            FileInfoBase[] fileInfos = directory.GetFiles("*.csproj", System.IO.SearchOption.TopDirectoryOnly);
            if (fileInfos.Length == 0)
            {
                SystemContext.ConsoleErrorWriteLine("No csproj file found for default namespace. Please specify a namespace as a command argument.");
                return (string.Empty, false);
            }

            if (fileInfos.Length > 1)
            {
                SystemContext.ConsoleWriteLine("Multiple csproj files found for namespace, using the first one.");
            }

            var fileToSearch = fileInfos[0];

            // Search CSPROJ file for RootNamespace or take filename
            string nameSpace;
            using (var fr = fileToSearch.OpenRead())
            {
                var sr = new System.IO.StreamReader(fr);
                string fileContents = sr.ReadToEnd();

                // Look for a <RootNamespace> tag
                string openingTag = "<RootNamespace>";
                string closingTag = "</RootNamespace>";
                int openingIndex = fileContents.IndexOf(openingTag, StringComparison.Ordinal);
                if (openingIndex > -1)
                {
                    int closingIndex = fileContents.IndexOf(closingTag, openingIndex + openingTag.Length, StringComparison.Ordinal);
                    int substringStartingIndex = openingIndex + openingTag.Length;
                    nameSpace = fileContents.Substring(substringStartingIndex, closingIndex - substringStartingIndex);
                }
                else
                {
                    nameSpace = _fileSystem.Path.GetFileNameWithoutExtension(fileToSearch.Name);
                }
            }

            return (nameSpace, !string.IsNullOrWhiteSpace(nameSpace));
        }
    }
}

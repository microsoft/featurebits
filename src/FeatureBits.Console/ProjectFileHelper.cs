// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.Abstractions;

namespace Dotnet.FBit
{
    public static class ProjectFileHelper
    {
        public static IFileSystem FileSystem { get; set; } = new FileSystem();

        public static string GetDefaultNamespace(DirectoryInfoBase directory)
        {
            FileInfoBase[] fileInfos = FindAllCsprojFiles(directory);
            FileInfoBase fileToSearch = ChooseCsproj(fileInfos);
            return GetNamespaceFromCsproj(fileToSearch);
        }

        public static string GetUserSecretsId(string directory)
        {
            try
            {
                FileInfoBase[] fileInfos = FindAllCsprojFiles(FileSystem.DirectoryInfo.FromDirectoryName(directory));
                FileInfoBase fileToSearch = ChooseCsproj(fileInfos);
                return GetUserSecretsIdFromProject(fileToSearch);
            }
            catch (FileNotFoundException)
            {
                return string.Empty;
            }
        }

        private static string GetUserSecretsIdFromProject(FileInfoBase fileToSearch)
        {
            string openingTag = "<UserSecretsId>";
            string closingTag = "</UserSecretsId>";
            string userSecretsId = string.Empty;
            using (Stream fr = fileToSearch.OpenRead())
            {
                StreamReader sr = new StreamReader(fr);
                string fileContents = sr.ReadToEnd();
                int openingIndex = fileContents.IndexOf(openingTag, StringComparison.OrdinalIgnoreCase);
                if (openingIndex > -1)
                {
                    int closingIndex = fileContents.IndexOf(closingTag, openingIndex + openingTag.Length, StringComparison.OrdinalIgnoreCase);
                    int substringStartingIndex = openingIndex + openingTag.Length;
                    userSecretsId = fileContents.Substring(substringStartingIndex, closingIndex - substringStartingIndex);
                }
            }

            return userSecretsId;
        }

        private static FileInfoBase[] FindAllCsprojFiles(DirectoryInfoBase directory)
        {
            FileInfoBase[] fileInfos = directory.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
            if (fileInfos.Length == 0)
            {
                SystemContext.ConsoleErrorWriteLine("No csproj file found for default namespace. Please specify a namespace as a command argument.");
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

            FileInfoBase fileToSearch = fileInfos[0];
            return fileToSearch;
        }

        private static string GetNamespaceFromCsproj(FileInfoBase fileToSearch)
        {
            string nameSpace;
            using (Stream fr = fileToSearch.OpenRead())
            {
                StreamReader sr = new StreamReader(fr);
                string fileContents = sr.ReadToEnd();

                // Look for a <RootNamespace> tag
                string openingTag = "<RootNamespace>";
                string closingTag = "</RootNamespace>";
                int openingIndex = fileContents.IndexOf(openingTag, StringComparison.OrdinalIgnoreCase);
                if (openingIndex > -1)
                {
                    int closingIndex = fileContents.IndexOf(closingTag, openingIndex + openingTag.Length, StringComparison.OrdinalIgnoreCase);
                    int substringStartingIndex = openingIndex + openingTag.Length;
                    nameSpace = fileContents.Substring(substringStartingIndex, closingIndex - substringStartingIndex);
                }
                else
                {
                    nameSpace = FileSystem.Path.GetFileNameWithoutExtension(fileToSearch.Name);
                }
            }

            return nameSpace;
        }
    }
}

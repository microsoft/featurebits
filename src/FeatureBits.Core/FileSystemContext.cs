// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;

namespace FeatureBits
{
    public static class FileSystemContext
    {
        [ThreadStatic]
        private static Func<string, string> _readAllText;

        public static Func<string, string> ReadAllText
        {
            get => _readAllText ?? (_readAllText = File.ReadAllText);
            set => _readAllText = value;
        }
    }
}

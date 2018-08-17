// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace FeatureBits.Core
{
    public static class SystemContext
    {
        [ThreadStatic] private static Action<string> _consoleWriteLine;
        [ThreadStatic] private static Action<string> _consoleErrorWriteLine;
        [ThreadStatic] private static Func<DateTime> _now;
        [ThreadStatic] private static Func<string, string> _getEnvironmentVariable;

        public static Action<string> ConsoleWriteLine
        {
            get => _consoleWriteLine ?? (_consoleWriteLine = Console.WriteLine);
            set => _consoleWriteLine = value;
        }

        public static Action<string> ConsoleErrorWriteLine
        {
            get => _consoleErrorWriteLine ?? (_consoleErrorWriteLine = Console.Error.WriteLine);
            set => _consoleErrorWriteLine = value;
        }
        
        public static Func<DateTime> Now
        {
            get { return _now ?? (_now = () => DateTime.Now); }
            set => _now = value;
        }
        
        public static Func<string, string> GetEnvironmentVariable
        {
            get => _getEnvironmentVariable ?? (_getEnvironmentVariable = Environment.GetEnvironmentVariable);
            set => _getEnvironmentVariable = value;
        }
    }
}

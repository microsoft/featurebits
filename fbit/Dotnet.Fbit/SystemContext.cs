using System;
using System.IO.Abstractions;

namespace Dotnet.FBit
{
    public static class SystemContext
    {
        [ThreadStatic] private static Action<string> _consoleWriteLine;
        [ThreadStatic] private static Action<string> _consoleErrorWriteLine;

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
        // FileSystem.Path.GetFileNameWithoutExtension(fileToSearch.Name)
    }
}

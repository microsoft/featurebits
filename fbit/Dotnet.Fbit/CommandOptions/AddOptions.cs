using CommandLine;

namespace Dotnet.FBit.CommandOptions
{
    /// <summary>
    /// Options for the generate command
    /// </summary>
    [Verb(name: "add", HelpText = "Add a feature bit to the data store")]
    public class AddOptions
    {
        /// <summary>
        /// Connection string to the database storing the feature bits
        /// </summary>
        [Option('c', "connectionstring", Required = true, HelpText = "Connection string to the database storing the feature bits")]
        public string DatabaseConnectionString { get; set; }

        /// <summary>
        /// Specifies whether the feature bit should be blanket on or off.
        /// </summary>
        [Option('o', "onoff", HelpText = "Specifies whether the feature bit should be blanket on or off.")]
        public bool OnOff { get; set; }
    }
}

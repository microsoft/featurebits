using System.Text;
using Dotnet.FBit;
using Xunit;

namespace Dotnet.Fbit.Tests
{
    public class ProgramTests
    {
        // TODO 001: Make this work
        //[Fact]
        //public void ItReturnsDoingAddTemporarily()
        //{
        //    // arrange 
        //    var builder = new FileContent();
        //    SystemContext.ConsoleWriteLine = s => builder.Append(s);
        //    SystemContext.ConsoleErrorWriteLine = s => builder.Append(s);
        //    var args = "add -c foo";

        //    // Act
        //    var result = Program.Main(args.Split(' '));

        //    // assert
        //    Assert.Equal(0, result);
        //    Assert.Equal("Doing Add - Dotnet.FBit.CommandOptions.AddOptions", builder.ToString());
        //}

        [Fact]
        public void ItReturns1OnInvalidParams()
        {
            // arrange 
            var builder = new StringBuilder();
            SystemContext.ConsoleWriteLine = s => builder.Append(s);
            SystemContext.ConsoleErrorWriteLine = s => builder.Append(s);
            var args = "foo";

            // Act
            var result = Program.Main(args.Split(' '));

            // assert
            Assert.Equal(1, result);
        }

    }
}

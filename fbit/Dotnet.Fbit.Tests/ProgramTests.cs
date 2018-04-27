using System.Text;
using Dotnet.FBit;
using Xunit;

namespace Dotnet.Fbit.Tests
{
    public class ProgramTests
    {
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

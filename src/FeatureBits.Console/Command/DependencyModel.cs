using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.FBit.Command
{
    public sealed class DependencyModel
    {
        public int ParentId { get; set; }
        public int ChildId { get; set; }
    }
}

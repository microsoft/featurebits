using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.FBit.Command
{
    public sealed class DependencyModel
    {
        public DependencyModel()
        {
        }

        public string OwningId { get; set; }
        public string ParentId { get; set; }
        public string ChildId { get; set; }
        public override string ToString()
        {
            return $"Fbit:{OwningId} => {ParentId}|{ChildId}";
        }

    }
}

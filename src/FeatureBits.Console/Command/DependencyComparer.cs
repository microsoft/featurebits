using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.FBit.Command
{
    public class DependencyComparer : IEqualityComparer<DependencyModel>
    {
        public bool Equals(DependencyModel source, DependencyModel other)
        {
            if (source is null || other is null)
            {
                return false;
            }

            return source.OwningId == other.OwningId && source.ParentId == other.ParentId && source.ChildId == other.ChildId;
        }

        public int GetHashCode(DependencyModel source)
        {
            if (source is null)
            {
                return 0;
            }
            // build heirarchial pattern
            return $"{source.OwningId}|{source.ParentId}|{source.ChildId}".GetHashCode();
        }
    }
}

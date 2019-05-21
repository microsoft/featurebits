using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FeatureBits.Core
{
    public class FeatureBitCommandException : Exception
    {
        public FeatureBitCommandException()
        {
        }

        public FeatureBitCommandException(string message) : base(message)
        {
        }

        public FeatureBitCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NET452
        protected FeatureBitCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}

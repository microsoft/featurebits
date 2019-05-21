using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FeatureBits.Data
{
    public class FeatureBitDataException : Exception
    {
        public FeatureBitDataException()
        {
        }

        public FeatureBitDataException(string message) : base(message)
        {
        }

        public FeatureBitDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NET452
        protected FeatureBitDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}

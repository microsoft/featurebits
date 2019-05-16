using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FeatureBits.Core
{
    public class FeatureBitException : Exception
    {
        public FeatureBitException()
        {
        }

        public FeatureBitException(string message) : base(message)
        {
        }

        public FeatureBitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FeatureBitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

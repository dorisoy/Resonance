using System;
using System.Collections.Generic;
using System.Text;

namespace Resonance.Exceptions
{
    public class ResonanceAdapterFailedException : Exception
    {
        public ResonanceAdapterFailedException(String message, Exception adapterException) : base(message, adapterException)
        {

        }
    }
}

using Resonance.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resonance.Tests.SignalR
{
    public class TestServiceInformation : IResonanceServiceInformation
    {
        public string ServiceId { get; set; }
    }
}

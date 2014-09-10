using Artemis;
using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisUnitTests
{
    public class OverrideComponent : IComponent
    {
        public int ComponentId { get {return -1; } }
        public bool IsSerializable { get { return false; } }
    }
}

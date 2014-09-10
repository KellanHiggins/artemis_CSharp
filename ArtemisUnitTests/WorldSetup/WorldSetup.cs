using Artemis;
using Artemis.Interface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisUnitTests.WorldSetup
{
    [TestFixture]
    public class SetupTheWorld
    {
        protected EntityWorld entityWorld;

        [TestFixtureSetUp]
        public void SetupWorld()
        {
            entityWorld = new EntityWorld();
            entityWorld.InitializeAll(true);
        }
    }
}

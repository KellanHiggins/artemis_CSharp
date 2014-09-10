using Artemis;
using Artemis.Interface;
using Artemis.Manager;
using Artemis.Attributes;
using Artemis.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisUnitTests.EntityModel.TestProcessors
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 1)]
    public class DestroyEntityProcessor : QueueSystemProcessing
    {
        public DestroyEntityProcessor()
            : base()
        {

        }

        public override void Process(Entity entity)
        {
            entity.Delete();
        }
    }
}

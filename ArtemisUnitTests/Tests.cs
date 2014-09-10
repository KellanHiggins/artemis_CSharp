using Artemis;
using ArtemisUnitTests.EntityModel.TestProcessors;
using ArtemisUnitTests.WorldSetup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisUnitTests
{
    [TestFixture]
    public class Tests : SetupTheWorld
    {
        [Test]
        public void TestOverLappingComponents()
        {
            Entity entity = entityWorld.CreateEntity();
            entity.AddComponent<TestComponent>(new TestComponent());
            Debug.WriteLine("New entity has been created with entity Ids: " + entity.ToString() + " with components numbering 1: " + entity.Components.Count);

            entityWorld.Update();

            entityWorld.SystemManager.GetSystem<DestroyEntityProcessor>().AddToQueue(entity);

            Debug.WriteLine("There should be a total of one entity now" + entityWorld.EntityManager.ActiveEntities.Count);

            entityWorld.Update();

            Entity newEntity = entityWorld.CreateEntity();
            Debug.WriteLine("The new entity has an Id of: " + newEntity.ToString() + " with components " + newEntity.Components.Count);

            entityWorld.Update();

            Entity newerEntity = entityWorld.CreateEntity();
            Debug.WriteLine("The new entity has an Id of: " + newerEntity.ToString() + " with components " + newEntity.Components.Count);

            ////Debug.WriteLine("There should be no any active entities" + entityWorld.EntityManager.ActiveEntities.Count);
            //Debug.WriteLine("There should be no any active entities " + entity.ToString() + " with components numbering : " + entity.Components.Count);
            ////Debug.WriteLine("This is the number of removed entities: " + entityWorld.EntityManager.TotalRemoved);

            //Entity newEntity = entityWorld.CreateEntity();

            //Debug.WriteLine("The new entity has an Id of: " + newEntity.ToString());

            //Debug

        }
    }
}

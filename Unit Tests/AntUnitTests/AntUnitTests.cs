using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utils;
using FakeItEasy;
using ProducerConsumer;
using DynamicObjects;
using BoardInterfaceForObjects;
using IDynamicObjects;
using System.Collections.Generic;

namespace Unit_Tests
{
    [TestClass]
    public class AntUnitTests
    {
        [TestMethod]
        public void Test_Fight_With_Other_Should_Win()
        {
            //initialize
            var ant = new Ant() { Strength = 3 };
            
            var other = A.Fake<IDynamicObject>();
            var producerConsumer = A.Fake<IProducerConsumerMessages<string>>();
            ant.ProducerConsumer = producerConsumer;
            A.CallTo(() => other.Strength).Returns(1);
            A.CallTo(() => other.SetState(A<State>._)).Invokes(()=> other.State = State.Depressed);
            
            //act
            ant.Fight(other);
            //assert
            Assert.AreEqual(5, ant.Strength);
            Assert.AreEqual(State.Depressed, other.State);
        }

        [TestMethod]
        public void Test_Fight_With_Other_Should_Lose()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            var ant = new Ant(info,rnd);

            var producerConsumer = A.Fake<IProducerConsumerMessages<string>>();
            ant.ProducerConsumer = producerConsumer;
            var other = A.Fake<IDynamicObject>();
            A.CallTo(() => other.Strength).ReturnsLazily(()=> 4);
            A.CallToSet(() => other.Strength).To(6);
            //act
            ant.Fight(other);
            //assert
            A.CallTo(() => other.AddStrength(2)).MustHaveHappened();
            Assert.AreEqual(State.Depressed, ant.State);
        }

        [TestMethod]

        public void Test_ActDepressed()
        {
            //initialize
            IBoardFunctions board = A.Fake<IBoardFunctions>();
            IInfo info = A.Fake<IInfo>();
            IProducerConsumerMessages<string> pc = A.Fake<IProducerConsumerMessages<string>>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IDynamicObject ant = new Ant(info,rnd) { BoardFunctions = board, X = 1, Y = 1,ProducerConsumer = pc};

            List<IDynamicObject> l = new List<IDynamicObject>();
            var positions = new List<(int, int)> { (0, 0), (1, 0), (2, 1)};
            foreach (var p in positions)
            {
                var obj = A.Fake<IDynamicObject>();
                A.CallTo(() => obj.X).ReturnsLazily(() => p.Item1);
                A.CallTo(() => obj.Y).ReturnsLazily(() => p.Item2);
                A.CallTo(() => obj.State).Returns(State.Alive);
                l.Add(obj);
            }

            var dynamicObject = A.Fake<IDynamicObject>();

            A.CallTo(() => board.GetNearObjects(ant.X, ant.Y, 1)).Returns(l);
            A.CallTo(() => info.MaxObjectsPerArea).Returns(3);
            A.CallTo(() => info.MinObjectsPerArea).Returns(1);

            //act
            ant.SetState(State.Depressed);
            ant.Action();

            //assert
            Assert.AreEqual(State.Alive, ant.State);
        }

        [TestMethod]

        public void Test_TryToMove()
        {
            IInfo info = A.Fake<IInfo>();
            IBoardFunctions board = A.Fake<IBoardFunctions>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            IDynamicObject ant = new Ant(info, rnd) { X = 0, Y = 0,Agility=1 , BoardFunctions = board};

            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).Returns(1);

            //gameLogic.ActAliveObject(dynamicObject);

            ant.TryToMove();
            A.CallTo(() => board.TryToMove(0, 0, 1, 1)).MustHaveHappened();

        }

    }
}

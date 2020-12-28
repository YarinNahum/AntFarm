using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FakeItEasy;
using ProducerConsumer;
using DynamicObjects;

namespace Unit_Tests
{
    [TestClass]
    public class AntUnitTests
    {
        [TestMethod]
        public void Test_Fight_With_Other_Should_Win()
        {
            var ant = new Ant();
            var other = A.Fake<IDynamicObject>();
            var producerConsumer = A.Fake<IProducerConsumerMessages<string>>();
            ant.ProducerConsumer = producerConsumer;
            A.CallTo(() => other.Strength).Returns(1);
            A.CallTo(() => other.SetState(A<State>._)).Invokes(()=> other.State = State.Depressed);
            ant.Fight(other);
            Assert.AreEqual(5, ant.Strength);
            Assert.AreEqual(State.Depressed, other.State);
        }

        [TestMethod]
        public void Test_Fight_With_Other_Should_Lose()
        {
            var ant = new Ant();
            var producerConsumer = A.Fake<IProducerConsumerMessages<string>>();
            ant.ProducerConsumer = producerConsumer;
            var other = A.Fake<IDynamicObject>();
            int strength = 4;
            A.CallTo(() => other.Strength).ReturnsLazily(()=> strength);
            A.CallToSet(() => other.Strength).To(6);
            ant.Fight(other);
            strength = 6;
            Assert.AreEqual(6, other.Strength);
            Assert.AreEqual(State.Depressed, ant.State);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardNamespace;
using Utils;
using IDynamicObjects;


namespace GameLogicNameSpace
{
    public interface IGameLogic
    {
        /// <summary>
        /// Generate initial objects on the board.
        /// The number of objects to create is given with the value of <see cref="Info.NumberOfObjects"/>.
        /// If the count is bigger than the dimensions of the board, it will throw a ArgumentException exception.
        /// See <see cref="Board"/>
        /// 
        /// <seealso cref="IDynamicObject"/>
        /// </summary>
        void GenerateInitialObjects();

        /// <summary>
        /// Wake up each dynamic object on the board.
        /// </summary>
        void WakeUp();

        /// <summary>
        /// Update the dynamic objects that are alive.
        /// </summary>
        void UpdateAlive();

        /// <summary>
        /// Returns the number of alive objects.
        /// </summary>
        /// <returns>A number of alive objects</returns>
        int GetNumberOfAliveObjects();


        /// <summary>
        /// Decide how to act when a new day is started. It will create a new task for each alive object
        /// and each dynamic object will choose what action to perform.
        /// See <see cref="Info.TimePerDay"/>
        /// <seealso cref=""/>
        /// on the board using <see cref="TaskFactory"/> 
        /// </summary>
        void StartNewDay();

        /// <summary>
        /// Generate food 
        /// </summary>
        void GenerateFood();

        /// <summary>
        /// Consume all the messages that are in the queue.
        /// See <see cref="ProducerConsumer.IProducerConsumerMessages{string}"/>
        /// </summary>
        void ConsumeAllMessages();

        /// <summary>
        /// for testing purposes
        /// </summary>
        /// <param name="info"></param>
        /// <param name="board"></param>
        void GameLogicTest(IInfo info, IBoard board, IRandomTest rnd);

    }
}

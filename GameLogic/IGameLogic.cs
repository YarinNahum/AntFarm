using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardNamespace;
using Utils;
using DynamicObjects;


namespace GameLogicNameSpace
{
    interface IGameLogic
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
        /// Decide how to act on a depressed dynamic object.
        /// See <see cref="State"/>
        /// <seealso cref="Info.MinObjectsPerArea"/>
        /// <seealso cref="Info.MaxObjectsPerArea"/>
        /// </summary>
        /// <param name="obj">The dynamic object that is depressed</param>
        void ActDepressedObject(IDynamicObject obj);

        /// <summary>
        /// /// Decide how to act on a Alive and non depressed dynamic object.
        /// See <see cref="State"/>
        /// <seealso cref="IDynamicObject.DecideAction"/>
        /// 
        /// </summary>
        /// <param name="obj">The dynamic object that is depressed</param>
        /// </summary>
        /// <param name="obj"></param>
        void ActAliveObject(IDynamicObject obj);

        /// <summary>
        /// Try to move an object on the board
        /// See <see cref="IBoard"/>
        /// <seealso cref="MyRandom"/>
        /// </summary>
        /// <param name="obj">The object trying to move</param>
        void Move(IDynamicObject obj);

        /// <summary>
        /// The given object argument trying to fight other dynamic object on the board.
        /// See <see cref="IDynamicObject.Fight(IDynamicObject other)"/>
        /// </summary>
        /// <param name="obj">The object that is trying to fight</param>
        void Fight(IDynamicObject obj);

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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using GameLogicNameSpace;

namespace Controller
{
    /// <summary>
    /// The main class of the game
    /// </summary>
    public class GameController
    {
        private int maxNumberOfDays;
        private GameLogic gameLogic;
        public GameController()
        {
            maxNumberOfDays = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxNumberOfDays"));
            gameLogic = new GameLogic();
            
        }

        public void StartGame()
        {
            gameLogic.GenerateInitialObjects();

            while (maxNumberOfDays > 0)
            {
                maxNumberOfDays--;
                int dynamicObjectCount = gameLogic.GetNumberOfAliveObjects();
                if (dynamicObjectCount == 0)
                {
                    gameLogic.ConsumeAllMessages();
                    Console.WriteLine("All the objects are dead.");
                    Console.WriteLine("Click enter to close the terminal");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                Console.WriteLine("Starting a new day!");
                gameLogic.StartNewDay();
                gameLogic.GenerateFood();
                gameLogic.UpdateAlive();
                gameLogic.WakeUp();
                Console.WriteLine("The day has ended!");
            }
            gameLogic.ConsumeAllMessages();
            Console.WriteLine("All days have passed. Click enter to exit");
            Console.ReadLine();
            
        }
    }
}

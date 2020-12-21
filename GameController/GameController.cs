using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MyGameLogic;

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
                int dynamicObjectCount = gameLogic.GetANumberOfAliveObjects();
                if (dynamicObjectCount == 0)
                {
                    Console.WriteLine("All the objects are dead.");
                    Console.WriteLine("Press enter to close the terminal");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                gameLogic.WakeUp();
                gameLogic.StartNewDay();
                gameLogic.GenerateFood();
                gameLogic.UpdateAlive();
            }
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Controller;
using System.Threading;


namespace AntFarm
{

    class AntFarm
    {
        static void Main()
        {
            GameController gameController = new GameController();
            gameController.StartGame();
            
        }

    }
}

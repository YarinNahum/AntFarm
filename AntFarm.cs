using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Utils;
using Controller;

namespace AntFarm
{

    class AntFarm
    {
        static void Main(string[] args)
        {
            GameController gameController = new GameController(new Info());
            gameController.StartGame();
        }

    }
}

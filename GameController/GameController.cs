using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Board;

namespace Controller
{
    public class GameController
    {
        private Info Info { get; set; }
        public GameController(Info info)
        {
            Info = info;
        }

        public void StartGame()
        {
            GameBoard gameBoard = new GameBoard(Info.NumberOfAnts, Info.Length, Info.Hight);
            int antCount = Info.NumberOfAnts;

        }
    }
}

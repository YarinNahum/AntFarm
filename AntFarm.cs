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
            Info info = ReadConfiguration();
            GameController gameController = new GameController(info);
            gameController.StartGame();

        }

        public static Info ReadConfiguration()
        {
            Info info = new Info();
            try
            {
                var m = ConfigurationManager.AppSettings;
                info.FoodPerDay = Convert.ToInt32(m.Get("FoodPerDay"));
                info.AntStrengthDecay = Convert.ToInt32(m.Get("AntStrengthDecay"));
                info.AntStartStrengthLow = Convert.ToInt32(m.Get("AntStartStrengthLow"));
                info.AntStartStrengthHigh = Convert.ToInt32(m.Get("AntStartStrengthHigh"));
                info.MinAntsPerArea = Convert.ToInt32(m.Get("MinAntsPerArea"));
                info.MaxAntsPerArea = Convert.ToInt32(m.Get("MaxAntsPerArea"));
                info.NumberOfAnts = Convert.ToInt32(m.Get("NumberOfAnts"));
                info.Length = Convert.ToInt32(m.Get("Length"));
                info.Hight = Convert.ToInt32(m.Get("Hight"));
                info.TimePerDay = Convert.ToInt32(m.Get("TimePerDay"));
                info.AntSleepDaysLow = Convert.ToInt32(m.Get("AntSleepDaysLow"));
                info.AntSleepDaysHigh = Convert.ToInt32(m.Get("AntSleepDaysHigh"));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return info;
        }
    }
}

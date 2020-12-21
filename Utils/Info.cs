using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Info
    {
        private static Info info;
        private Info()
        {
            try
            {
                var m = ConfigurationManager.AppSettings;
                FoodPerDay = Convert.ToInt32(m.Get("FoodPerDay"));
                ObjectStrengthDecay = Convert.ToInt32(m.Get("ObjectStrengthDecay"));
                ObjectStartStrengthLow = Convert.ToInt32(m.Get("ObjectStartStrengthLow"));
                ObjectStartStrengthHigh = Convert.ToInt32(m.Get("ObjectStartStrengthHigh"));
                MinObjectsPerArea = Convert.ToInt32(m.Get("MinObjectsPerArea"));
                MaxObjectsPerArea = Convert.ToInt32(m.Get("MaxObjectsPerArea"));
                NumberOfObjects = Convert.ToInt32(m.Get("NumberOfObjects"));
                Length = Convert.ToInt32(m.Get("Length"));
                Hight = Convert.ToInt32(m.Get("Hight"));
                TimePerDay = Convert.ToInt32(m.Get("TimePerDay"));
                ObjectSleepDaysLow = Convert.ToInt32(m.Get("ObjectSleepDaysLow"));
                ObjectSleepDaysHigh = Convert.ToInt32(m.Get("ObjectSleepDaysHigh"));
                ID = 1;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

        }
        public int FoodPerDay { get;}
        public int ObjectStrengthDecay { get;}
        public int ObjectStartStrengthLow { get;}
        public int ObjectStartStrengthHigh { get; }
        public int MinObjectsPerArea { get; }
        public int MaxObjectsPerArea { get;}
        public int TimePerDay { get;}
        public int NumberOfObjects { get; }
        public int Length { get; }
        public int Hight { get; }
        public int ObjectSleepDaysLow { get; }
        public int ObjectSleepDaysHigh { get; }
        public int ID { get; set; }

        public static Info Instance
        { get
            {
                if (info == null)
                    info = new Info();
                return info;
            } }

    }
}

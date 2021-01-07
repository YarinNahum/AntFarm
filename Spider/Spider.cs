using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using IDynamicObjects;
using StaticObjects;

namespace SpiderObject
{
    public class Spider : IDynamicObject
    {
        private int agilityProgress;
        public Spider() : base()
        {
            agilityProgress = 0;
            Strength = 1;
            Agility = 1;
        }

        public override string DecideAction()
        {
            return MyRandom.NextDouble() > 0.8 ? "Fight" : "Move";
        }

        protected override void ActAliveObject()
        {
            string action = DecideAction();
            if (action.Equals("Fight"))
            {
                TryToFight();
            }
            else if (action.Equals("Move"))
            {
                TryToMove();
            }
        }

        protected override void ActDepressedObject()
        {
            var objects = BoardFunctions.GetNearObjects(X, Y, 1);

            foreach (var obj in objects)
            {
                if (obj.DeBuff == DeBuff.Cocoon)
                {
                    Fight(obj);
                    SetState(State.Alive);
                    break;
                }
            }
        }

        public override void ActOnStaticObject(Food obj)
        {
            ProducerConsumer.Produce(String.Format("Spider number {0} ate food!", Id));
            Strength++;
            agilityProgress++;
            if (agilityProgress == 2)
            {
                agilityProgress = 0;
                Agility++;
            }
        }

        public override void Fight(IDynamicObject other)
        {
            
            Type t = other.GetType();
            if (t.IsAssignableFrom(GetType()))
            {
                FightSpider(other);
            }
            else
            {
                FightDifferentObject(other);
            }

        }

        private void FightSpider(IDynamicObject spider)
        {
            if (spider.DeBuff == DeBuff.Cocoon)
            {
                agilityProgress = 0;
                Strength += 2;
                Agility++;
                spider.SetState(State.Dead);
                ProducerConsumer.Produce(String.Format("Spider number {0} ate another spider with id {1}", Id, spider.Id));
            }
            else if (Agility > spider.Agility)
            {
                spider.DeBuff = DeBuff.Cocoon;
                ProducerConsumer.Produce(String.Format("Spider number {0} became a cocoone", spider.Id));
            }
        }

        private void FightDifferentObject(IDynamicObject other)
        {
            if (other.DeBuff == DeBuff.Cocoon)
            {
                agilityProgress = 0;
                Strength++;
                other.SetState(State.Dead);
                ProducerConsumer.Produce(String.Format("Spider number {0} ate another object with id {1}", Id, other.Id));
            }
            else
            {
                other.DeBuff = DeBuff.Cocoon;
                ProducerConsumer.Produce(String.Format("object number {0} became a cocoon", other.Id));
            }
        }

    }
}

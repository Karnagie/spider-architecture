using Core.Models.Stats;
using Infrastructure.Factories;
using Infrastructure.Helpers;

namespace Core.Models.Components
{
    public class SpiderStats
    {
        public IntStat Health;
        public IntStat Damage;
        public FloatStat Timer = new FloatStat(30);

        public SpiderStats(int health, int damage, SpiderTag tag)
        {
            Health = new IntStat(health);
            Damage = new IntStat(damage);
            Tag = tag;
        }

        public SpiderTag Tag;
    }
}
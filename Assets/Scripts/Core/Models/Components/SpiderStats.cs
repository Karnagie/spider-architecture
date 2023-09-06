using Core.Models.Stats;
using Infrastructure.Factories;

namespace Core.Models.Components
{
    public class SpiderStats
    {
        public IntStat Health;
        public IntStat Damage;

        public SpiderStats(int health, int damage, SpiderTag tag)
        {
            Health = new IntStat(health);
            Damage = new IntStat(damage);
            Tag = tag;
        }

        public SpiderTag Tag;
    }
}
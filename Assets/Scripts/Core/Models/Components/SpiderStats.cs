using Core.Models.Stats;

namespace Core.Models.Components
{
    public class SpiderStats
    {
        public IntStat Health;

        public SpiderStats()
        {
            Health = new IntStat(100);
        }
    }
}